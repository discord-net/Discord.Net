using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GroupAttribute : Attribute
    {
        public string Prefix { get; }

        public GroupAttribute()
        {
            Prefix = null;
        }
        public GroupAttribute(string prefix)
        {
            Prefix = prefix;
        }
    }
}
