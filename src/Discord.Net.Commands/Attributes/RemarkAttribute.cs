using System;

namespace Discord.Commands
{
    // Extension of the Cosmetic Summary, for Groups, Commands, and Parameters
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RemarkAttribute : Attribute
    {
        public string Text { get; }

        public RemarkAttribute(string text)
        {
            Text = text;
        }
    }
}
