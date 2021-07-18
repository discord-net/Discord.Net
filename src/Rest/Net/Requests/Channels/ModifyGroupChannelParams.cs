using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to include in a request to modify a <see cref="GroupChannel"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#modify-channel-json-params-group-dm"/>
    /// </remarks>
    public record ModifyGroupChannelParams
    {
        /// <summary>
        /// <see cref="GroupChannel"/> name.
        /// </summary>
        public Optional<string> Name { get; set; }

        /// <summary>
        /// <see cref="GroupChannel"/> icon.
        /// </summary>
        public Optional<Image> Icon { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotNullOrWhitespace(Name!, nameof(Name));
            Preconditions.LengthAtLeast(Name!, Channel.MinChannelNameLength, nameof(Name));
            Preconditions.LengthAtMost(Name!, Channel.MaxChannelNameLength, nameof(Name));
        }
    }
}
