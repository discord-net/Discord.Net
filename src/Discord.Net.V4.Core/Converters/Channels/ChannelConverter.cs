using Discord.API;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Converters;

public class ChannelConverter : JsonConverter<Channel>
{
    private static readonly Dictionary<ChannelType, Type> _channels;

    static ChannelConverter()
    {
        _channels = typeof(Channel).Assembly.GetTypes()
            .Where(x =>
                x.IsClass && x.IsAssignableTo(typeof(Channel)) && x != typeof(Channel) &&
                x.GetCustomAttribute<ChannelTypeOfAttribute>() is not null)
            .ToDictionary(x => x.GetCustomAttribute<ChannelTypeOfAttribute>()!.Type, x => x);
    }

    public override Channel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var channelObject = JsonDocument.ParseValue(ref reader).RootElement;

        if (!channelObject.TryGetProperty("type", out var channelTypeProp))
            throw new JsonException("Required property 'type' was not present in the provided json object");

        var channelType = (ChannelType)channelTypeProp.GetInt32();

        if (_channels.TryGetValue(channelType, out var type))
            return (Channel?)channelObject.Deserialize(type, options);

        // unknown channel type
        if (!channelObject.TryGetProperty("id", out var idProp))
            throw new JsonException("Required property 'id' was not present in the provided json object");

        return new Channel() {Id = idProp.GetUInt64(), Type = channelType};
    }

    public override void Write(Utf8JsonWriter writer, Channel value, JsonSerializerOptions options)
        => throw new NotSupportedException();
}
