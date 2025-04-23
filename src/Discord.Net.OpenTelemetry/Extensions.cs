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
        public static TracerProviderBuilder AddDiscordNetInstrumentation(this TracerProviderBuilder builder)
        {
            throw new NotImplementedException();
        }

        public static MeterProviderBuilder AddDiscordNetInstrumentation(this MeterProviderBuilder builder)
        {
            throw new NotImplementedException();
        }
    }

}
