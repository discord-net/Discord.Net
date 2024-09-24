using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), ChannelType.GroupDM)]
public sealed class GroupDMChannel : Channel, IGroupDMChannelModel, IModelSourceOfMultiple<IUserModel>
{
    [JsonPropertyName("last_message_id")]
    public Optional<ulong?> LastMessageId { get; set; }

    [JsonPropertyName("last_pin_timestamp")]
    public Optional<DateTimeOffset?> LastPinTimestamp { get; set; }

    [JsonPropertyName("flags")]
    public Optional<int> Flags { get; set; }

    [JsonPropertyName("recipients")]
    public required User[] Recipients { get; set; }

    [JsonPropertyName("icon")]
    public Optional<string?> Icon { get; set; }

    [JsonPropertyName("name")]
    public Optional<string?> Name { get; set; }

    [JsonPropertyName("owner_id")]
    public Optional<ulong> OwnerId { get; set; }

    [JsonPropertyName("managed")]
    public Optional<bool> Managed { get; set; }

    [JsonPropertyName("application_id")]
    public Optional<ulong> ApplicationId { get; set; }

    IEnumerable<ulong> IGroupDMChannelModel.Recipients => Recipients.Select(x => x.Id);

    IEnumerable<IUserModel> IModelSourceOfMultiple<IUserModel>.GetModels() => Recipients;

    public IEnumerable<IModel> GetDefinedModels() => Recipients;
}
