using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[ChannelTypeOf(ChannelType.DM)]
public sealed class DMChannelModel : Channel, IDMChannelModel, IModelSource, IModelSourceOf<IUserModel>
{
    [JsonPropertyName("last_message_id")]
    public Optional<ulong?> LastMessageId { get; set; }

    [JsonPropertyName("last_pin_timestamp")]
    public Optional<DateTimeOffset?> LastPinTimestamp { get; set; }

    [JsonPropertyName("flags")]
    public int Flags { get; set; }

    [JsonPropertyName("recipients")]
    public required User[] Recipients { get; set; }

    ulong IDMChannelModel.RecipientId => Recipients[0].Id;
    public IEnumerable<IEntityModel> GetDefinedModels() => Recipients;

    IUserModel IModelSourceOf<IUserModel>.Model => Recipients[0];
}
