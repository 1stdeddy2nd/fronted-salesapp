using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using SalesAnalytics.Data;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load secrets from Azure Key Vault using Managed Identity
builder.Configuration.AddAzureKeyVault(
    new Uri("https://salesanalytics-kv.vault.azure.net/"),
    new DefaultAzureCredential()
);

// ✅ Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ✅ Services
builder.Services.AddDbContext<SalesDbContext>(options =>
    options.UseSqlServer(builder.Configuration["DbConnectionString"])
);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
        options.TokenValidationParameters.RoleClaimType = 
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                var claims = context.Principal?.Claims.Select(c => $"{c.Type}: {c.Value}");
                logger.LogInformation("All claims:\n{Claims}", string.Join("\n", claims ?? Enumerable.Empty<string>()));
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError("Authentication failed: {Message}", context.Exception.Message);
                return Task.CompletedTask;
            }
        };
    },
    options => builder.Configuration.Bind("AzureAd", options)
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    var apiScope = $"api://{builder.Configuration["AzureAd:ClientId"]}/App.Read";

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sales API", Version = "v1" });
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "OAuth2 Authorization Code with PKCE",
        Name = "oauth2",
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/authorize"),
                TokenUrl = new Uri($"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/token"),
                Scopes = new Dictionary<string, string>
                {
                    { apiScope, "Read access to Sales App API" }
                }
            }
        }
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            },
            new[] { apiScope }
        }
    });
});

var app = builder.Build();

// ✅ Global error handler
app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "🔥 Unhandled exception occurred");
        throw;
    }
});

// ✅ Startup log
var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();
startupLogger.LogInformation("🚀 SalesApp started at {Time}", DateTime.UtcNow);

// ✅ Middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sales API v1");
    c.OAuthClientId(builder.Configuration["AzureAd:SwaggerClientId"]);
    c.OAuthUsePkce();
    c.OAuthScopeSeparator(" ");
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", (IConfiguration config, SalesDbContext db) =>
{
    var dbConn = config["DbConnectionString"] ?? "[null]";
    return Results.Ok(new
    {
        status = "Healthy",
        dbConnection = dbConn.Length > 10 ? "Loaded" : "Missing",
        time = DateTime.UtcNow
    });
});

app.Run();
