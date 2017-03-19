namespace Discord.Commands
{
    public class CommandServiceConfig
    {
        /// <summary> The default RunMode commands should have, if one is not specified on the Command attribute or builder. </summary>
        public RunMode DefaultRunMode { get; set; } = RunMode.Sync;

        public char SeparatorChar { get; set; } = ' ';
        /// <summary> Should commands be case-sensitive? </summary>
        public bool CaseSensitiveCommands { get; set; } = false;

        /// <summary> Gets or sets the minimum log level severity that will be sent to the Log event. </summary>
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;

        /// <summary> Gets or sets whether RunMode.Sync commands should push exceptions up to the caller. </summary>
        public bool ThrowOnError { get; set; } = true;
    }
}