using System;

namespace Discord.Commands
{
    /// <summary> Marks the execution information for a command. </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// Gets the text that has been set to be recognized as a command.
        /// </summary>
        public string Text { get; }
        /// <summary>
        /// Specifies the <see cref="RunMode"/> of the command. This affects how the command is executed.
        /// </summary>
        public RunMode RunMode { get; set; } = RunMode.Default;

        public CommandAttribute()
        {
            Text = null;
        }
        public CommandAttribute(string text)
        {
            Text = text;
        }
    }
}
