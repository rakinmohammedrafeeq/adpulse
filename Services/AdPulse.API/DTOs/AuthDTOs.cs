using System.ComponentModel.DataAnnotations;

namespace AdPulse.API.DTOs;

public record LoginRequest(
    [Required][EmailAddress] string Email,
    [Required] string Password
);

public record RegisterRequest(
    [Required] string TenantName,
    [Required] string CompanyName,
    [Required][EmailAddress] string Email,
    [Required][MinLength(8)] string Password,
    [Required] string FirstName,
    [Required] string LastName
);

public record AuthResponse(
    string Token,
    Guid UserId,
    Guid TenantId,
    string Email,
    string FirstName,
    string LastName,
    string Role
);

public record ChangePasswordRequest(
    [Required] string CurrentPassword,
    [Required][MinLength(8)] string NewPassword
);
