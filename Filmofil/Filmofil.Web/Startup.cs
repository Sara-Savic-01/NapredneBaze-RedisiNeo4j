using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Filmofil.Business;
using Filmofil.Business.CategorieManagment;
using Filmofil.Neo4J;
using Filmofil.Redis;
using Filmofil.Services;
using Filmofil.Services.Hubs;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;
using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace Filmofil.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddFilmofilServices();
            services.AddRedisServices(Configuration);
            services.AddNeo4JServices(Configuration);
            services.AddBussinesServices();
            services.AddSignalR();

            services.AddCors(options =>
            {
                options.AddPolicy("MyCorsPolicy",
                builder => builder
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .Build()
                );
            });

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
             services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Filmofil", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, ICategorieManager categorieManager, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Filmofil v1"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            categorieManager.CheckCategoriesInRedis();

            var cultureInfo = new CultureInfo("en-US");
            cultureInfo.NumberFormat.CurrencySymbol = "€";

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseCors("MyCorsPolicy");

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                endpoints.MapHub<MessageHub>("/chat");
            });

        }
    }
}
