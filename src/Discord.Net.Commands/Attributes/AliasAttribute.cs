using System;

namespace Discord.Commands
{
    /// <summary> Provides aliases for a command. </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple=true)]
    public class AliasAttribute : Attribute
    {
        /// <summary> The type to be applied to this alias group <summary>
        public AliasType Type { get; }
        /// <summary> The aliases which have been defined for the command. </summary>
        public string[] Aliases { get; }

        /// <summary> Creates a new <see cref="AliasAttribute"/> with the given aliases. </summary>
        public AliasAttribute(params string[] aliases)
        {
            Type = AliasType.Relative;
            Aliases = aliases;
        }
        /// <summary> Creates a new <see cref="AliasAttribute"/> with the given aliases. </summary>
        public AliasAttribute(AliasType type, params string[] aliases)
        {
            Type = type;
            Aliases = aliases;
        }
    }
}
