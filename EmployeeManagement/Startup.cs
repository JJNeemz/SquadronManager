using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MimeKit;

namespace EmployeeManagement
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            {
                services.AddDbContextPool<AppDbContext>(options =>
                    options.UseSqlServer(_config.GetConnectionString("DefaultConnection")));
            }
            else
            {
                services.AddDbContextPool<AppDbContext>(options =>
                    options.UseSqlServer(_config.GetConnectionString("DefaultConnection")));
            }


            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Override default password requirements
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 3;
                options.Password.RequireNonAlphanumeric = false;

                // Require users to confirm email before logging in
                options.SignIn.RequireConfirmedEmail = true;

                // Lock user out for 30 minutes after 5 failed login attempts.
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);

            })
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();


            services.AddMvc(options => options.EnableEndpointRouting = false).AddXmlSerializerFormatters();

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = "173623478929-ks7iqdjr750foeuvn6dutiggs1jj5gpq.apps.googleusercontent.com";
                    options.ClientSecret = "HgJ_AXdUTAcZ_LQp-sDWROAR";
                })
                .AddFacebook(options =>
                {
                    options.AppId = "204646917472013";
                    options.AppSecret = "660d3e9b4d7e49d588077c74fdcc8e25";
                });

            // Changes token lifespan of all tokens.
            services.Configure<DataProtectionTokenProviderOptions>(options =>
                        options.TokenLifespan = TimeSpan.FromHours(12));


            // Claims policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role", "true"));
                //options.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin", "true"));
            });


            //The following line is saying, when someone requests the IEmployeeRepository interface,
            //Create an instance of the SQLEmployeeRepository class and inject that instance
            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
            services.AddScoped<IOfficeRepository, SQLOfficeRepository>();
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaims>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else
            {
                app.UseExceptionHandler("/Error");
                // We can specify the URL we want to go to if there is a non successful error code.
                // The placeholder automatically receives the error code.
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            app.UseRouting();

            app.UseStaticFiles();

            app.UseAuthentication();

            //app.UseMvcWithDefaultRoute();
            app.UseMvc(routes => {
                routes.MapRoute("default", "{controller=Employee}/{action=Index}/{id?}");
            });

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("app.Run Hello World");
            //});

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("app.UseEndpoints Hello World");
            //    });
            //});
        }
    }
}
