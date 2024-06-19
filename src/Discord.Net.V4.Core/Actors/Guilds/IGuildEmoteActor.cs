using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableGuildEmoteActor :
    IGuildEmoteActor,
    ILoadableEntity<ulong, IGuildEmote>;

public interface IGuildEmoteActor :
    IActor<ulong, IGuildEmote>,
    IGuildRelationship,
    IModifiable<ulong, IGuildEmoteActor, EmoteProperties, ModifyEmojiParams>,
    IDeletable<ulong, IGuildEmoteActor>
{
    static ApiBodyRoute<ModifyEmojiParams> IModifiable<ulong, IGuildEmoteActor, EmoteProperties, ModifyEmojiParams>.ModifyRoute(IPathable path, ulong id, ModifyEmojiParams args)
        => Routes.ModifyGuildEmoji(path.Require<IGuild>(), id, args);

    static BasicApiRoute IDeletable<ulong, IGuildEmoteActor>.DeleteRoute(IPathable path, ulong id)
        => Routes.DeleteGuildEmoji(path.Require<IGuild>(), id);
}
