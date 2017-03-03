using System;

namespace Discord.Commands
{
    /// <summary> Provides aliases for a command. </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AliasAttribute : Attribute
    {
        /// <summary> The aliases which have been defined for the command. </summary>
        public string[] Aliases { get; }

        /// <summary> Creates a new <see cref="AliasAttribute"/> with the given aliases. </summary>
        public AliasAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }
    }
}
