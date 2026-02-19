using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System;

namespace Discord.OpenTelemetry
{
    /// <summary>
    /// An extension class which contains methods to add the Discord.Net OpenTelemetry instrumentation.
    /// </summary>
    public static class Extensions
    {
        private static readonly string[] _sourceNames = ["Discord.Net.WebSocket", "Discord.Net.Audio"];

        /// <summary>
        /// Adds the trace sources of DNet.
        /// </summary>
        /// <param name="builder">The trace provider to add these sources to.</param>
        /// <returns>The provided trace provider to chain calls.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TracerProviderBuilder AddDiscordNetInstrumentation(this TracerProviderBuilder builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));
            return builder.AddSource(_sourceNames);
        }

        /// <summary>
        /// Adds the meters of DNet.
        /// </summary>
        /// <param name="builder">The meter provider to add the meters to.</param>
        /// <returns>The provided meter builder to chain calls.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static MeterProviderBuilder AddDiscordNetInstrumentation(this MeterProviderBuilder builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));
            return builder.AddMeter(_sourceNames);
        }
    }

}
