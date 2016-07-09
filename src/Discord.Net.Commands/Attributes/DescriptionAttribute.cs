using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public class DescriptionAttribute : Attribute
    {
        public string Text { get; }
        public DescriptionAttribute(string text)
        {
            Text = text;
        }
    }
}
