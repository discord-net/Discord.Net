using Discord.Gateway;
using Discord.Models;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Gateway;

using GuildsPager = GatewayPagedIndexableActor<GatewayGuildActor, ulong, GatewayGuild, RestPartialGuild, PageUserGuildsParams, IEnumerable<IPartialGuildModel>>;

public partial class DiscordGatewayClient
{
    [SourceOfTruth]
    public GuildsPager Guilds { get; }

    [SourceOfTruth]
    public GatewayIndexableActor<GatewayChannelActor, ulong, GatewayChannel> Channels { get; }

    [SourceOfTruth]
    public GatewayIndexableActor<GatewayUserActor, ulong, GatewayUser> Users { get; }

    public IIndexableActor<IWebhookActor, ulong, IWebhook> Webhooks => throw new NotImplementedException();

}
