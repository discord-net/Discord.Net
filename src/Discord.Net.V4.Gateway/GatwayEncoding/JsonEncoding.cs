using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Gateway;

[method: TypeFactory]
public sealed partial class JsonEncoding(DiscordGatewayClient client, DiscordGatewayConfig config) : IGatewayEncoding
{
    public DiscordGatewayClient Client { get; } = client;
    public string Identifier => "json";

    private readonly JsonSerializerOptions _options = config.JsonSerializerOptions;

    public ValueTask<T?> DecodeAsync<T>(Stream data, CancellationToken token = default)
        => JsonSerializer.DeserializeAsync<T>(data, _options, token);

    public async ValueTask EncodeAsync<T>(Stream stream, T value, CancellationToken token = default)
        => await JsonSerializer.SerializeAsync(stream, value, _options, token);
}