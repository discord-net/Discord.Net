using System.Text.Json.Serialization;

namespace Discord.API
{
    /// <summary>
    /// Represents a vanity invite.
    /// </summary>
    public class InviteVanity
    {
        /// <summary>
        /// The unique code for the invite link.
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// The total amount of vanity invite uses.
        /// </summary>
        [JsonPropertyName("uses")]
        public int Uses { get; set; }
    }
}
