using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Activity : IActivityModel, IEntityModelSource
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("url")]
    public Optional<string?> Url { get; set; }

    [JsonPropertyName("created_at")]
    [JsonConverter(typeof(MillisecondEpocConverter))]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("timestamps")]
    public Optional<ActivityTimestamps> Timestamps { get; set; }

    [JsonPropertyName("application_id")]
    public Optional<ulong> ApplicationId { get; set; }

    [JsonPropertyName("details")]
    public Optional<string?> Details { get; set; }

    [JsonPropertyName("state")]
    public Optional<string?> State { get; set; }

    [JsonPropertyName("emoji")]
    public Optional<IEmote?> Emoji { get; set; }

    [JsonPropertyName("party")]
    public Optional<ActivityParty> Party { get; set; }

    [JsonPropertyName("assets")]
    public Optional<ActivityAssets> Assets { get; set; }

    [JsonPropertyName("secrets")]
    public Optional<ActivitySecrets> Secrets { get; set; }

    [JsonPropertyName("instance")]
    public Optional<bool> Instance { get; set; }

    [JsonPropertyName("Flags")]
    public Optional<int> Flags { get; set; }

    [JsonPropertyName("buttons")]
    public Optional<ActivityButton[]> Buttons { get; set; }

    string? IActivityModel.Url => Url;
    string? IActivityModel.Details => Details;
    string? IActivityModel.State => State;
    int? IActivityModel.Flags => Flags;
    IEmojiModel? IActivityModel.Emoji => ~Emoji;

    ulong? IActivityModel.ApplicationId => ApplicationId;

    string? IActivityModel.LargeImage => Assets.Map(v => v.LargeImage);

    string? IActivityModel.LargeText => Assets.Map(v => v.LargeText);

    string? IActivityModel.SmallImage => Assets.Map(v => v.SmallImage);

    string? IActivityModel.SmallText => Assets.Map(v => v.SmallText);

    string? IActivityModel.PartyId => Party.Map(v => v.Id);

    long[]? IActivityModel.PartySize => Party.Map(v => v.Size);

    string? IActivityModel.JoinSecret => Secrets.Map(v => v.Join);

    string? IActivityModel.SpectateSecret => Secrets.Map(v => v.Spectate);

    string? IActivityModel.MatchSecret => Secrets.Map(v => v.Match);

    DateTimeOffset? IActivityModel.TimestampStart => Timestamps.Map(v => v.Start);

    DateTimeOffset? IActivityModel.TimestampEnd => Timestamps.Map(v => v.End);

    public IEnumerable<IEntityModel> GetEntities()
    {
        if (Emoji is {IsSpecified: true, Value: not null})
            yield return Emoji.Value;
    }
}
