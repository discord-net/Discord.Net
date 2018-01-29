using System.Collections.Generic;
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

        /// <summary> Collection of aliases that can wrap strings for command parsing. 
        /// represents the opening quotation mark and the value is the corresponding closing mark.</summary>
        public Dictionary<char, char> QuotationMarkAliasMap { get; set; }
        = new Dictionary<char, char> {
            {'\"', '\"' },
            {'«', '»' },
            {'‘', '’' },
            {'“', '”' },
            {'„', '‟' },
            {'‹', '›' },
            {'‚', '‛' },
            {'《', '》' },
            {'〈', '〉' },
            {'「', '」' },
            {'『', '』' },
            {'〝', '〞' },
            {'﹁', '﹂' },
            {'﹃', '﹄' },
            {'＂', '＂' },
            {'＇', '＇' },
            {'｢', '｣' }
        };

        /// <summary> Determines whether extra parameters should be ignored. </summary>
        public bool IgnoreExtraArgs { get; set; } = false;
    }
}
