﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyOtpDemoBankWeb.Api;
using MyOtpDemoBankWeb.Models;

namespace MyOtpDemoBankWeb
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


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddDbContext<ApplicationDbContext>(context => { context.UseInMemoryDatabase("GothamBankDb"); });
            services.AddHttpClient<LoginController>();

            services.Configure<OtpServerSettings>(options =>
            {
                options.EndPoint = Configuration.GetSection("OtpServer:EndPoint").Value;
                options.Controller = Configuration.GetSection("OtpServer:Controller").Value;
                options.GetTOtpAction = Configuration.GetSection("OtpServer:GetTOtpAction").Value;
                options.ValidataTOtpAction = Configuration.GetSection("OtpServer:ValidataTOtpAction").Value;
                options.SharedKey = Configuration.GetSection("OtpServer:SharedKey").Value;
            });
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
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
