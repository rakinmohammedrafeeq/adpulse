using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Compact;
using AdPulse.API.Data;
using AdPulse.API.Infrastructure;
using AdPulse.API.Middleware;
using AdPulse.API.Services;
using AdPulse.API.Services.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
DotNetEnv.Env.Load();

// Configure Serilog
var logstashHost = Environment.GetEnvironmentVariable("LOGSTASH_HOST") ?? "localhost";
var logstashPort = Environment.GetEnvironmentVariable("LOGSTASH_PORT") ?? "8080";

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "AdPulse.API")
    .WriteTo.Console()
    .WriteTo.File(new CompactJsonFormatter(), "logs/adpulse-.json", rollingInterval: RollingInterval.Day)
    .WriteTo.Http(
        requestUri: $"http://{logstashHost}:{logstashPort}",
        queueLimitBytes: null)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new UtcDateTimeJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new NullableUtcDateTimeJsonConverter());
    });
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AdPulse Campaign Management API",
        Version = "v1",
        Description = "REST API for managing advertising campaigns, ad groups, creatives, audiences, events, and analytics"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure database
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost,1433;Database=AdPulse;User Id=sa;Password=AdPulse2026!;TrustServerCertificate=True;MultipleActiveResultSets=True";

builder.Services.AddDbContext<AdPulseDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure JWT authentication
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? builder.Configuration["Jwt:Secret"]
    ?? "your-super-secret-jwt-key-change-this-in-production-min-32-chars";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? builder.Configuration["Jwt:Issuer"] ?? "AdPulse",
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? builder.Configuration["Jwt:Audience"] ?? "AdPulse.API",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Configure CORS
var corsOrigins = Environment.GetEnvironmentVariable("CORS_ORIGINS")
    ?? builder.Configuration["Cors:Origins"]
    ?? "http://localhost:3000,http://localhost:5173,http://localhost:4173";

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(corsOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Register application services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICampaignService, CampaignService>();
builder.Services.AddSingleton<IElasticsearchService, ElasticsearchService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

var app = builder.Build();

// Auto-migrate and seed demo database & Elasticsearch
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var elasticsearch = services.GetRequiredService<IElasticsearchService>();
    await elasticsearch.InitializeAsync();

    try
    {
        var dbContext = services.GetRequiredService<AdPulseDbContext>();
        await DbInitializer.InitializeAsync(dbContext, elasticsearch, logger);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Database initialization warning: {Message}", ex.Message);
    }
}

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AdPulse API v1");
    c.RoutePrefix = "swagger";
});

app.UseSerilogRequestLogging();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

// Apply tenant middleware after authentication
app.UseTenantMiddleware();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    application = "AdPulse.API"
}));

// Root redirect to Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// Log startup
Log.Information("AdPulse API starting up...");
Log.Information("Environment: {Environment}", app.Environment.EnvironmentName);

app.Run();

// Ensure Serilog flushes on shutdown
Log.CloseAndFlush();
