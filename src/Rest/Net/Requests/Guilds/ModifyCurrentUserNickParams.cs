using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record ModifyCurrentUserNickParams
    {
        /// <summary>
        /// Value to set users nickname to.
        /// </summary>
        public Optional<string?> Nick { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.LengthAtMost(Nick, GuildMember.MaxNicknameLength, nameof(Nick));
        }
    }
}
