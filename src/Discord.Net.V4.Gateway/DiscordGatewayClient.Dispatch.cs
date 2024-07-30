using Discord.Gateway.Events;
using Discord.Gateway.State;
using Discord.Gateway.Users;
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
            case DispatchEventNames.Ready when payload is IReadyPayload readyPayload:
                await HandleReadyAsync(readyPayload, token);
                break;
        }


        await _dispatchQueue.AcceptAsync(type, payload);
    }

    private async Task HandleReadyAsync(IReadyPayload readyPayload, CancellationToken token)
    {
        if (StateController.SelfUserModel is null)
            StateController.SelfUserModel = new(readyPayload.User);
        else
        {
            StateController.SelfUserModel.UserModelPart = readyPayload.User;
            StateController.SelfUserModel.SelfUserModelPart = readyPayload.User;
        }

        var store = await StateController.GetStoreAsync(Template.Of<UserIdentity>(), token);
        using var broker = await StateController.GetBrokerAsync<ulong, GatewayUser, IUserModel>(token);
        await broker.Value.UpdateAsync(StateController.SelfUserModel, store, token);


        // TODO: shards, resume, session
    }
}
