using CLup.API.Exceptions;
using CLup.API.Extensions;
using CLup.Application;
using CLup.Application.Auth;
using CLup.Domain;
using CLup.Infrastructure;
using FluentValidation.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace CLup.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // TODO: Load from AppSettings.
        builder.WebHost.UseUrls("http://localhost:5001");
        ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

        var app = builder.Build();
        await Configure(app, builder.Environment);
    }

    private static void ConfigureServices(
        IServiceCollection services,
        IConfiguration config,
        IWebHostEnvironment environment)
    {
        services
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddRouting(options => options.LowercaseUrls = true)
            .ConfigureCors(config)
            .ConfigureJwt(config)
            .ConfigureSwagger()
            .AddAuthorization(config =>
            {
                config.AddPolicy(Policies.Admin, Policies.AdminPolicy());
                config.AddPolicy(Policies.User, Policies.UserPolicy());
            })
            .AddControllers()
            .AddFluentValidation(fv =>
            {
                fv.ImplicitlyValidateChildProperties = true;
            });

        services.AddSingleton(config);
        services
            .ConfigureDomain(config)
            .ConfigureApplication(config)
            .ConfigureInfrastructure(config, environment);
    }

    public static async Task Configure(WebApplication app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        await app.ConfigureSeed();
        app.UseHttpsRedirection();

        app.UseSwagger();
        app.UseSwaggerUI(swaggerUiOptions =>
        {
            swaggerUiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Customers Lineup API");
            swaggerUiOptions.RoutePrefix = string.Empty;
            swaggerUiOptions.DocExpansion(DocExpansion.None);
            swaggerUiOptions.DisplayRequestDuration();
        });

        app.UseExceptionHandler();
        app.UseRouting();
        app.UseCors("CorsApi");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}