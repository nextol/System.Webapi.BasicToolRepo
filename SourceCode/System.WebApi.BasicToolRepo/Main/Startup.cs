
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.WebApi.BasicToolRepo.Swagger;
using System;
using Microsoft.AspNetCore.HttpOverrides;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using System.Webapi.BasicToolRepo.Helper;
using System.Webapi.BasicToolRepo;
using System.Webapi.BasicToolRepo.Extensions;
namespace System.WebApi.BasicToolRepo
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public CoreSdkOptions CoreOpts { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            //Following sample tests, API, OnePGSQL DB & One Thirdparty service
            var coreOpts = CoreSdkOptions.GetDefaultOpts();
            coreOpts.Logging.Enable = true;//By default it is enabled on SDK but better we do it explicitly 
            coreOpts.HealthCheck.Enable = true;//Make sure if you have any disabled Core SDK Featurs disabled, add it here.
            CoreOpts = coreOpts;
        }
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCoreServices(Configuration, CoreOpts);
            services.ConfigureCors();
            //Register Version
            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionReader = new HeaderApiVersionReader("X-version");
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "System.Webapi.BasicToolRepo", Version = "v1" });
                c.OperationFilter<GceHeaderFilter>();
                c.CustomSchemaIds(type => type.ToString());
            });

            services.AddHttpClient();
            services.AddControllers();
            services.AddAutoMapper(typeof(Startup));
            services.ConfigureApplicationDependencies(Configuration);
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true; // Enforce HSTS on all Sub-Domains as well
                options.MaxAge = TimeSpan.FromDays(365); // One year expiry
            });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Webapi.BasicToolRepo.Contracts.InterFaces.IBasicToolLogger<Startup> logger)
        {
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UsePathBase("/System.Webapi.BasicToolRepo");
            logger.Info("API Configure - Started");

            //app.UseDeveloperExceptionPage();//Generic error response won't work if Developer Exception Page is enabled
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/System.Webapi.BasicToolRepo/swagger/v1/swagger.json", "System.Webapi.BasicToolRepo v1"));
            if (env.IsDevelopment())
            {
                // Remove this in order to see the generic error response.If Developer Exception Page is eabled the custom json error wont work.
                // app.UseDeveloperExceptionPage();
            }
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // Add health check endpoints if enabled
                if (CoreOpts?.HealthCheck?.Enable == true)
                {
                    endpoints.MapHealthChecks("/health");
                }
            });
            logger.Info("API Configure - Completed");
        }
    }
}
