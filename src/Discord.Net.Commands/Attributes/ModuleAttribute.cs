using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : Attribute
    {
        public string Prefix { get; }
        public bool AutoLoad { get; }
        public bool ForceWhitespace { get; }

        public ModuleAttribute(string prefix = null)
        {
            Prefix = prefix;
            AutoLoad = true;
            ForceWhitespace = true;
        }
    }
}
