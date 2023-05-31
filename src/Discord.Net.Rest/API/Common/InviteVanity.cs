using Newtonsoft.Json;

namespace Discord.API
{
    /// <summary>
    /// Represents a vanity invite.
    /// </summary>
    internal class InviteVanity
    {
        /// <summary>
        /// The unique code for the invite link.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// The total amount of vanity invite uses.
        /// </summary>
        [JsonProperty("uses")]
        public int Uses { get; set; }
    }
}
