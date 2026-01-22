using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Mark an <see cref="IModal"/> property as a modal input field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public abstract class ModalInputAttribute : ModalComponentAttribute
    {
        /// <summary>
        ///     Gets the custom id of the text input.
        /// </summary>
        public string CustomId { get; }

        /// <summary>
        ///     Create a new <see cref="ModalInputAttribute"/>.
        /// </summary>
        /// <param name="customId">The custom id of the input.</param>
        /// <param name="id">Optional identifier for component.</param>
        internal ModalInputAttribute(string customId, int id) : base(id)
        {
            CustomId = customId;
        }
    }
}
