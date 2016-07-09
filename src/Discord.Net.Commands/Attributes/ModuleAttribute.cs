using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : Attribute
    {
        public string Prefix { get; }
        public ModuleAttribute()
        {
            Prefix = null;
        }
        public ModuleAttribute(string prefix)
        {
            Prefix = prefix;
        }
    }
}
