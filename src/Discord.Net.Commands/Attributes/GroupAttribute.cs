using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GroupAttribute : Attribute
    {
        public GroupAttribute()
        {
            Prefix = null;
        }

        public GroupAttribute(string prefix)
        {
            Prefix = prefix;
        }

        public string Prefix { get; }
    }
}
