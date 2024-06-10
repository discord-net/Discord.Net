namespace Discord;

public interface IGuildsEntitySource<TGuild> : ILoadableEntity<ulong, TGuild>, IClientProvider, IAsyncEnumerable<TGuild>
    where TGuild : class, IGuild
{
    IRootEntitySource<ILoadableEntity<ulong, IGuildChannel>, ulong, IGuildChannel> Channels { get; }
    ILoadableEntity<ulong, IGuildChannel> Channel(ulong id) => Channels[id];

    IRootEntitySource<ILoadableEntity<ulong, IGuildUser>, ulong, IGuildUser> Members { get; }
    ILoadableEntity<ulong, IGuildUser> Member(ulong id) => Members[id];

    IRootEntitySource<ILoadableEntity<ulong, IGuildEmote>, ulong, IGuildEmote> Emotes { get; }
    ILoadableEntity<ulong, IGuildEmote> Emote(ulong id) => Emotes[id];

    IRootEntitySource<ILoadableEntity<ulong, IRole>, ulong, IRole> Roles { get; }
    ILoadableEntity<ulong, IRole> Role(ulong id) => Roles[id];

    IRootEntitySource<ILoadableEntity<ulong, IGuildScheduledEvent>, ulong, IGuildScheduledEvent> ScheduledEvents { get; }
    ILoadableEntity<ulong, IGuildScheduledEvent> ScheduledEvent(ulong id) => ScheduledEvents[id];
}
