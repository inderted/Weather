using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.EntityFrameworkCore;
using Remotion.Linq.Utilities;
using Weather.Services.Authentication;
using Weather.Data;
using Weather.Services;
using Weather.Services.HangFireTasks;
using Weather.Services.HttpMethods;
using Weather.Services.SetWeathers;

namespace Weather
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //EF
            services.AddDbContext<DataContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("DBConnection")));
            //MVC
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //Hangfire
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DBConnection")));

            #region CustomService

            services.AddScoped<IBasicAuthMiddleware, BasicAuthMiddleware>();
            services.AddScoped<IHttpDataMethodService, HttpDataMethodService>();
            services.AddScoped<IDataParsingService, DataParsingService>();
            services.AddScoped<IGetWeatherTaskService, GetWeatherTaskService>();


            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseMiddleware<BasicAuthMiddleware>("tetxua.com");

            //Hangfire
            var options = new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            };
            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", options);
            RecurringJob.AddOrUpdate<IGetWeatherTaskService>(x => x.GetWeather(), Cron.Daily);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
        }

        public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
        {
            //这里需要配置权限规则
            public bool Authorize(DashboardContext context)
            {
                //var httpContext = context.GetHttpContext();
                //var authService = httpContext.RequestServices.GetRequiredService<IBasicAuthMiddleware>();
                //return authService.Invoke(httpContext).Result;
                return true;
            }
        }
    }
}
