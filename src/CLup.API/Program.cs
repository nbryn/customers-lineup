using System.Reflection;
using System.Text.Json.Serialization;
using CLup.API.Exceptions;
using CLup.API.Extensions;
using CLup.API.Middleware;
using CLup.Application;
using CLup.Application.Auth;
using CLup.Domain;
using CLup.Infrastructure;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace CLup.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var appSettings = new AppSettings();
        builder.Configuration.GetSection("settings").Bind(appSettings);
        builder.Services.AddSingleton(appSettings);
        builder.WebHost.UseUrls(appSettings.Url);

        ConfigureServices(builder.Services, appSettings, builder);

        var app = builder.Build();
        await Configure(app, builder.Environment);
        app.Run();
    }

    private static void ConfigureServices(
        IServiceCollection services,
        AppSettings appSettings,
        WebApplicationBuilder builder)
    {
        services
            .ConfigureCors()
            .ConfigureDomain()
            .ConfigureSwagger()
            .ConfigureApplication()
            .ConfigureJwt(appSettings)
            .ConfigureSerilog(builder, appSettings)
            .ConfigureInfrastructure(appSettings, builder.Environment)
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddRouting(options => options.LowercaseUrls = true)
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
            .Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressInferBindingSourcesForParameters = true;
            })
            .AddAuthorization(config =>
            {
                config.AddPolicy(Policies.Admin, Policies.AdminPolicy());
                config.AddPolicy(Policies.User, Policies.UserPolicy());
            })
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .AddFluentValidation(fv =>
            {
                fv.ImplicitlyValidateChildProperties = true;
            });
    }

    private static async Task Configure(WebApplication app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        await app.ConfigureSeed();
        app.UseHttpsRedirection();

        app.UseMiddleware<RequestLogContextMiddleware>();
        app.UseSerilogRequestLogging();

        app.UseSwagger();
        app.UseSwaggerUI(swaggerUiOptions =>
        {
            swaggerUiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Customers Lineup API");
            swaggerUiOptions.RoutePrefix = string.Empty;
            swaggerUiOptions.DocExpansion(DocExpansion.None);
            swaggerUiOptions.DisplayRequestDuration();
        });

        if (!env.IsDevelopment())
        {
            app.UseExceptionHandler(_ => { });
        }

        app.UseRouting();
        app.UseCors("CorsApi");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}
