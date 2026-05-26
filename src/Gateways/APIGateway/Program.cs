using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// CORS
builder.Services.AddCors(options =>
    options.AddPolicy(name: "RentAcmeOrigins", builder =>
        builder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin())
);

// Health
var hcBuilder = builder.Services.AddHealthChecks();
hcBuilder
    .AddCheck("self", () => HealthCheckResult.Healthy());

// Configure Ocelot
builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true);
builder.Services
    .AddOcelot(builder.Configuration);

// Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Audience = builder.Configuration["Keycloak:Audience"];
        options.MetadataAddress = builder.Configuration["Keycloak:MetadataAddress"] ?? throw new ArgumentNullException("Keycloak metadata address is not configured");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Keycloak:Issuer"],
            RoleClaimType = ClaimTypes.Role
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("RentAcmeOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});
app.MapHealthChecks("/health/ready");
app.MapControllers();

app.MapWhen(
    context => !context.Request.Path.StartsWithSegments("/health"),
    appBranch =>
    {
        appBranch.UseOcelot().Wait();
    }
);

app.Run();
