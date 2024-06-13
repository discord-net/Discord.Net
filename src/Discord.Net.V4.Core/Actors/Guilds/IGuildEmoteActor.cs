using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableGuildEmoteActor<TEmote> :
    IGuildEmoteActor<TEmote>,
    ILoadableEntity<ulong, TEmote>
    where TEmote : class, IGuildEmote;

public interface IGuildEmoteActor<out TEmote> :
    IActor<ulong, TEmote>,
    IGuildRelationship,
    IModifiable<ulong, IGuildEmoteActor<TEmote>, EmoteProperties, ModifyEmojiParams>,
    IDeletable<ulong, IGuildEmoteActor<TEmote>>
    where TEmote : IGuildEmote
{
    static ApiBodyRoute<ModifyEmojiParams> IModifiable<ulong, IGuildEmoteActor<TEmote>, EmoteProperties, ModifyEmojiParams>.ModifyRoute(IPathable path, ulong id, ModifyEmojiParams args)
        => Routes.ModifyGuildEmoji(path.Require<IGuild>(), id, args);

    static BasicApiRoute IDeletable<ulong, IGuildEmoteActor<TEmote>>.DeleteRoute(IPathable path, ulong id)
        => Routes.DeleteGuildEmoji(path.Require<IGuild>(), id);
}
