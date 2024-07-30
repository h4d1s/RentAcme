using Consul;
using ConsulIntegrationHelpers.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Values;
using OpenIddict.Validation.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Authentication
builder.Services.AddAuthorization();
builder.Services
    .AddAuthentication(options => {
        options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    });

// Consul
builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
{
    var address = builder.Configuration["Consul:Address"] ?? "";
    consulConfig.Address = new Uri(address);
}));
builder.Services.AddScoped<IConsulServiceDiscovery, ConsulServiceDiscovery>();

// OpenIdDict
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        var consulServiceDiscovery = builder.Services.BuildServiceProvider().GetRequiredService<IConsulServiceDiscovery>();
        try
        {
            var address = consulServiceDiscovery.GetServiceAddress("user").Result;
            var configuration = builder.Configuration;
            var openIddictConfig = configuration.GetSection("OpenIddict");

            options.SetIssuer($"https://{address}");
            options.AddAudiences(openIddictConfig.GetSection("Audiences").Get<string[]>());

            options.UseIntrospection()
                   .SetClientId(openIddictConfig["Introspection:ClientId"])
                   .SetClientSecret(openIddictConfig["Introspection:ClientSecret"]);

            options
                .UseSystemNetHttp()
                .ConfigureHttpClientHandler(handler =>
                {
                    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                });

            options.UseAspNetCore();
        }
        catch (Exception)
        {

        }
    });
    
// CORS
builder.Services.AddCors(options =>
    options.AddPolicy(name: "RentAcmeOrigins", builder =>
        builder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials())
);

// Configure Ocelot
builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration).AddConsul();

builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
{
    var address = builder.Configuration["Consul:Address"] ?? "";
    consulConfig.Address = new Uri(address);
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("RentAcmeOrigins");
app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.UseOcelot().Wait();

app.Run();
