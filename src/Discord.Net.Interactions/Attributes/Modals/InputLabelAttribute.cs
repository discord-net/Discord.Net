using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Creates a custom label for an modal input.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class InputLabelAttribute : Attribute
    {
        /// <summary>
        ///     Gets the label of the input.
        /// </summary>
        public string Label { get; }

        /// <summary>
        ///     Creates a custom label for an modal input.
        /// </summary>
        /// <param name="label">The label of the input.</param>
        public InputLabelAttribute(string label)
        {
            Label = label;
        }
    }
}
