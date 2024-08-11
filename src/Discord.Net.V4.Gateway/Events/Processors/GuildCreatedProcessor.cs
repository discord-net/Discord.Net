using Discord.Gateway.State;
using Discord.Models;

namespace Discord.Gateway.Processors;

[DispatchEvent(DispatchEventNames.GuildCreate)]
public sealed partial class GuildCreatedProcessor(DiscordGatewayClient client) :
    IDispatchProcessor<IGuildCreatePayloadData>
{
    public async ValueTask ProcessAsync(IGuildCreatePayloadData payload, CancellationToken token = default)
    {
        if (payload.Unavailable)
        {
            client.UnavailableGuilds.Add(payload.Id);
            return;
        }

        if (payload is not IExtendedGuild extendedGuild)
            return;

        var guildsBroker = await GatewayGuildActor.GetConfiguredBrokerAsync(client, IPathable.Empty, token);
        await guildsBroker.UpdateAsync(extendedGuild, token);

        var guildCachePath = new CachePathable {GuildIdentity.Of(extendedGuild.Id)};

        var voiceStateBroker = await GatewayVoiceStateActor.GetConfiguredBrokerAsync(client, guildCachePath, token);
        await voiceStateBroker.BatchUpdateAsync(extendedGuild.VoiceStates, token);

        var membersBroker = await GatewayMemberActor.GetConfiguredBrokerAsync(client, guildCachePath, token);
        await membersBroker.BatchUpdateAsync(extendedGuild.Members, token);

        var channelsBroker = await GatewayGuildChannelActor.GetConfiguredBrokerAsync(client, guildCachePath, token);
        await channelsBroker.BatchUpdateAsync(extendedGuild.Channels, token);

        var threadsBroker = await GatewayThreadChannelActor.GetConfiguredBrokerAsync(client, guildCachePath, token);
        await threadsBroker.BatchUpdateAsync(extendedGuild.Threads, token);

        // TODO: presence

        var stageInstancesBroker =
            await GatewayStageInstanceActor.GetConfiguredBrokerAsync(client, guildCachePath, token);
        await stageInstancesBroker.BatchUpdateAsync(extendedGuild.StageInstances, token);

        var guildScheduledEventsBroker =
            await GatewayGuildScheduledEventActor.GetConfiguredBrokerAsync(client, guildCachePath, token);
        await guildScheduledEventsBroker.BatchUpdateAsync(extendedGuild.GuildScheduledEvents, token);
    }
}
