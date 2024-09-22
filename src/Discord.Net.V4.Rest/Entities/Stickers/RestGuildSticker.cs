using Discord.Models;
using Discord.Rest.Extensions;

namespace Discord.Rest;

[ExtendInterfaceDefaults(typeof(IGuildStickerActor))]
public partial class RestGuildStickerActor :
    RestStickerActor,
    IGuildStickerActor,
    IRestActor<RestGuildStickerActor, ulong, RestGuildSticker, IGuildStickerModel>
{
    [SourceOfTruth] public RestGuildActor Guild { get; }

    [SourceOfTruth]
    internal override GuildStickerIdentity Identity { get; }

    [TypeFactory]
    public RestGuildStickerActor(
        DiscordRestClient client,
        GuildIdentity guild,
        GuildStickerIdentity sticker
    ) : base(client, sticker)
    {
        Identity = sticker | this;
        Guild = guild.Actor ?? new(client, guild);
    }

    [SourceOfTruth]
    internal RestGuildSticker CreateEntity(IGuildStickerModel model)
        => RestGuildSticker.Construct(Client, this, model);
}

public sealed partial class RestGuildSticker :
    RestSticker,
    IGuildSticker,
    IRestConstructable<RestGuildSticker, RestGuildStickerActor, IGuildStickerModel>
{
    [SourceOfTruth] public RestMemberActor? Author { get; private set; }

    public bool? IsAvailable => Model.Available;

    internal override IGuildStickerModel Model => _model;

    [ProxyInterface(
        typeof(IGuildStickerActor),
        typeof(IGuildRelationship),
        typeof(IEntityProvider<IGuildSticker, IGuildStickerModel>)
    )]
    internal override RestGuildStickerActor Actor { get; }

    private IGuildStickerModel _model;

    internal RestGuildSticker(
        DiscordRestClient client,
        IGuildStickerModel model,
        RestGuildStickerActor actor
    ) : base(client, model, actor)
    {
        _model = model;
        Actor = actor;

        Author = model.AuthorId.HasValue
            ? actor.Guild.Members[model.AuthorId.Value]
            : null;
    }

    public static RestGuildSticker Construct(
        DiscordRestClient client, 
        RestGuildStickerActor actor, 
        IGuildStickerModel model
        ) => new(client, model, actor);

    [CovariantOverride]
    public ValueTask UpdateAsync(IGuildStickerModel model, CancellationToken token = default)
    {
        Author = Author.UpdateFrom(
            model.AuthorId,
            Actor.Guild.Members.Specifically
        );

        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IGuildStickerModel GetModel() => Model;
}
