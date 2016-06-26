using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public string Text { get; }

        public CommandAttribute()
        {
            Text = null;
        }
        public CommandAttribute(string text)
        {
            Text = text;
        }
    }
}
