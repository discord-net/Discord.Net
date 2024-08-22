using Discord.Models;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestStickerActor :
    RestActor<ulong, RestSticker, StickerIdentity>,
    IStickerActor
{
    internal override StickerIdentity Identity { get; }

    [TypeFactory]
    public RestStickerActor(
        DiscordRestClient client,
        StickerIdentity sticker
    ) : base(client, sticker)
    {
        Identity = sticker | this;
    }

    [SourceOfTruth]
    internal RestSticker CreateEntity(IStickerModel model)
        => RestSticker.Construct(Client, model);
}

public partial class RestSticker :
    RestEntity<ulong>,
    ISticker,
    IConstructable<RestSticker, IStickerModel, DiscordRestClient>,
    IRestConstructable<RestSticker, RestStickerActor, IStickerModel>
{
    public string Name => Model.Name;

    public StickerFormatType Format => (StickerFormatType)Model.FormatType;

    [SourceOfTruth] public RestStickerPackActor? Pack { get; }

    public string? Description => Model.Description;

    public IReadOnlyCollection<string> Tags { get; private set; }

    public StickerType Type => (StickerType)Model.Type;

    public int? SortOrder => Model.SortValue;

    [ProxyInterface] internal virtual RestStickerActor Actor { get; }

    internal virtual IStickerModel Model => _model;

    private IStickerModel _model;

    internal RestSticker(
        DiscordRestClient client,
        IStickerModel model,
        RestStickerActor? actor = null,
        StickerPackIdentity? pack = null
    ) : base(client, model.Id)
    {
        _model = model;
        Tags = model.Tags.Split(',').ToImmutableArray();

        Actor = actor ?? new(client, StickerIdentity.Of(this));

        Pack = pack?.Actor ?? (pack is not null
            ? new RestStickerPackActor(client, pack)
            : model.PackId.Map(
                (id, client) => new RestStickerPackActor(client, StickerPackIdentity.Of(id)),
                client
            ));
    }

    public static RestSticker Construct(DiscordRestClient client, IStickerModel model)
        => Construct(client, null, model);

    public static RestSticker Construct(DiscordRestClient client, StickerPackIdentity? pack, IStickerModel model)
    {
        return model switch
        {
            IGuildStickerModel guildSticker
                => RestGuildSticker.Construct(client, GuildIdentity.Of(guildSticker.GuildId), guildSticker),
            _ => new RestSticker(client, model, pack: pack)
        };
    }

    public virtual ValueTask UpdateAsync(IStickerModel model, CancellationToken token = default)
    {
        if (_model.Tags != model.Tags)
            Tags = Model.Tags.Split(',').ToImmutableArray();

        _model = model;

        return ValueTask.CompletedTask;
    }

    public virtual IStickerModel GetModel() => Model;
}
