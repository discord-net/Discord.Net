using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : Attribute
    {
        public string Prefix { get; }
        public bool AutoLoad { get; }

        public ModuleAttribute()
        {
            Prefix = null;
            AutoLoad = true;
        }
        public ModuleAttribute(string prefix)
        {
            Prefix = prefix;
            AutoLoad = true;
        }
    }
}
