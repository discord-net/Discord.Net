using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record ModifyCurrentUserParams
    {
        /// <summary>
        /// <see cref="User"/>'s username, if changed may cause the <see cref="User"/>'s
        /// discriminator to be randomized.
        /// </summary>
        public Optional<string> Username { get; set; }

        /// <summary>
        /// If passed, modifies the <see cref="User"/>'s avatar.
        /// </summary>
        public Optional<Image?> Avatar { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
        }
    }
}
