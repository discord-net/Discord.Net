using Discord.Gateway.Events;
using Discord.Gateway.State;
using Discord.Gateway;
using Discord.Models;
using Discord.Models.Dispatch;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Gateway;

public sealed partial class DiscordGatewayClient
{
    internal HashSet<ulong> UnavailableGuilds { get; } = new(2500);

    private readonly IGatewayDispatchQueue _dispatchQueue;

    private async Task HandleDispatchAsync(string type, IGatewayPayloadData? payload, CancellationToken token)
    {
        switch (type)
        {
            case DispatchEventNames.Ready when payload is IReadyPayloadData readyPayload:
                await HandleReadyAsync(readyPayload, token);
                break;
            default:
                if (payload is not null)
                {
                    foreach (var processor in GetProcessors(type))
                        await processor.ProcessAsync(payload, token);
                }

                break;
        }

        await _dispatchQueue.AcceptAsync(type, GetDispatchEvents(type), payload, token);
    }

    private async Task HandleReadyAsync(IReadyPayloadData readyPayloadData, CancellationToken token)
    {
        _sessionId = readyPayloadData.SessionId;
        _resumeGatewayUrl = readyPayloadData.ResumeGatewayUrl;

        if (readyPayloadData.Shard is not null)
        {
            ShardId = readyPayloadData.Shard[0];
            TotalShards = readyPayloadData.Shard[1];
        }

        if (StateController.SelfUserModel is null)
            StateController.SelfUserModel = new(readyPayloadData.User);
        else
        {
            StateController.SelfUserModel.UserModelPart = readyPayloadData.User;
            StateController.SelfUserModel.SelfUserModelPart = readyPayloadData.User;
        }

        var broker = await GatewayCurrentUserActor.GetConfiguredBrokerAsync(this, CachePathable.Empty, token);
        
        await broker.UpdateAsync(StateController.SelfUserModel, token);

        UnavailableGuilds.Clear();
        UnavailableGuilds.UnionWith(readyPayloadData.Guilds.Select(x => x.Id));

        // TODO: application
    }
}
