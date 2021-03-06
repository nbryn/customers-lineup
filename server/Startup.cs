using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;

using Data;
using Logic.Auth;
using Logic.Bookings;
using Logic.Businesses;
using Logic.Context;
using Logic.Employees;
using Logic.TimeSlots;
using Logic.Users;
using Logic.Util;


namespace CLup
{
    public class Startup
    {

        internal IConfiguration _config { get; private set; }

        readonly string CorsApi = "CorsApi";
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddCors(options =>
                                       {
                                           options.AddPolicy("CorsApi",
                                                             builder =>
                                                             {
                                                                 builder.WithOrigins("http://localhost:3000")
                                                                        .AllowAnyMethod()
                                                                        .AllowAnyHeader()
                                                                        .AllowCredentials();

                                                             });
                                       });

            services.AddSingleton<IConfiguration>(_config);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });
                
            services.AddAuthorization(config =>
            {
                config.AddPolicy(Policies.Admin, Policies.AdminPolicy());
                config.AddPolicy(Policies.User, Policies.UserPolicy());
            });

            services.AddScoped<ICLupContext, CLupContext>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IDTOMapper, DTOMapper>();

            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            
            services.AddScoped<ITimeSlotService, TimeSlotService>();
            services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();

            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IBookingRepository, BookingRepository>();

            services.AddScoped<IBusinessOwnerRepository, BusinessOwnerRepository>();
            services.AddScoped<IBusinessService, BusinessService>();
            services.AddScoped<IBusinessRepository, BusinessRepository>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddControllers();

            //var connectionString = "DataSource=myshareddb;mode=memory;cache=shared";

            var connectionString = _config.GetConnectionString("development");

            services.AddDbContext<CLupContext>(options =>
                              options.UseSqlServer(connectionString),
                   ServiceLifetime.Transient);

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsApi");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
