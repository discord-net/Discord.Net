using Discord.Models;
using Discord.Models.Json;
using Discord.Models.Json.Stickers;
using Discord.Rest.Extensions;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Rest;

[method: TypeFactory]
[ExtendInterfaceDefaults(typeof(IGuildStickerActor))]
public partial class RestGuildStickerActor(
    DiscordRestClient client,
    GuildIdentity guild,
    GuildStickerIdentity sticker
) :
    RestActor<ulong, RestGuildSticker, GuildStickerIdentity>(client, sticker),
    IGuildStickerActor
{
    [SourceOfTruth]
    public RestGuildActor Guild { get; } = guild.Actor ?? new(client, guild);

    [SourceOfTruth]
    internal RestGuildSticker CreateEntity(IGuildStickerModel model)
        => RestGuildSticker.Construct(Client, Guild.Identity, model);
}

public sealed partial class RestGuildSticker :
    RestSticker,
    IGuildSticker,
    IContextConstructable<RestGuildSticker, IGuildStickerModel, GuildIdentity, DiscordRestClient>
{
    [SourceOfTruth]
    public RestMemberActor? Author { get; private set; }

    public bool? IsAvailable => Model.Available;

    internal override IGuildStickerModel Model => _model;

    [ProxyInterface(
        typeof(IGuildStickerActor),
        typeof(IGuildRelationship),
        typeof(IEntityProvider<IGuildSticker, IGuildStickerModel>)
    )]
    internal RestGuildStickerActor Actor { get; }

    private IGuildStickerModel _model;

    internal RestGuildSticker(
        DiscordRestClient client,
        GuildIdentity guild,
        IGuildStickerModel model,
        RestGuildStickerActor? actor = null
    ) : base(client, model)
    {
        _model = model;

        Actor = actor ?? new(client, guild, GuildStickerIdentity.Of(this));

        Author = model.AuthorId.Map(
            static (id, client, guild, model)
                => new RestMemberActor(
                    client,
                    guild,
                    MemberIdentity.Of(id),
                    UserIdentity.FromReferenced<RestUser, DiscordRestClient>(model, id, client)
                ),
            client,
            guild,
            model
        );
    }

    public static RestGuildSticker Construct(DiscordRestClient client, GuildIdentity guild, IGuildStickerModel model)
        => new(client, guild, model);

    [CovariantOverride]
    public ValueTask UpdateAsync(IGuildStickerModel model, CancellationToken token = default)
    {
        Author = Author.UpdateFrom(
            model.AuthorId,
            RestMemberActor.Factory,
            Client,
            Actor.Guild.Identity,
            model.AuthorId.Map(
                static (id, model, client)
                    => UserIdentity.FromReferenced<RestUser, DiscordRestClient>(model, id, client),
                model,
                Client
            )
        );

        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IGuildStickerModel GetModel() => Model;
}
