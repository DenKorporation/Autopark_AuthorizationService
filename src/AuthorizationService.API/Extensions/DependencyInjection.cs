using System.Security.Cryptography;
using Asp.Versioning;
using AuthorizationService.API.Middleware;
using AuthorizationService.API.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace AuthorizationService.API.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.ConfigureApiVersioning();

        services.ConfigureAuthentication(configuration);
        services.AddAuthorization();

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.ConfigureOpenApi();

        return services;
    }

    private static IServiceCollection ConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(
            options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            }).AddApiExplorer(
            options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }

    private static IServiceCollection ConfigureAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
            .AddJwtBearer(
                options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidIssuer = configuration.GetRequiredSection("Identity:Url").Value,
                        IssuerSigningKey =
                            GetRsaPublicKey(configuration.GetRequiredSection("Identity:PublicKey").Value!),
                    };
                });

        return services;
    }

    private static IServiceCollection ConfigureOpenApi(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.ConfigureOptions<ConfigureSwaggerGenOptions>();

        return services;
    }

    private static RsaSecurityKey GetRsaPublicKey(string publicKeyBase64)
    {
        var rsa = RSA.Create();

        var publicKeyBytes = Convert.FromBase64String(publicKeyBase64);
        rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        return new RsaSecurityKey(rsa);
    }
}
