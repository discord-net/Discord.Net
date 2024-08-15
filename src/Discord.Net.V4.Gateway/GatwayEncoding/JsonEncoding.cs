using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Gateway;

[method: TypeFactory]
public sealed partial class JsonEncoding(DiscordGatewayClient client) : IGatewayEncoding
{
    public DiscordGatewayClient Client { get; } = client;
    public string Identifier => "json";
    public TransportFormat Format => TransportFormat.Text;

    public async ValueTask<T?> DecodeAsync<T>(Stream data, CancellationToken token = default)
        => await JsonSerializer.DeserializeAsync<T>(data, Client.Config.JsonSerializerOptions, token);

    public async ValueTask EncodeAsync<T>(Stream stream, T value, CancellationToken token = default)
        => await JsonSerializer.SerializeAsync(stream, value, Client.Config.JsonSerializerOptions, token);
}
