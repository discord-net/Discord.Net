using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record GroupDMAddRecipientParams
    {
        /// <summary>
        /// Access token of a <see cref="User"/> that has granted your app the gdm.join scope.
        /// </summary>
        public string? AccessToken { get; set; } // Required property candidate

        /// <summary>
        /// Nickname of the <see cref="User"/> being added.
        /// </summary>
        public string? Nick { get; set; } // Required property candidate

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotNullOrEmpty(AccessToken, nameof(AccessToken));
        }
    }
}
