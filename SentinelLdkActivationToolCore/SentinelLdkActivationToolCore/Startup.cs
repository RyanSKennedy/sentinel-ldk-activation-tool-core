using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyLogClass;
using SentinelLdkActivationToolCore.Models;

namespace SentinelLdkActivationToolCore
{
    public class Startup
    {
        public static AppSettings myAppSettings = new AppSettings();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            myAppSettings = new AppSettings("log-" + DateTime.Now.Day.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Year.ToString() + ".log");
            if (myAppSettings.LogIsEnabled) Log.Write("//=====================");
            if (myAppSettings.LogIsEnabled) Log.Write("Service started");
            if (myAppSettings.LogIsEnabled) Log.Write("//=====================");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            //Bot Configurations
            Bot.GetBotClientAsync().Wait();
        }
    }
}
