using AdPulse.API.DTOs;

namespace AdPulse.API.Services;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    string GenerateJwtToken(Guid userId, Guid tenantId, string email, string role);
}
