using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : Attribute
    {
        public string Prefix { get; }
        public bool Autoload { get; }
        public ModuleAttribute(bool autoload = true)
        {
            Prefix = null;
            Autoload = autoload;
        }
        public ModuleAttribute(string prefix, bool autoload = true)
        {
            Prefix = prefix;
            Autoload = autoload;
        }
    }
}
