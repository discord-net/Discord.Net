using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableGuildActor<TGuild> : IGuildActor<TGuild>, ILoadableEntity<ulong, TGuild>
    where TGuild : class, IGuild;

public interface IGuildActor<out TGuild> :
    IActor<ulong, TGuild>,
    IModifiable<ulong, IGuildActor<TGuild>, ModifyGuildProperties, ModifyGuildParams>,
    IDeletable<ulong, IGuildActor<TGuild>>
    where TGuild : IGuild
{
    ILoadableRootActor<ILoadableThreadActor<IThreadChannel>, ulong, IThreadChannel> ActiveThreads { get; }
    ILoadableRootActor<ILoadableTextChannelActor<ITextChannel>, ulong, ITextChannel> TextChannels { get; }
    ILoadableTextChannelActor<ITextChannel> TextChannel(ulong id) => TextChannels[id];

    ILoadableRootActor<ILoadableGuildChannelActor<IGuildChannel>, ulong, IGuildChannel> Channels { get; }
    ILoadableGuildChannelActor<IGuildChannel> Channel(ulong id) => Channels[id];

    IPagedLoadableRootActor<ILoadableGuildMemberActor<IGuildMember>, ulong, IGuildMember> Members { get; }
    ILoadableGuildMemberActor<IGuildMember> Member(ulong id) => Members[id];

    ILoadableRootActor<ILoadableGuildEmoteActor<IGuildEmote>, ulong, IGuildEmote> Emotes { get; }
    ILoadableGuildEmoteActor<IGuildEmote> Emote(ulong id) => Emotes[id];

    ILoadableRootActor<ILoadableRoleActor<IRole>, ulong, IRole> Roles { get; }
    ILoadableRoleActor<IRole> Role(ulong id) => Roles[id];

    ILoadableRootActor<ILoadableGuildStickerActor<IGuildSticker>, ulong, IGuildSticker> Stickers { get; }
    ILoadableGuildStickerActor<IGuildSticker> Sticker(ulong id) => Stickers[id];

    ILoadableRootActor<ILoadableGuildScheduledEventActor<IGuildScheduledEvent>, ulong, IGuildScheduledEvent> ScheduledEvents { get; }
    ILoadableGuildScheduledEventActor<IGuildScheduledEvent> ScheduledEvent(ulong id) => ScheduledEvents[id];


    static BasicApiRoute IDeletable<ulong, IGuildActor<TGuild>>
        .DeleteRoute(IPathable path, ulong id) => Routes.DeleteGuild(id);

    static ApiBodyRoute<ModifyGuildParams> IModifiable<ulong, IGuildActor<TGuild>, ModifyGuildProperties, ModifyGuildParams>
        .ModifyRoute(IPathable path, ulong id, ModifyGuildParams args) => Routes.ModifyGuild(id, args);
}
