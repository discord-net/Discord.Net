namespace Discord.Interactions
{
    /// <summary>
    ///     Marks a <see cref="IModal"/> property as a text input.
    /// </summary>
    public sealed class ModalTextInputAttribute : ModalInputAttribute
    {
        /// <inheritdoc/>
        public override ComponentType ComponentType => ComponentType.TextInput;

        /// <summary>
        ///     Gets the style of the text input.
        /// </summary>
        public TextInputStyle Style { get; }

        /// <summary>
        ///     Gets the placeholder of the text input.
        /// </summary>
        public string Placeholder { get; }

        /// <summary>
        ///     Gets the minimum length of the text input.
        /// </summary>
        public int MinLength { get; }

        /// <summary>
        ///     Gets the maximum length of the text input.
        /// </summary>
        public int MaxLength { get; }

        /// <summary>
        ///     Gets the initial value to be displayed by this input.
        /// </summary>
        public string InitialValue { get; }

        /// <summary>
        ///     Create a new <see cref="ModalTextInputAttribute"/>.
        /// </summary>
        /// <param name="customId">The custom id of the text input.></param>
        /// <param name="style">The style of the text input.</param>
        /// <param name="placeholder">The placeholder of the text input.</param>
        /// <param name="minLength">The minimum length of the text input's content.</param>
        /// <param name="maxLength">The maximum length of the text input's content.</param>
        /// <param name="initValue">The initial value to be displayed by this input.</param>
        public ModalTextInputAttribute(string customId, TextInputStyle style = TextInputStyle.Short, string placeholder = null, int minLength = 1, int maxLength = 4000, string initValue = null)
            : base(customId)
        {
            Style = style;
            Placeholder = placeholder;
            MinLength = minLength;
            MaxLength = maxLength;
            InitialValue = initValue;
        }
    }
}
