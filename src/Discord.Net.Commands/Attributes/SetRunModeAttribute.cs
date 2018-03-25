using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class SetRunModeAttribute : Attribute
    {
        public RunMode RunMode { get; }

        public SetRunModeAttribute(RunMode runMode)
        {
            RunMode = runMode;
        }
    }
}
