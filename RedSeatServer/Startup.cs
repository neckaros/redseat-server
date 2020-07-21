using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedSeatServer.Models;
using System.Text.Json.Serialization;
using Hangfire;
using Hangfire.Storage.SQLite;
using RedSeatServer.Services;
using RedSeatServer.Downloaders;

namespace RedSeatServer
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
            services.AddHangfire(configuration => configuration
       .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
       .UseSimpleAssemblyNameTypeSerializer()
       .UseRecommendedSerializerSettings()
       .UseSQLiteStorage());

            services.AddHttpClient();

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            services.AddDbContext<RedseatDbContext>();
            services.AddTransient<IDownloaderService, DownloaderService>();
            services.AddTransient<IIoService, IoService>();
            services.AddControllers().AddJsonOptions(opts =>
            {
                //opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                //opts.JsonSerializerOptions.IgnoreNullValues = true;
                opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBackgroundJobClient backgroundJobs, IRecurringJobManager recurringJob)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/error");
            }
            else
            {
                app.UseExceptionHandler("/error");
            }
                    

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseHangfireDashboard();
            backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));
            recurringJob.AddOrUpdate<MonitorProgressService>("monitor_downloads", (p) => p.CheckAllDownloads(), "*/20 * * * * *");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
