using CustomLogger.Logger;
using CustomLogger.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CustomLogger
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Adds services required for using options.
            services.AddOptions();

            // Configure settings
            services.Configure<CustomLoggerSettings>(Configuration.GetSection("CustomLogger"));

            // Services
            services.AddScoped<IMailService, DebugMailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<CustomLoggerSettings> customLoggerSettings, IMailService mailService)
        {
            loggerFactory.AddConsole();

            // Register custom logger
            loggerFactory.AddCustomLogger(customLoggerSettings, mailService, env.IsProduction() ? LogLevel.Error : LogLevel.Information);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                ILogger logger = loggerFactory.CreateLogger<Startup>();

                logger.LogInformation("API LoggerTests 1 - Normal Log");
                logger.LogCritical("API LoggerTests 2 - Critical Log with Send Mail");

                await context.Response.WriteAsync("Test log added, check it out in the `Logs` folder.");
            });
        }
    }
}
