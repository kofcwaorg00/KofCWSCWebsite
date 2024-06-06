using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Enrichers;
using Serilog.Events;
using Serilog.Sinks.Slack;
using Serilog.Sinks.Slack.Models;
using Serilog.Sinks.File;


namespace KofCWSCWebsite.Services
{
    public static class ConfigureSerilogLogging
    {
        public static IHostBuilder AddSerilogLogging(
            this IHostBuilder builder, ConfigurationManager configuration)
        {
            builder.UseSerilog((hostingContext, services, loggerConfiguration) =>
            {
                var levelSwitch = services.GetRequiredService<ILoggingLevelSwitchService>().LevelSwitch;

                loggerConfiguration
                    .MinimumLevel.ControlledBy(levelSwitch)
                    .Enrich.With(new ThreadIdEnricher())
                    .Enrich.WithProperty("Version", "1.0.0")
                    .WriteTo.Console(
                        outputTemplate: "{Timestamp:HH:mm} [{Level}] ({ThreadId}) {Message}{NewLine}{Exception}")
                    .WriteTo.Logger(lc => lc
                        .Filter.ByExcluding(logEvent =>
                            logEvent.Exception?.GetType() == typeof(UnauthorizedAccessException))
                        .WriteTo.Slack(new SlackSinkOptions
                        {
                            WebHookUrl = "https://hooks.slack.com/services/XXXXXX/XXXXXX/XXXXXXX",
                            MinimumLogEventLevel = LogEventLevel.Warning
                        }))
                    .WriteTo.Debug();

                loggerConfiguration.WriteTo.ApplicationInsights(
                    services.GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Traces);

            });

            return builder;
        }
    }
}
