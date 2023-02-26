using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Sets the input as required or optional.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequiredInputAttribute : Attribute
    {
        /// <summary>
        ///     Gets whether or not user input is required for this input.
        /// </summary>
        public bool IsRequired { get; }

        /// <summary>
        ///     Sets the input as required or optional.
        /// </summary>
        /// <param name="isRequired">Whether or not user input is required for this input.</param>
        public RequiredInputAttribute(bool isRequired = true)
        {
            IsRequired = isRequired;
        }
    }
}
