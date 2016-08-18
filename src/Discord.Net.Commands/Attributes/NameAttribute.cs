using System;

namespace Discord.Commands
{
    // Full summary of method
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class NameAttribute : Attribute
    {
        public string Text { get; }
        public NameAttribute(string text)
        {
            Text = text;
        }
    }
}
