using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using AdPulse.API.Data;
using AdPulse.API.DTOs;
using AdPulse.API.Models;
using AdPulse.API.Services;
using Xunit;

namespace AdPulse.Tests.Unit;

public class AuthServiceTests
{
    private AdPulseDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AdPulseDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AdPulseDbContext(options);
    }

    private IConfiguration CreateTestConfiguration()
    {
        var configValues = new Dictionary<string, string?>
        {
            { "Jwt:Secret", "this-is-a-secure-32-byte-test-jwt-secret-key-1234" },
            { "Jwt:Issuer", "AdPulse" },
            { "Jwt:Audience", "AdPulse.API" },
            { "Jwt:ExpiryMinutes", "60" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();
    }

    [Fact]
    public async Task RegisterAsync_CreatesNewTenantAndAdminUser()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var config = CreateTestConfiguration();
        var mockLogger = new Mock<ILogger<AuthService>>();
        var service = new AuthService(context, config, mockLogger.Object);

        var request = new RegisterRequest(
            TenantName: "Acme Corp",
            CompanyName: "Acme Corporation Global",
            Email: "admin@acmecorp.com",
            Password: "SecurePassword123!",
            FirstName: "Jane",
            LastName: "Doe"
        );

        // Act
        var response = await service.RegisterAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.False(string.IsNullOrEmpty(response.Token));
        Assert.Equal("admin@acmecorp.com", response.Email);
        Assert.Equal("Admin", response.Role);

        // Verify persistence
        var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.Name == "Acme Corp");
        Assert.NotNull(tenant);
        var user = await context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Email == "admin@acmecorp.com");
        Assert.NotNull(user);
        Assert.True(BCrypt.Net.BCrypt.Verify("SecurePassword123!", user.PasswordHash));
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var config = CreateTestConfiguration();
        var mockLogger = new Mock<ILogger<AuthService>>();
        var service = new AuthService(context, config, mockLogger.Object);

        var tenantId = Guid.NewGuid();
        var tenant = new Tenant
        {
            Id = tenantId,
            Name = "Test Company",
            CompanyName = "Test Company LLC",
            IsActive = true
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = "user@testcompany.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword!"),
            FirstName = "John",
            LastName = "Smith",
            Role = "User",
            IsActive = true
        };

        context.Tenants.Add(tenant);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var loginRequest = new LoginRequest("user@testcompany.com", "CorrectPassword!");

        // Act
        var result = await service.LoginAsync(loginRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("user@testcompany.com", result.Email);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.Equal(tenantId, result.TenantId);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var config = CreateTestConfiguration();
        var mockLogger = new Mock<ILogger<AuthService>>();
        var service = new AuthService(context, config, mockLogger.Object);

        var tenantId = Guid.NewGuid();
        context.Tenants.Add(new Tenant { Id = tenantId, Name = "Org", CompanyName = "Org LLC", IsActive = true });
        context.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = "user@org.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("RealPassword!"),
            FirstName = "Alice",
            LastName = "Wonder",
            Role = "User",
            IsActive = true
        });
        await context.SaveChangesAsync();

        var loginRequest = new LoginRequest("user@org.com", "WrongPassword!");

        // Act
        var result = await service.LoginAsync(loginRequest);

        // Assert
        Assert.Null(result);
    }
}
