using Discord.Models;
using PropertyChanged;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Discord.Rest.Stickers;

public partial class RestSticker :
    RestEntity<ulong>,
    ISticker,
    IConstructable<RestSticker, IStickerModel, DiscordRestClient>
{
    internal IStickerModel Model { get; private set; }

    internal RestSticker(DiscordRestClient client, IStickerModel model) : base(client, model.Id)
    {
        Model = model;
        Tags = model.Tags.Split(',').ToImmutableArray();
    }

    public static RestSticker Construct(DiscordRestClient client, IStickerModel model)
    {
        return model.GuildId.HasValue
            ? RestGuildSticker.Construct(client, model, model.GuildId.Value)
            : new RestSticker(client, model);
    }



    protected virtual void OnModelUpdated()
    {
        if(IsTagsOutOfDate) Tags = Model.Tags.Split(',').ToImmutableArray();
    }

    public string Name => Model.Name;

    public StickerFormatType Format => (StickerFormatType)Model.FormatType;

    public ulong? PackId => Model.PackId;

    public string? Description => Model.Description;

    public IReadOnlyCollection<string> Tags { get; private set; }

    public StickerType Type => (StickerType)Model.Type;

    public bool? IsAvailable => Model.Available;

    public int? SortOrder => Model.SortValue;
}
