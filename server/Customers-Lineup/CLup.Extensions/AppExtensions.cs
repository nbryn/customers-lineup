using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using CLup.Data;
using CLup.Data.Initializer;

namespace CLup.Extensions
{
    public static class AppExtensions
    {

        public static void ConfigureDataInitialiser(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            CLupContext context = scope.ServiceProvider.GetRequiredService<CLupContext>();

            context.Database.EnsureCreated();
            var dataInitializer = scope.ServiceProvider.GetService<DataInitializer>();

            dataInitializer.InitializeSeed();
        }
    }
}