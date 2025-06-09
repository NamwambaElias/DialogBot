using DialogBot1.Bots;
using DialogBot1.Dialogs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DialogBot1
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        // Constructor receives the app configuration (from appsettings.json, environment variables, etc.)
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method is used to register services with the dependency injection container
        public void ConfigureServices(IServiceCollection services)
        {
            // Register the bot's credentials (empty app ID/secret for local testing)
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();

            // Register memory storage for state (used to store conversation data in memory)
            services.AddSingleton<IStorage, MemoryStorage>();

            // Register user and conversation state
            services.AddSingleton<UserState>();
            services.AddSingleton<ConversationState>();

            // Register dialogs
            services.AddSingleton<GreetingDialog>();
            services.AddSingleton<FlightBookingDialog>();
            services.AddSingleton<MainDialog>(); // Handles greeting + booking steps

            // Register the bot adapter with error handling
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Register the bot itself (EchoBot uses MainDialog)
            services.AddTransient<IBot, EchoBot>();

            // Add support for controllers (BotController.cs)
            services.AddControllers().AddNewtonsoftJson();
        }

        // This method configures the app's HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Show detailed error pages in development
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Route HTTP requests
            app.UseDefaultFiles();    // Use default index.html if present
            app.UseStaticFiles();     // Serve static content (e.g., .css, .js)

            app.UseRouting();         // Enable request routing

            // Map bot endpoints (e.g., POST /api/messages)
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
