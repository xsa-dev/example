using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AutoMapper;
using WebApi.Core.Domain.Entities;
using WebApi.Core.Services;
using WebApi.Helpers;
using WebApi.Hubs;

namespace WebApi
{
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices (IServiceCollection services) {
            services.AddCors ();
            services.AddMvc (
                options => options.EnableEndpointRouting = false );
            
            var connection = @"Server=localhost,1433;Database=WebApi;ConnectRetryCount=0;User=sa;Password=ToPSG5!Rf";
            
            services.AddDbContext<DataContext>
                (options => options.UseSqlServer(connection));

            services.AddAutoMapper(typeof(UserDto));

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection ("AppSettings");
            services.Configure<AppSettings> (appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings> ();
            var key = Encoding.ASCII.GetBytes (appSettings.Secret);
            services.AddAuthentication (x => {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer (x => {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey (key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddScoped<IUserService, UserService> ();
            services.AddScoped<ITransactionService, TransactionService> ();
            services.AddScoped<IEmailService, EmailService> ();
            services.AddSignalR ();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseRouting();
            
            app.UseCors (x => x
                .AllowAnyMethod ()
                .AllowAnyHeader ()
                .WithOrigins (
                    "http://localhost:8080"
                )
                .AllowCredentials ()
            );
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Ping}/{action=Pong}");
                    endpoints.MapControllerRoute("default", "{controller=Ping}/{action=Pong}");
                endpoints.MapHub<ChatHub>("/chatHub");
            });

            app.UseAuthentication ();
        }
    }
}