using System;

namespace Discord.Commands
{
    public class CommandServiceConfig
    {
        /// <summary> Gets or sets the default RunMode commands should have, if one is not specified on the Command attribute or builder. </summary>
        public RunMode DefaultRunMode { get; set; } = RunMode.Sync;

        public char SeparatorChar { get; set; } = ' ';
        
        /// <summary> Determines whether commands should be case-sensitive. </summary>
        public bool CaseSensitiveCommands { get; set; } = false;

        /// <summary> Gets or sets the minimum log level severity that will be sent to the Log event. </summary>
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;

        /// <summary> Determines whether RunMode.Sync commands should push exceptions up to the caller. </summary>
        public bool ThrowOnError { get; set; } = true;

        /// <summary> Determines whether extra parameters should be ignored. </summary>
        public bool IgnoreExtraArgs { get; set; } = false;

        ///// <summary> Gets or sets the <see cref="IServiceProvider"/> to use. </summary>
        //public IServiceProvider ServiceProvider { get; set; } = null;

        ///// <summary> Gets or sets a factory function for the <see cref="IServiceProvider"/> to use. </summary>
        //public Func<CommandService, IServiceProvider> ServiceProviderFactory { get; set; } = null;
    }
}
