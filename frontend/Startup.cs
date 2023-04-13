using System;
using DopplerSDK.ConfigurationProvider;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace frontend
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
            services.AddRazorPages();

            var dopplerClientConfig = Configuration.Get<DopplerClientConfiguration>();
            dopplerClientConfig.DopplerNameTransformer = DopplerNameTransformers.None;
            var dopplerClient = new DopplerClient(dopplerClientConfig);
            var dopplerClientResponse = dopplerClient.FetchSecretsAsync().Result;
            foreach (var secret in dopplerClientResponse.Secrets) Configuration[secret.Key] = secret.Value;

            services.AddHttpClient<PizzaClient>(client => 
            {
                var baseAddress = new Uri(Configuration.GetValue<string>("backendUrl"));

                client.BaseAddress = baseAddress;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

    }
}
