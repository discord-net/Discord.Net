using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Marks a <see cref="IModal"/> property as a text input.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ModalTextInputAttribute : ModalInputAttribute
    {
        /// <inheritdoc/>
        public override ComponentType ComponentType => ComponentType.TextInput;

        /// <summary>
        ///     Gets the style of the text input.
        /// </summary>
        public TextInputStyle Style { get; set; }

        /// <summary>
        ///     Gets the placeholder of the text input.
        /// </summary>
        public string Placeholder { get; set; }

        /// <summary>
        ///     Gets the minimum length of the text input.
        /// </summary>
        public int MinLength { get; set; }

        /// <summary>
        ///     Gets the maximum length of the text input.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        ///     Gets the initial value to be displayed by this input.
        /// </summary>
        public string InitialValue { get; set; }

        /// <summary>
        ///     Create a new <see cref="ModalTextInputAttribute"/>.
        /// </summary>
        /// <param name="customId">The custom id of the text input.></param>
        /// <param name="style">The style of the text input.</param>
        /// <param name="placeholder">The placeholder of the text input.</param>
        /// <param name="minLength">The minimum length of the text input's content.</param>
        /// <param name="maxLength">The maximum length of the text input's content.</param>
        /// <param name="initValue">The initial value to be displayed by this input.</param>
        /// <param name="id">The optional identifier for the component.</param>
        public ModalTextInputAttribute(string customId, TextInputStyle style = TextInputStyle.Short, string placeholder = null, int minLength = 1, int maxLength = 4000, string initValue = null, int id = 0)
            : base(customId, id)
        {
            Style = style;
            Placeholder = placeholder;
            MinLength = minLength;
            MaxLength = maxLength;
            InitialValue = initValue;
        }
    }
}
