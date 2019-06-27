using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.AddCors ();
            // services.AddDbContext<DataContext> (x => x.UseInMemoryDatabase ("TestDb"));

            services.AddMvc ();

            var connection = @"Server=(localdb)\mssqllocaldb;Database=ExampleContext1;Trusted_Connection=True;ConnectRetryCount=0";
            services.AddDbContext<DataContext>
                (options => options.UseSqlServer(connection));

            services.AddAutoMapper ();

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

            // configure DI for application services
            services.AddScoped<IUserService, UserService> ();
            services.AddScoped<ITransactionService, TransactionService> ();
            services.AddScoped<IEmailService, EmailService> ();
            services.AddSignalR ();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
            //  loggerFactory.AddConsole (Configuration.GetSection ("Logging"));
            //  loggerFactory.AddDebug ();

            // global cors policy
            app.UseCors (x => x
                .AllowAnyMethod ()
                .AllowAnyHeader ()
                .WithOrigins (
                    "http://localhost:8080"
                )
                .AllowCredentials ()
            );

            app.UseAuthentication ();
            app.UseSignalR (routes => {
                routes.MapHub<ChatHub> ("/chatHub");
            });
            app.UseMvc ();

        }
    }
}