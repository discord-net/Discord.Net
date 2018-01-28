using System;

namespace Discord.Commands
{
    // Override public name of command/module
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class NameAttribute : Attribute
    {
        public string Text { get; }

        public NameAttribute(string text)
        {
            Text = text;
        }
    }
}
