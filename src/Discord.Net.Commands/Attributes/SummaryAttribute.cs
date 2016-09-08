using System;

namespace Discord.Commands
{
    // Cosmetic Summary, for Groups and Commands
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
