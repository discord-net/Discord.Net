using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : Attribute
    {
        public string Prefix { get; }
        public bool AutoLoad { get; set; }
        public bool ForceWhitespace { get; set; }

        public ModuleAttribute(string prefix = null)
        {
            Prefix = prefix;
            AutoLoad = true;
            ForceWhitespace = true;
        }
    }
}
