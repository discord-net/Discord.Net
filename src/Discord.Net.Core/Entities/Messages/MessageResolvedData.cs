using System.Collections.Generic;

namespace Discord;

public class MessageResolvedData
{
    /// <summary>
    ///     Gets a collection of <see cref="IUser"/> resolved in the message.
    /// </summary>
    public IReadOnlyCollection<IUser> Users { get; }

    /// <summary>
    ///     Gets a collection of <see cref="IGuildUser"/> resolved in the message.
    /// </summary>
    public IReadOnlyCollection<IGuildUser> Members { get; }

    /// <summary>
    ///     Gets a collection of <see cref="IRole"/> resolved in the message.
    /// </summary>
    public IReadOnlyCollection<IRole> Roles { get; }

    /// <summary>
    ///     Gets a collection of <see cref="IChannel"/> resolved in the message.
    /// </summary>
    public IReadOnlyCollection<IChannel> Channels { get; }

    internal MessageResolvedData(IReadOnlyCollection<IUser> users, IReadOnlyCollection<IGuildUser> members, IReadOnlyCollection<IRole> roles, IReadOnlyCollection<IChannel> channels)
    {
        Users = users;
        Members = members;
        Roles = roles;
        Channels = channels;
    }
}
