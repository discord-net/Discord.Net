using System;

namespace Discord.Commands
{
    // Cosmetic Description for Groups, Commands, and Parameters
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
