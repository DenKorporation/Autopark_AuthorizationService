using System.Reflection;
using AuthorizationService.BLL.Services.Implementations;
using AuthorizationService.BLL.Services.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthorizationService.BLL.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddBllServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.ConfigureMapping();
        services.ConfigureValidation();
        services.ConfigureServices();

        return services;
    }

    private static IServiceCollection ConfigureMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }

    private static IServiceCollection ConfigureValidation(this IServiceCollection services)
    {
        services
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services
            .AddFluentValidationAutoValidation();

        return services;
    }

    private static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IContractService, ContractService>();
        services.AddScoped<IPassportService, PassportService>();
        services.AddScoped<IWorkBookService, WorkBookService>();

        return services;
    }
}
