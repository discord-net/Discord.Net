using System;

namespace Discord.Commands
{
    // Extension of the Cosmetic Summary, for Groups, Commands, and Parameters
    /// <summary>
    ///     An attribute that allows you to attach remarks to your command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RemarksAttribute : Attribute
    {
        public string Text { get; }

        public RemarksAttribute(string text)
        {
            Text = text;
        }
    }
}
