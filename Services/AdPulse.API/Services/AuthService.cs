using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AdPulse.API.Data;
using AdPulse.API.DTOs;
using AdPulse.API.Models;
using BCrypt.Net;

namespace AdPulse.API.Services;

public class AuthService : IAuthService
{
    private readonly AdPulseDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AdPulseDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            // Find user by email (ignore tenant query filter)
            var user = await _context.Users
                .IgnoreQueryFilters()
                .Include(u => u.Tenant)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("Login failed for email: {Email}", request.Email);
                return null;
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid password for user: {Email}", request.Email);
                return null;
            }

            // Check tenant is active
            if (!user.Tenant.IsActive)
            {
                _logger.LogWarning("Login attempted for inactive tenant: {TenantId}", user.TenantId);
                return null;
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Generate JWT token
            var token = GenerateJwtToken(user.Id, user.TenantId, user.Email, user.Role);

            _logger.LogInformation("User logged in successfully: {Email}", user.Email);

            return new AuthResponse(
                token,
                user.Id,
                user.TenantId,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Role
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            throw;
        }
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        try
        {
            // Check if email already exists
            var existingUser = await _context.Users
                .IgnoreQueryFilters()
                .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (existingUser)
            {
                _logger.LogWarning("Registration failed - email already exists: {Email}", request.Email);
                return null;
            }

            // Check if tenant name already exists
            var existingTenant = await _context.Tenants
                .AnyAsync(t => t.Name.ToLower() == request.TenantName.ToLower());

            if (existingTenant)
            {
                _logger.LogWarning("Registration failed - tenant name already exists: {TenantName}", request.TenantName);
                return null;
            }

            // Create new tenant
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = request.TenantName,
                CompanyName = request.CompanyName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tenants.Add(tenant);

            // Create new user (admin role for first user)
            var user = new User
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate JWT token
            var token = GenerateJwtToken(user.Id, user.TenantId, user.Email, user.Role);

            _logger.LogInformation("New tenant and user registered: {TenantName}, {Email}", tenant.Name, user.Email);

            return new AuthResponse(
                token,
                user.Id,
                user.TenantId,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Role
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
            throw;
        }
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        try
        {
            var user = await _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return false;
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                _logger.LogWarning("Invalid current password for user: {UserId}", userId);
                return false;
            }

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Password changed successfully for user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
            throw;
        }
    }

    public string GenerateJwtToken(Guid userId, Guid tenantId, string email, string role)
    {
        var jwtSecret = _configuration["JWT_SECRET"] ?? _configuration["Jwt:Secret"] 
            ?? throw new InvalidOperationException("JWT Secret not configured");
        var jwtIssuer = _configuration["JWT_ISSUER"] ?? _configuration["Jwt:Issuer"] ?? "AdPulse";
        var jwtAudience = _configuration["JWT_AUDIENCE"] ?? _configuration["Jwt:Audience"] ?? "AdPulse.API";
        var jwtExpiryMinutes = int.Parse(_configuration["JWT_EXPIRY_MINUTES"] ?? _configuration["Jwt:ExpiryMinutes"] ?? "60");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim("tenant_id", tenantId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
