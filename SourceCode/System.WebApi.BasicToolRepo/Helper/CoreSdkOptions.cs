using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace System.Webapi.BasicToolRepo.Helper
{
    public class CoreSdkOptions
    {
        public LoggingOptions Logging { get; set; } = new LoggingOptions();
        public HealthCheckOptions HealthCheck { get; set; } = new HealthCheckOptions();

        // Add more feature-specific options as needed...

        public static CoreSdkOptions GetDefaultOpts()
        {
            return new CoreSdkOptions
            {
                Logging = new LoggingOptions { Enable = true },
                HealthCheck = new HealthCheckOptions { Enable = true }
            };
        }
    }
    public class LoggingOptions
    {
        public bool Enable { get; set; } = true;
        // You can add more logging configuration here
    }
    public class HealthCheckOptions
    {
        public bool Enable { get; set; } = true;
        // You can add health check URI, timeout, etc.
    }
}
