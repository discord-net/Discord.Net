namespace Discord;

public interface ILoadableGuildEntitySource<TGuild> : IGuildEntitySource<TGuild>, ILoadableEntity<ulong, TGuild>
    where TGuild : class, IGuild;

public interface IGuildEntitySource<out TGuild> :
    IEntitySource<ulong, TGuild>
    where TGuild : IGuild
{
    IRootEntitySource<ILoadableTextChannelEntitySource<ITextChannel>, ulong, ITextChannel> TextChannels { get; }
    ILoadableTextChannelEntitySource<ITextChannel> TextChannel(ulong id) => TextChannels[id];

    IRootEntitySource<ILoadableEntity<ulong, IGuildChannel>, ulong, IGuildChannel> Channels { get; }
    ILoadableEntity<ulong, IGuildChannel> Channel(ulong id) => Channels[id];

    IRootEntitySource<ILoadableGuildMemberEntitySource<IGuildUser>, ulong, IGuildUser> Members { get; }
    ILoadableGuildMemberEntitySource<IGuildUser> Member(ulong id) => Members[id];

    IRootEntitySource<ILoadableEntity<ulong, IGuildEmote>, ulong, IGuildEmote> Emotes { get; }
    ILoadableEntity<ulong, IGuildEmote> Emote(ulong id) => Emotes[id];

    IRootEntitySource<ILoadableEntity<ulong, IRole>, ulong, IRole> Roles { get; }
    ILoadableEntity<ulong, IRole> Role(ulong id) => Roles[id];

    IRootEntitySource<ILoadableEntity<ulong, IGuildSticker>, ulong, IGuildSticker> Stickers { get; }

    ILoadableEntity<ulong, IGuildSticker> Sticker(ulong id) => Stickers[id];

    IRootEntitySource<ILoadableEntity<ulong, IGuildScheduledEvent>, ulong, IGuildScheduledEvent> ScheduledEvents { get; }
    ILoadableEntity<ulong, IGuildScheduledEvent> ScheduledEvent(ulong id) => ScheduledEvents[id];
}
