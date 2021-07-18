namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record AddGuildMemberParams
    {
        /// <summary>
        /// An oauth2 access token granted with the guilds.join to the bot's application for the user you want to add to the guild.
        /// </summary>
        public string? AccessToken { get; set; } // Required property candidate

        /// <summary>
        /// Value to set users nickname to.
        /// </summary>
        public Optional<string> Nick { get; set; }

        /// <summary>
        /// Array of role ids the member is assigned.
        /// </summary>
        public Optional<Snowflake[]> Roles { get; set; }

        /// <summary>
        /// Whether the user is muted in voice channels.
        /// </summary>
        public Optional<bool> Mute { get; set; }

        /// <summary>
        /// Whether the user is deafened in voice channels.
        /// </summary>
        public Optional<bool> Deaf { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotNullOrEmpty(AccessToken, nameof(AccessToken));
        }
    }
}
