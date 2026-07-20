using System.Security.Claims;
using AdPulse.API.Data;

namespace AdPulse.API.Middleware;

/// <summary>
/// Middleware to extract tenant ID from JWT claims and set it in the DbContext
/// This ensures multi-tenant data isolation at the database query level
/// </summary>
public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;

    public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, AdPulseDbContext dbContext)
    {
        // Extract tenant ID from JWT claims
        var tenantIdClaim = context.User.FindFirst("tenant_id")?.Value;

        if (!string.IsNullOrEmpty(tenantIdClaim) && Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            // Set the current tenant in DbContext for query filtering
            dbContext.SetCurrentTenant(tenantId);
            
            // Also add to HttpContext for easy access
            context.Items["TenantId"] = tenantId;

            _logger.LogDebug("Tenant ID set for request: {TenantId}", tenantId);
        }
        else
        {
            _logger.LogDebug("No tenant ID found in claims for authenticated request");
        }

        await _next(context);
    }
}

public static class TenantMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantMiddleware>();
    }
}
