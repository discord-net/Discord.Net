using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record ModifyGuildMemberParams
    {
        /// <summary>
        /// Value to set users nickname to.
        /// </summary>
        public Optional<string?> Nick { get; set; }

        /// <summary>
        /// Array of role ids the member is assigned.
        /// </summary>
        public Optional<Snowflake[]?> Roles { get; set; }

        /// <summary>
        /// Whether the user is muted in voice channels. Will throw a 400 if the user is not in a voice channel.
        /// </summary>
        public Optional<bool?> Mute { get; set; }

        /// <summary>
        /// Whether the user is deafened in voice channels. Will throw a 400 if the user is not in a voice channel.
        /// </summary>
        public Optional<bool?> Deaf { get; set; }

        /// <summary>
        /// Id of channel to move user to (if they are connected to voice).
        /// </summary>
        public Optional<Snowflake?> ChannelId { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.LengthAtMost(Nick, GuildMember.MaxNicknameLength, nameof(Nick));
        }
    }
}
