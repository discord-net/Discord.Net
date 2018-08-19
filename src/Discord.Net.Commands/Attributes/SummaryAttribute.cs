using System;

namespace Discord.Commands
{
    // Cosmetic Summary, for Groups and Commands
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter)]
    public class SummaryAttribute : Attribute
    {
        public SummaryAttribute(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}
