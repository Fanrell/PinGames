using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using PinGames.Data;
using PinGames.Models;
using Serilog.Extensions.Logging.File;

namespace PinGames
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(config =>
                {
                    config.Cookie.Name = "UserLoginCookie";
                    config.LoginPath = "/Login";
                    config.AccessDeniedPath = "/Home";
                });
            services.AddMvc();
            services.AddDistributedMemoryCache();
            services.AddControllersWithViews();
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddDbContextPool<ApplicationDbContext>(
                           dbContextOptions => dbContextOptions
                               .UseMySql(
                                    Configuration.GetConnectionString("LocalHost"),
                                   mySqlOptions => mySqlOptions
                                       .CharSetBehavior(CharSetBehavior.NeverAppend))
                               // Everything from this point on is optional but helps with debugging.
                               .EnableSensitiveDataLogging()
                               .EnableDetailedErrors());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("~/Logs/logs--{Date}.txt");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Unspecified
            });
            app.UseStatusCodePages();
            

            app.UseEndpoints(endpoints =>
            {
                    endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                    endpoints.MapControllerRoute(
                    name: "profile",
                    pattern: "{controller=profile}/{action=Index}/{login?}");

                    endpoints.MapControllerRoute(
                    name: "game",
                    pattern: "{controller=profile}/{action=Game}/{gameId?}");

                    endpoints.MapControllerRoute(
                    name: "gameToLibrary",
                    pattern: "{controller=library}/{action=gameToLibrary}/{gameId?}");

                    endpoints.MapControllerRoute(
                    name: "ProfileInfo",
                    pattern: "{controller=profile}/{action=ProfileInfo}/{login?}");
            });
        }
    }
}
