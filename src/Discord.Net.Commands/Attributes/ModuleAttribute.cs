using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : Attribute
    {
        public string Prefix { get; }
        public bool AutoLoad { get; set; }
        public bool AppendSpace { get; set; }

        public ModuleAttribute() : this(null) { }

        public ModuleAttribute(string prefix = null)
        {
            Prefix = prefix;
            AutoLoad = true;
            AppendSpace = true;
        }
    }
}
