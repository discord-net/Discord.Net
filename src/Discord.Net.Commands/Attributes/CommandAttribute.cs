using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute()
        {
            Text = null;
        }

        public CommandAttribute(string text)
        {
            Text = text;
        }

        public string Text { get; }
        public RunMode RunMode { get; set; } = RunMode.Default;
        public bool? IgnoreExtraArgs { get; set; }
    }
}
