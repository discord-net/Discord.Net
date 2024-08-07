using Discord.Gateway.Events;
using Discord.Gateway.State;
using Discord.Gateway;
using Discord.Models;
using Discord.Models.Dispatch;
using Microsoft.Extensions.Logging;

namespace Discord.Gateway;

public sealed partial class DiscordGatewayClient
{
    private readonly IGatewayDispatchQueue _dispatchQueue;

    private async Task ProcessDispatchAsync(string type, IGatewayPayloadData? payload, CancellationToken token)
    {
        // TODO:
        // we update state first, then invoke the event queue
        switch (type)
        {
            case DispatchEventNames.Ready when payload is IReadyPayloadData readyPayload:
                await HandleReadyAsync(readyPayload, token);
                break;
        }


        await _dispatchQueue.AcceptAsync(type, payload);
    }

    private async Task HandleReadyAsync(IReadyPayloadData readyPayloadData, CancellationToken token)
    {
        if (StateController.SelfUserModel is null)
            StateController.SelfUserModel = new(readyPayloadData.User);
        else
        {
            StateController.SelfUserModel.UserModelPart = readyPayloadData.User;
            StateController.SelfUserModel.SelfUserModelPart = readyPayloadData.User;
        }

        var store = await StateController.GetRootStoreAsync<GatewayUserActor, ulong, IUserModel>(token);
        var broker = await StateController.GetBrokerAsync<ulong, GatewayUser, GatewayUserActor, IUserModel>(token);
        await broker.UpdateAsync(StateController.SelfUserModel, store, token);


        // TODO: shards, resume, session
    }
}
