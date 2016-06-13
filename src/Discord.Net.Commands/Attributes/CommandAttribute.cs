using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public string Name { get; }
        public CommandAttribute(string name)
        {
            Name = name;
        }
    }
}
