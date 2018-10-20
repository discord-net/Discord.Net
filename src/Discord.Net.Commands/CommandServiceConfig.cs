using System.Collections.Generic;

namespace Discord.Commands
{
    /// <summary>
    ///     Represents a configuration class for <see cref="CommandService"/>.
    /// </summary>
    public class CommandServiceConfig
    {
        /// <summary>
        ///     Gets or sets the default <see cref="RunMode" /> commands should have, if one is not specified on the
        ///     Command attribute or builder.
        /// </summary>
        public RunMode DefaultRunMode { get; set; } = RunMode.Sync;

        /// <summary>
        ///     Gets or sets the <see cref="char"/> that separates an argument with another.
        /// </summary>
        public char SeparatorChar { get; set; } = ' ';
        
        /// <summary>
        ///     Gets or sets whether commands should be case-sensitive.
        /// </summary>
        public bool CaseSensitiveCommands { get; set; } = false;

        /// <summary>
        ///     Gets or sets the minimum log level severity that will be sent to the <see cref="CommandService.Log"/> event.
        /// </summary>
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;

        /// <summary>
        ///     Gets or sets whether <see cref="RunMode.Sync"/> commands should push exceptions up to the caller.
        /// </summary>
        public bool ThrowOnError { get; set; } = true;

        /// <summary>
        /// Collection of aliases for matching pairs of string delimiters.
        /// The dictionary stores the opening delimiter as a key, and the matching closing delimiter as the value.
        /// If no value is supplied <see cref="QuotationAliasUtils.GetDefaultAliasMap"/> will be used, which contains
        /// many regional equivalents.
        /// Only values that are specified in this map will be used as string delimiters, so if " is removed then
        /// it won't be used.
        /// If this map is set to null or empty, the default delimiter of " will be used.
        /// </summary>
        /// <example>
        /// <code language="cs">
        /// QuotationMarkAliasMap = new Dictionary&lt;char, char%gt;()
        /// {
        ///     {'\"', '\"' },
        ///     {'“', '”' },
        ///     {'「', '」' },
        /// }
        /// </code>
        /// </example>
        public Dictionary<char, char> QuotationMarkAliasMap { get; set; } = QuotationAliasUtils.GetDefaultAliasMap;

        /// <summary>
        ///     Gets or sets a value that indicates whether extra parameters should be ignored.
        /// </summary>
        public bool IgnoreExtraArgs { get; set; } = false;
    }
}
