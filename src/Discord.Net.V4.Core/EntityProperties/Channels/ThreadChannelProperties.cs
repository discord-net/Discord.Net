/* Unmerged change from project 'Discord.Net.V4.Core (net7.0)'
Before:
namespace Discord;
After:
using Discord;
using Discord.EntityProperties.Channels;

namespace Discord;
*/

namespace Discord.EntityProperties.Channels;

/// <summary>
///     Provides properties that are used to modify an <see cref="IThreadChannel"/> with the specified changes.
/// </summary>
/// <seealso cref="IThreadChannel.ModifyAsync(Action{ThreadChannelProperties}, RequestOptions)"/>
public class ThreadChannelProperties : TextChannelProperties
{
    /// <summary>
    /// Gets or sets the tags applied to a forum thread
    /// </summary>
    public Optional<IEnumerable<ulong>> AppliedTags { get; set; }

    /// <summary>
    ///     Gets or sets whether or not the thread is locked.
    /// </summary>
    public Optional<bool> Locked { get; set; }

    /// <summary>
    ///     Gets or sets whether or not the thread is archived.
    /// </summary>
    public Optional<bool> Archived { get; set; }
}
