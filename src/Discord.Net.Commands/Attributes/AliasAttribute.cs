using System;

namespace Discord.Commands
{
    /// <summary>
    ///     Marks the aliases for a command.
    /// </summary>
    /// <remarks>
    ///     This attribute allows a command to have one or multiple aliases. In other words, the base command can have
    ///     multiple aliases when triggering the command itself, giving the end-user more freedom of choices when giving
    ///     hot-words to trigger the desired command. See the example for a better illustration.
    /// </remarks>
    /// <example>
    ///     In the following example, the command can be triggered with the base name, "stats", or either "stat" or
    ///     "info".
    ///     <code language="cs">
    ///     [Command("stats")]
    ///     [Alias("stat", "info")]
    ///     public async Task GetStatsAsync(IUser user)
    ///     {
    ///         // ...pull stats
    ///     }
    ///     </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AliasAttribute : Attribute
    {
        /// <summary>
        ///     Gets the aliases which have been defined for the command.
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        ///     Creates a new <see cref="AliasAttribute" /> with the given aliases.
        /// </summary>
        public AliasAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }
    }
}
