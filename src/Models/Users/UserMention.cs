using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///  Represents a discord user mention, that is a <see cref="User"/> with
    ///  an additional partial <see cref="Member"/>.
    /// </summary>
    public record UserMention : User
    {
        /// <summary>
        /// Additional partial member field.
        /// </summary>
        [JsonPropertyName("member")]
        public Optional<GuildMember> Member { get; init; }
    }
}
