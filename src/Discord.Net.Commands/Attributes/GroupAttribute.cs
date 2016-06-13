using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GroupAttribute : Attribute
    {
        public string Name { get; }
        public GroupAttribute(string name)
        {
            Name = name;
        }
    }
}
