global using Webhooks = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayWebhookActor,
    ulong,
    Discord.Gateway.GatewayWebhook,
    Discord.Rest.RestWebhook,
    Discord.IWebhook,
    Discord.Models.IWebhookModel
>;

global using IncomingWebhooks = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayIncomingWebhookActor,
    ulong,
    Discord.Gateway.GatewayIncomingWebhook,
    Discord.Rest.RestIncomingWebhook,
    Discord.IIncomingWebhook,
    Discord.Models.IWebhookModel
>;

global using ChannelFollowerWebhooks = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayChannelFollowerWebhookActor,
    ulong,
    Discord.Gateway.GatewayChannelFollowerWebhook,
    Discord.Rest.RestChannelFollowerWebhook,
    Discord.IChannelFollowerWebhook,
    Discord.Models.IWebhookModel
>;

global using StickerPacks = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayStickerPackActor,
    ulong,
    Discord.Gateway.GatewayStickerPack,
    Discord.Rest.RestStickerPack,
    Discord.IStickerPack,
    Discord.Models.IStickerPackModel
>;

global using GuildsPager = Discord.Gateway.GatewayPartialPagedIndexableLink<
    Discord.Gateway.GatewayGuildActor,
    ulong,
    Discord.Gateway.GatewayGuild,
    Discord.Rest.RestPartialGuild,
    Discord.Models.IGuildModel,
    Discord.Models.IPartialGuildModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IPartialGuildModel>,
    Discord.PageUserGuildsParams
>;

global using MembersPager = Discord.Gateway.GatewayPagedIndexableLink<
    Discord.Gateway.GatewayMemberActor,
    ulong,
    Discord.Gateway.GatewayMember,
    Discord.Rest.RestMember,
    Discord.IMember,
    Discord.Models.IMemberModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IMemberModel>,
    Discord.PageGuildMembersParams
>;

global using BansPager = Discord.Gateway.GatewayPagedIndexableLink<
    Discord.Gateway.GatewayBanActor,
    ulong,
    Discord.Gateway.GatewayBan,
    Discord.Rest.RestBan,
    Discord.IBan,
    Discord.Models.IBanModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IBanModel>,
    Discord.PageGuildBansParams
>;

global using PublicArchivedThreadsPager = Discord.Gateway.GatewayPagedIndexableLink<
    Discord.Gateway.GatewayThreadChannelActor,
    ulong,
    Discord.Gateway.GatewayThreadChannel,
    Discord.Rest.RestThreadChannel,
    Discord.IThreadChannel,
    Discord.Models.IThreadChannelModel,
    Discord.Models.Json.ChannelThreads,
    Discord.PagePublicArchivedThreadsParams
>;

global using PrivateArchivedThreadsPager = Discord.Gateway.GatewayPagedIndexableLink<
    Discord.Gateway.GatewayThreadChannelActor,
    ulong,
    Discord.Gateway.GatewayThreadChannel,
    Discord.Rest.RestThreadChannel,
    Discord.IThreadChannel,
    Discord.Models.IThreadChannelModel,
    Discord.Models.Json.ChannelThreads,
    Discord.PagePrivateArchivedThreadsParams
>;

global using JoinedPrivateArchivedThreadsPager = Discord.Gateway.GatewayPagedIndexableLink<
    Discord.Gateway.GatewayThreadChannelActor,
    ulong,
    Discord.Gateway.GatewayThreadChannel,
    Discord.Rest.RestThreadChannel,
    Discord.IThreadChannel,
    Discord.Models.IThreadChannelModel,
    Discord.Models.Json.ChannelThreads,
    Discord.PageJoinedPrivateArchivedThreadsParams
>;

global using GatewayIntegrations = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayIntegrationActor,
    ulong,
    Discord.Gateway.GatewayIntegration,
    Discord.Rest.RestIntegration,
    Discord.IIntegration,
    Discord.Models.IIntegrationModel
>;

global using GatewayRoles = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayRoleActor,
    ulong,
    Discord.Gateway.GatewayRole,
    Discord.Rest.RestRole,
    Discord.IRole,
    Discord.Models.IRoleModel
>;

global using GatewayGuildEmotes = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayGuildEmoteActor,
    ulong,
    Discord.Gateway.GatewayGuildEmote,
    Discord.Rest.RestGuildEmote,
    Discord.IGuildEmote,
    Discord.Models.IGuildEmoteModel
>;

