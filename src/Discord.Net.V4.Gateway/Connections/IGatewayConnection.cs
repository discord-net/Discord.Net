using Discord.API.Gateway;
using Discord.API.Rest;
using System;
namespace Discord.Gateway
{
    public interface IGatewayConnection
    {
        Task ConnectAsync(
            Uri url,
            CancellationToken token = default);

        Task DisconnectAsync(CancellationToken token = default);

        ValueTask SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken token = default);
        ValueTask<Stream> ReadAsync(CancellationToken token = default);
    }
}

