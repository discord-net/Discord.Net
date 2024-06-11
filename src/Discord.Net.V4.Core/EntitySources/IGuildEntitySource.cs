namespace Discord;

public interface IGuildEntitySource<TGuild> : IClientProvider, ILoadableEntity<ulong, TGuild>, IPathable
    where TGuild : class, IGuild
{
    IRootEntitySource<IMessageChannelEntitySource<ITextChannel>, ulong, ITextChannel> TextChannels { get; }
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
