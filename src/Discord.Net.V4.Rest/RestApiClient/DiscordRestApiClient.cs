using Discord.Rest.Converters;
using System.Text.Json;

namespace Discord.Rest;

public partial class DiscordRestApiClient : IRestApiProvider
{
    private JsonSerializerOptions _serializerOptions;

    public DiscordRestApiClient()
    {
        _serializerOptions = new JsonSerializerOptions
        {
            Converters = {
                new OptionalConverter(),
                new UInt64Converter(),
                new EmbedTypeConverter(),
                new UserStatusConverter()
            },
        };
    }


    public ValueTask DisposeAsync() => throw new NotImplementedException();

    public void Dispose() => throw new NotImplementedException();

    public Task LoginAsync(TokenType tokenType, string token, bool validateToken, CancellationToken? cancellationToken = null) => throw new NotImplementedException();

    public Task LogoutAsync(CancellationToken? cancellationToken = null) => throw new NotImplementedException();

    public virtual Task<IReadOnlyCollection<IVoiceRegion>> ListVoiceRegionsAsync(CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();
}
