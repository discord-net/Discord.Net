using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Provides properties that are used to modify an <see cref="Emote" /> with the specified changes.
    /// </summary>
    /// <seealso cref="IGuild.ModifyEmoteAsync"/>
    public class EmoteProperties
    {
        /// <summary>
        ///     Gets or sets the name of the <see cref="Emote"/>.
        /// </summary>
        public Optional<string> Name { get; set; }
        /// <summary>
        ///     Gets or sets the roles that can access this <see cref="Emote"/>.
        /// </summary>
        public Optional<IEnumerable<IRole>> Roles { get; set; }
    }
}
