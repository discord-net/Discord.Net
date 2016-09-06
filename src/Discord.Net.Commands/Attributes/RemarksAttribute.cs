using System;

namespace Discord.Commands
{
    // Extension of the Cosmetic Summary, for Groups, Commands, and Parameters
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RemarksAttribute : Attribute
    {
        public string Text { get; }

        public RemarksAttribute(string text)
        {
            Text = text;
        }
    }
}
