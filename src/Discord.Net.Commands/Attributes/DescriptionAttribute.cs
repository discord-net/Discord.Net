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

    [AttributeUsage(AttributeTargets.Method)]
    public class SynopsisAttribute : Attribute
    {
        public string Text { get; }
        public SynopsisAttribute(string text)
        {
            Text = text;
        }
    }
}
