using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Mark an <see cref="IModal"/> property as a modal input field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public abstract class ModalInputAttribute : Attribute
    {
        /// <summary>
        ///     Gets the custom id of the text input.
        /// </summary>
        public string CustomId { get; }

        /// <summary>
        ///     Gets the type of the component.
        /// </summary>
        public abstract ComponentType ComponentType { get; }

        /// <summary>
        ///     Create a new <see cref="ModalInputAttribute"/>.
        /// </summary>
        /// <param name="customId">The custom id of the input.</param>
        protected ModalInputAttribute(string customId)
        {
            CustomId = customId;
        }
    }
}
