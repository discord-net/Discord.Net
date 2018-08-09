using System;

namespace Discord.Commands
{
    // Cosmetic Summary, for Groups and Commands
    /// <summary>
    ///     An attribute that allows you to attach a summary to your command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class SummaryAttribute : Attribute
    {
        public string Text { get; }

        public SummaryAttribute(string text)
        {
            Text = text;
        }
    }
}
