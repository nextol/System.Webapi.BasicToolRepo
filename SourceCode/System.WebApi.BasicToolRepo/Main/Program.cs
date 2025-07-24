
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Diagnostics.CodeAnalysis;
namespace System.WebApi.BasicToolRepo
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        protected Program()
        {
            //added the protected constructor to fix Sonar Lint warning
        }
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                        .CreateLogger();
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var envName = hostingContext.HostingEnvironment.EnvironmentName;
                    config.AddJsonFile($"Main/appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"Main/appsettings.{envName}.json", optional: false, reloadOnChange: true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
