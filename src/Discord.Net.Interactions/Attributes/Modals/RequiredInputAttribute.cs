using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Sets the input as required or optional.
    /// </summary>
    /// <remarks>
    ///     Not applicable for checkbox component. See Discord API documentation for further information.
    /// </remarks>
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
