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

        var guildsBroker = await Brokers.Guild.GetConfiguredBrokerAsync(client, token: token);
        await guildsBroker.UpdateAsync(extendedGuild, token);

        using var guildCachePath = new CachePathable();
        guildCachePath.Add(GuildIdentity.Of(extendedGuild.Id));

        var voiceStateBroker = await Brokers.VoiceState.GetConfiguredBrokerAsync(client, guildCachePath, token);
        await voiceStateBroker.BatchUpdateAsync(extendedGuild.VoiceStates, token);

        var membersBroker = await Brokers.Member.GetConfiguredBrokerAsync(client, guildCachePath, token);
        await membersBroker.BatchUpdateAsync(extendedGuild.Members, token);

        var channelsBroker = await Brokers.GuildChannel.GetConfiguredBrokerAsync(client, guildCachePath, token);
        await channelsBroker.BatchUpdateAsync(extendedGuild.Channels, token);

        var threadsBroker = await Brokers.ThreadChannel.GetConfiguredBrokerAsync(client, guildCachePath, token);
        await threadsBroker.BatchUpdateAsync(extendedGuild.Threads, token);

        // TODO: presence

        var stageInstancesBroker = Brokers.StageInstance.GetBroker(client);
        // we have to iterate 'extendedGuild.StageInstances' since we need to include the channel in the path
        // when fetching the store.
        foreach (var stageInstance in extendedGuild.StageInstances)
        {
            using var path = new CachePathable();
            
            path.Add(GuildIdentity.Of(extendedGuild.Id));
            path.Add(StageChannelIdentity.Of(stageInstance.ChannelId));
            
            var store = await Stores.StageInstance.GetStoreInfoAsync(client, path, token);
            await stageInstancesBroker.UpdateAsync(stageInstance, store, token);
        }

        var guildScheduledEventsBroker =
            await Brokers.GuildScheduledEvent.GetConfiguredBrokerAsync(client, guildCachePath, token);
        await guildScheduledEventsBroker.BatchUpdateAsync(extendedGuild.GuildScheduledEvents, token);
    }
}
