using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DialogBot1
{
    public class Program
    {
        // The Main method is the starting point of the application.
        public static void Main(string[] args)
        {
            // Build and run the host (ASP.NET Core web server)
            CreateHostBuilder(args).Build().Run();
        }

        // Create and configure the IHostBuilder
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args) // Use default .NET host settings (e.g., logging, config)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Specify the Startup class to use for configuring the app
                    webBuilder.UseStartup<Startup>();
                });
    }
}
