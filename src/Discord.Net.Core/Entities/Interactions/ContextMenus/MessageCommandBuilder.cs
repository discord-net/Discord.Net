namespace Discord
{
    /// <summary>
    ///     A class used to build Message commands.
    /// </summary>
    public class MessageCommandBuilder
    {
        /// <summary>
        ///     Returns the maximum length a commands name allowed by Discord
        /// </summary>
        public const int MaxNameLength = 32;

        /// <summary>
        ///     Gets or sets the name of this Message command.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                Preconditions.NotNullOrEmpty(value, nameof(Name));
                Preconditions.AtLeast(value.Length, 1, nameof(Name));
                Preconditions.AtMost(value.Length, MaxNameLength, nameof(Name));

                _name = value;
            }
        }

        /// <summary>
        ///     Gets or sets whether the command is enabled by default when the app is added to a guild
        /// </summary>
        public bool IsDefaultPermission { get; set; } = true;

        private string _name;

        /// <summary>
        ///     Build the current builder into a <see cref="MessageCommandProperties"/> class.
        /// </summary>
        /// <returns>
        ///     A <see cref="MessageCommandProperties"/> that can be used to create message commands.
        /// </returns>
        public MessageCommandProperties Build()
        {
            var props = new MessageCommandProperties
            {
                Name = Name,
                IsDefaultPermission = IsDefaultPermission
            };

            return props;
        }

        /// <summary>
        ///     Sets the field name.
        /// </summary>
        /// <param name="name">The value to set the field name to.</param>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public MessageCommandBuilder WithName(string name)
        {
            Name = name;
            return this;
        }

        /// <summary>
        ///     Sets the default permission of the current command.
        /// </summary>
        /// <param name="isDefaultPermission">The default permission value to set.</param>
        /// <returns>The current builder.</returns>
        public MessageCommandBuilder WithDefaultPermission(bool isDefaultPermission)
        {
            IsDefaultPermission = isDefaultPermission;
            return this;
        }
    }
}
