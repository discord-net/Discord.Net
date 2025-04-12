using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the <see cref="InputComponentInfo"/> class for <see cref="ComponentType.TextInput"/> type.
    /// </summary>
    public class TextInputComponentInfo : InputComponentInfo
    {
        /// <summary>
        /// <c>true</c> when <see cref="InputComponentInfo.Type"/> overrides <see cref="object.ToString"/>.
        /// </summary>
        internal bool TypeOverridesToString => _typeOverridesToString.Value;
        private readonly Lazy<bool> _typeOverridesToString;

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

        internal TextInputComponentInfo(Builders.TextInputComponentBuilder builder, ModalInfo modal) : base(builder, modal)
        {
            Style = builder.Style;
            Placeholder = builder.Placeholder;
            MinLength = builder.MinLength;
            MaxLength = builder.MaxLength;
            InitialValue = builder.InitialValue;

            _typeOverridesToString = new(() => ReflectionUtils<object>.OverridesToString(Type));
        }
    }
}
