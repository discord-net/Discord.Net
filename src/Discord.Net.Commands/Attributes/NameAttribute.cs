using System;

namespace Discord.Commands
{
    // Override public name of command/module
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter)]
    public class NameAttribute : Attribute
    {
        public string Text { get; }

        public NameAttribute(string text)
        {
            Text = text;
        }
    }
}
