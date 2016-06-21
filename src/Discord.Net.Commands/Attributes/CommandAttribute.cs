using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public string Text { get; }
        public string Name { get; }

        public CommandAttribute(string name) : this(name, name) { }
        public CommandAttribute(string text, string name)
        {
            Text = text.ToLowerInvariant();
            Name = name;
        }
    }
}
