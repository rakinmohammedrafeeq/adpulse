using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdPulse.API.Services.Elasticsearch;
using System.Security.Claims;

namespace AdPulse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(IElasticsearchService elasticsearchService, ILogger<SearchController> logger)
    {
        _elasticsearchService = elasticsearchService;
        _logger = logger;
    }

    private Guid GetTenantId()
    {
        return Guid.Parse(User.FindFirst("tenant_id")?.Value 
            ?? throw new UnauthorizedAccessException("Tenant ID not found"));
    }

    /// <summary>
    /// Search campaigns by name or description
    /// </summary>
    [HttpGet("campaigns")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchCampaigns(
        [FromQuery] string q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { message = "Search query 'q' is required" });
        }

        var tenantId = GetTenantId();
        var from = (page - 1) * pageSize;

        var results = await _elasticsearchService.SearchCampaignsAsync(q, tenantId, from, pageSize);

        return Ok(new
        {
            query = q,
            page,
            pageSize,
            total = results.Count,
            results
        });
    }
}
