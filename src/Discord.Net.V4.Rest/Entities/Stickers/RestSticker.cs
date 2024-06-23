using Discord.Models;
using PropertyChanged;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Discord.Rest.Stickers;

public partial class RestSticker(DiscordRestClient client, IStickerModel model) :
    RestEntity<ulong>(client, model.Id),
    ISticker,
    IConstructable<RestSticker, IStickerModel, DiscordRestClient>,
    INotifyPropertyChanged
{
    [OnChangedMethod(nameof(OnModelUpdated))]
    internal IStickerModel Model { get; set; } = model;

    public static RestSticker Construct(DiscordRestClient client, IStickerModel model)
    {
        if (model.GuildId.HasValue)
        {
            return RestGuildSticker.Construct(client, model, model.GuildId.Value);
        }

        return new(client, model);
    }

    protected virtual void OnModelUpdated()
    {
        if(IsTagsOutOfDate) Tags = Model.Tags.Split(',').ToImmutableArray();
    }

    public string Name => Model.Name;

    public StickerFormatType Format => (StickerFormatType)Model.FormatType;

    public ulong? PackId => Model.PackId;

    public string? Description => Model.Description;

    [VersionOn(nameof(Model.Tags), nameof(model.Tags))]
    public IReadOnlyCollection<string> Tags { get; private set; } =
        model.Tags.Split(',').ToImmutableArray();

    public StickerType Type => (StickerType)Model.Type;

    public bool? IsAvailable => Model.Available;

    public int? SortOrder => Model.SortValue;
}
