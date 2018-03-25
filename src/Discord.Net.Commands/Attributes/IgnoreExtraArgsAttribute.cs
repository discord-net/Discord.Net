using System;

namespace Discord.Commands
{
    /// <summary> Set whether or not to ignore extra arguments for an individual command method or module,
    /// overriding the setting in <see cref="CommandServiceConfig"/> if necessary. </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class IgnoreExtraArgsAttribute : Attribute
    {
        public bool IgnoreValue { get; }

        public IgnoreExtraArgsAttribute(bool ignoreValue)
        {
            IgnoreValue = ignoreValue;
        }
    }
}
