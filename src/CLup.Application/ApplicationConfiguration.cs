using System.Reflection;
using CLup.Application.Auth;
using FluentValidation;

namespace CLup.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
