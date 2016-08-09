using System;

namespace Discord.Commands
{
    // Full summary of method
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class DescriptionAttribute : Attribute
    {
        public string Text { get; }
        public DescriptionAttribute(string text)
        {
            Text = text;
        }
    }

    // Brief summary of method/module/parameter
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter)]
    public class SummaryAttribute : Attribute
    {
        public string Text { get; }
        public SummaryAttribute(string text)
        {
            Text = text;
        }
    }
}
