namespace Discord
{
    /// <summary>
    ///     A class used to build user commands.
    /// </summary>
    public class UserCommandBuilder
    {
        /// <summary> 
        ///     Returns the maximum length a commands name allowed by Discord.
        /// </summary>
        public const int MaxNameLength = 32;

        /// <summary>
        ///     Gets or sets the name of this User command.
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
        ///     Gets or sets whether the command is enabled by default when the app is added to a guild.
        /// </summary>
        public bool IsDefaultPermission { get; set; } = true;

        /// <summary>
        ///     Gets or sets whether or not this command can be used in DMs.
        /// </summary>
        public bool IsDMEnabled { get; set; } = true;

        /// <summary>
        ///     Gets or sets the default permission required to use this slash command.
        /// </summary>
        public GuildPermission? DefaultMemberPermissions { get; set; }

        private string _name;

        /// <summary>
        ///     Build the current builder into a <see cref="UserCommandProperties"/> class.
        /// </summary>
        /// <returns>A <see cref="UserCommandProperties"/> that can be used to create user commands.</returns>
        public UserCommandProperties Build()
        {
            var props = new UserCommandProperties
            {
                Name = Name,
                IsDefaultPermission = IsDefaultPermission,
                IsDMEnabled = IsDMEnabled,
                DefaultMemberPermissions = DefaultMemberPermissions ?? Optional<GuildPermission>.Unspecified
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
        public UserCommandBuilder WithName(string name)
        {
            Name = name;
            return this;
        }

        /// <summary>
        ///     Sets the default permission of the current command.
        /// </summary>
        /// <param name="isDefaultPermission">The default permission value to set.</param>
        /// <returns>The current builder.</returns>
        public UserCommandBuilder WithDefaultPermission(bool isDefaultPermission)
        {
            IsDefaultPermission = isDefaultPermission;
            return this;
        }

        /// <summary>
        ///     Sets whether or not this command can be used in dms
        /// </summary>
        /// <param name="permission"><see langword="true"/> if the command is available in dms, otherwise <see langword="false"/>.</param>
        /// <returns>The current builder.</returns>
        public UserCommandBuilder WithDMPermission(bool permission)
        {
            IsDMEnabled = permission;
            return this;
        }

        /// <summary>
        ///     Sets the default member permissions required to use this application command.
        /// </summary>
        /// <param name="permissions">The permissions required to use this command.</param>
        /// <returns>The current builder.</returns>
        public UserCommandBuilder WithDefaultMemberPermissions(GuildPermission? permissions)
        {
            DefaultMemberPermissions = permissions;
            return this;
        }
    }
}