global using GatewayGuildStickers = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayGuildStickerActor,
    ulong,
    Discord.Gateway.GatewayGuildSticker,
    Discord.Rest.RestGuildSticker,
    Discord.IGuildSticker,
    Discord.Models.IGuildStickerModel
>;

global using GatewayGuildInvites = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayGuildInviteActor,
    string,
    Discord.Gateway.GatewayGuildInvite,
    Discord.Rest.RestGuildInvite,
    Discord.IGuildInvite,
    Discord.Models.IInviteModel
>;

global using GatewayGuildChannelInvites = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayGuildChannelInviteActor,
    string,
    Discord.Gateway.GatewayGuildChannelInvite,
    Discord.Rest.RestGuildChannelInvite,
    Discord.IGuildChannelInvite,
    Discord.Models.IInviteModel
>;

global using GatewayGuildScheduledEvents = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayGuildScheduledEventActor,
    ulong,
    Discord.Gateway.GatewayGuildScheduledEvent,
    Discord.Rest.RestGuildScheduledEvent,
    Discord.IGuildScheduledEvent,
    Discord.Models.IGuildScheduledEventModel
>;

global using GatewayActiveThreads = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayThreadChannelActor,
    ulong,
    Discord.Gateway.GatewayThreadChannel,
    Discord.Rest.RestThreadChannel,
    Discord.IThreadChannel,
    Discord.Models.IThreadChannelModel,
    Discord.Models.Json.ListActiveGuildThreadsResponse
>;

global using GatewayGuildChannels = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayGuildChannelActor,
    ulong,
    Discord.Gateway.GatewayGuildChannel,
    Discord.Rest.RestGuildChannel,
    Discord.IGuildChannel,
    Discord.Models.IGuildChannelModel
>;
global using GatewayTextChannels = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayTextChannelActor,
    ulong,
    Discord.Gateway.GatewayTextChannel,
    Discord.Rest.RestTextChannel,
    Discord.ITextChannel,
    Discord.Models.IGuildTextChannelModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IGuildChannelModel>
>;
global using GatewayVoiceChannels = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayVoiceChannelActor,
    ulong,
    Discord.Gateway.GatewayVoiceChannel,
    Discord.Rest.RestVoiceChannel,
    Discord.IVoiceChannel,
    Discord.Models.IGuildVoiceChannelModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IGuildChannelModel>
>;
global using GatewayCategoryChannels = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayCategoryChannelActor,
    ulong,
    Discord.Gateway.GatewayCategoryChannel,
    Discord.Rest.RestCategoryChannel,
    Discord.ICategoryChannel,
    Discord.Models.IGuildCategoryChannelModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IGuildChannelModel>
>;
global using GatewayNewsChannels = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayNewsChannelActor,
    ulong,
    Discord.Gateway.GatewayNewsChannel,
    Discord.Rest.RestNewsChannel,
    Discord.INewsChannel,
    Discord.Models.IGuildNewsChannelModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IGuildChannelModel>
>;
global using GatewayThreadChannels = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayThreadChannelActor,
    ulong,
    Discord.Gateway.GatewayThreadChannel,
    Discord.Rest.RestThreadChannel,
    Discord.IThreadChannel,
    Discord.Models.IThreadChannelModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IGuildChannelModel>
>;
global using GatewayStageChannels = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayStageChannelActor,
    ulong,
    Discord.Gateway.GatewayStageChannel,
    Discord.Rest.RestStageChannel,
    Discord.IStageChannel,
    Discord.Models.IGuildStageChannelModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IGuildChannelModel>
>;
global using GatewayForumChannels = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayForumChannelActor,
    ulong,
    Discord.Gateway.GatewayForumChannel,
    Discord.Rest.RestForumChannel,
    Discord.IForumChannel,
    Discord.Models.IGuildForumChannelModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IGuildChannelModel>
>;
global using GatewayMediaChannels = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayMediaChannelActor,
    ulong,
    Discord.Gateway.GatewayMediaChannel,
    Discord.Rest.RestMediaChannel,
    Discord.IMediaChannel,
    Discord.Models.IGuildMediaChannelModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IGuildChannelModel>
>;

global using GatewayIntegrationChannels = Discord.Gateway.GatewayEnumerableIndexableLink<
    Discord.Gateway.GatewayIntegrationChannelTrait,
    ulong,
    Discord.Gateway.GatewayGuildChannel,
    Discord.Rest.RestGuildChannel,
    Discord.IIntegrationChannel,
    Discord.Models.IGuildChannelModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IGuildChannelModel>
>;

