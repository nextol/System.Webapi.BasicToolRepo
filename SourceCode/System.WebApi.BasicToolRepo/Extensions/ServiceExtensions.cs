using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Webapi.BasicToolRepo.Entities.Models;
using System.Webapi.BasicToolRepo.Factories.Concrete;
using System.Webapi.BasicToolRepo.Factories;
using System.Webapi.BasicToolRepo.Helper;
using System.Webapi.BasicToolRepo.Entities.RequestContext;
using System;
using System.Webapi.BasicToolRepo.Infrastructure.Repositories;
using System.Webapi.BasicToolRepo.Contracts.InterFaces;
using Serilog;
using System.Webapi.BasicToolRepo.Logging;

namespace System.Webapi.BasicToolRepo
{
    [ExcludeFromCodeCoverage]
    /// <summary>
    /// Enables us to organize and extend  framework functionalities
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Without Block body
        /// </summary>
        /// <param name="services"></param>
        //public static void ConfigureCors(this ServiceCollection services) =>
        //    services.AddCors(options => 
        //    { 
        //        options.AddPolicy("CorsPolicy", builder => 
        //            builder.AllowAnyOrigin()
        //            .AllowAnyMethod()]
        //            .AllowAnyHeader());
        //    });
        public static IServiceCollection ConfigureCoreServices(
        this IServiceCollection services,
        IConfiguration configuration,
        CoreSdkOptions options)
        {
            services.AddHttpContextAccessor();

            // Setup logging, health checks, other SDKs...
            if (options.Logging.Enable)
            {
                services.AddLogging(); // or custom logging setup
            }

            if (options.HealthCheck.Enable)
            {
                services.AddHealthChecks(); // add more config as needed
            }
            
            // Register more services...

            return services;
        }
        /// <summary>
        ///  configure CORS in our application.
        ///  CORS (Cross-Origin Resource Sharing) is a mechanism to give or restrict access rights to applications from different domains
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin() // TO DO:: This is okay for DEV.. Should be restrictive with CORS settings in the production environment
                .AllowAnyMethod()
                .AllowAnyHeader());
            });

        }


        /// <summary>
        /// ASP.NET Core applications are by default self hosted, 
        /// and if we want to host our application on IIS, we need to configure an IIS integration
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options => { }); // TO DO:: Fine tune defaults
        }



        /// <summary>
        /// Register the RepositoryConfiguration class ( Ent.Lib connection) in the application’s dependency injection container
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureApplicationDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Appsettings>(configuration.GetSection("AppSettings"));

            //----GDS Context Resolver----//
            services.ConfigureGceRequestContext();
            //----Services & Repositories---//
            services.AddScoped<IApiClient, ApiClient>();
            //-----Factory DI------//
            services.AddScoped(typeof(IBasicToolLogger<>), typeof(Logger<>));
            services.AddScoped<IBasicToolImageServiceFactory, BasicToolServiceFactory>();
            services.AddScoped<IPdfToolServiceFactory, PdfServiceFactory>();
        }

        /// <summary>
        /// Coonfigure versioning extensions
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureVersioning(this IServiceCollection services) 
        { 
            services.AddApiVersioning(opt =>
            { 
                opt.ReportApiVersions = true; 
                opt.AssumeDefaultVersionWhenUnspecified = true; 
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
            }); 
        }


        /// <summary>
        /// Configures swagger middleware
        /// </summary>
        /// <param name="services"></param>

        public static void ConfigureGceRequestContext(this IServiceCollection services)
        {
            services.AddScoped<IRequestContextProvider, RequestContextResolver>();
            services.AddScoped((Func<IServiceProvider, IRequestContextInfo>)(s => s.GetRequiredService<IRequestContextProvider>().GetCurrentContext()));
        }
    }
}
