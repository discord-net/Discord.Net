using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System;

namespace Discord.OpenTelemetry
{
    /// <summary>
    /// An extension class which contains methods to added the Discord.Net OpenTelemetry instrumentations.
    /// </summary>
    public static class Extensions
    {
        private static readonly string[] SourceNames = ["Discord.Net.WebSocket", "Discord.Net.Audio"];

        public static TracerProviderBuilder AddDiscordNetInstrumentation(this TracerProviderBuilder builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));
            return builder.AddSource(SourceNames);
        }

        public static MeterProviderBuilder AddDiscordNetInstrumentation(this MeterProviderBuilder builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));
            return builder.AddMeter(SourceNames);
        }
    }

}
