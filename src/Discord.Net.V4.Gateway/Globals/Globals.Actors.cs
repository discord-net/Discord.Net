global using StickerPacks = Discord.Gateway.GatewayEnumerableIndexableActor<
    Discord.Gateway.GatewayStickerPackActor,
    ulong,
    Discord.Gateway.GatewayStickerPack,
    Discord.Rest.RestStickerPack,
    Discord.IStickerPack,
    Discord.Models.IStickerPackModel
>;

global using GuildsPager = Discord.Gateway.GatewayPartialPagedIndexableActor<
    Discord.Gateway.GatewayGuildActor,
    ulong,
    Discord.Gateway.GatewayGuild,
    Discord.Rest.RestPartialGuild,
    Discord.Models.IGuildModel,
    Discord.Models.IPartialGuildModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IPartialGuildModel>,
    Discord.PageUserGuildsParams
>;

global using MembersPager = Discord.Gateway.GatewayPagedIndexableActor<
    Discord.Gateway.GatewayMemberActor,
    ulong,
    Discord.Gateway.GatewayMember,
    Discord.Rest.RestMember,
    Discord.IMember,
    Discord.Models.IMemberModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IMemberModel>,
    Discord.PageGuildMembersParams
>;

global using BansPager = Discord.Gateway.GatewayPagedIndexableActor<
    Discord.Gateway.GatewayBanActor,
    ulong,
    Discord.Gateway.GatewayBan,
    Discord.Rest.RestBan,
    Discord.IBan,
    Discord.Models.IBanModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IBanModel>,
    Discord.PageGuildBansParams
>;

global using PublicArchivedThreadsPager = Discord.Gateway.GatewayPagedIndexableActor<
    Discord.Gateway.GatewayThreadChannelActor,
    ulong,
    Discord.Gateway.GatewayThreadChannel,
    Discord.Rest.RestThreadChannel,
    Discord.IThreadChannel,
    Discord.Models.IThreadChannelModel,
    Discord.Models.Json.ChannelThreads,
    Discord.PagePublicArchivedThreadsParams
>;

global using PrivateArchivedThreadsPager = Discord.Gateway.GatewayPagedIndexableActor<
    Discord.Gateway.GatewayThreadChannelActor,
    ulong,
    Discord.Gateway.GatewayThreadChannel,
    Discord.Rest.RestThreadChannel,
    Discord.IThreadChannel,
    Discord.Models.IThreadChannelModel,
    Discord.Models.Json.ChannelThreads,
    Discord.PagePrivateArchivedThreadsParams
>;

global using JoinedPrivateArchivedThreadsPager = Discord.Gateway.GatewayPagedIndexableActor<
    Discord.Gateway.GatewayThreadChannelActor,
    ulong,
    Discord.Gateway.GatewayThreadChannel,
    Discord.Rest.RestThreadChannel,
    Discord.IThreadChannel,
    Discord.Models.IThreadChannelModel,
    Discord.Models.Json.ChannelThreads,
    Discord.PageJoinedPrivateArchivedThreadsParams
>;

global using GatewayIntegrations = Discord.Gateway.GatewayEnumerableIndexableActor<
    Discord.Gateway.GatewayIntegrationActor,
    ulong,
    Discord.Gateway.GatewayIntegration,
    Discord.Rest.RestIntegration,
    Discord.IIntegration,
    Discord.Models.IIntegrationModel
>;

global using GatewayRoles = Discord.Gateway.GatewayEnumerableIndexableActor<
    Discord.Gateway.GatewayRoleActor,
    ulong,
    Discord.Gateway.GatewayRole,
    Discord.Rest.RestRole,
    Discord.IRole,
    Discord.Models.IRoleModel
>;

global using GatewayGuildEmotes = Discord.Gateway.GatewayEnumerableIndexableActor<
    Discord.Gateway.GatewayGuildEmoteActor,
    ulong,
    Discord.Gateway.GatewayGuildEmote,
    Discord.Rest.RestGuildEmote,
    Discord.IGuildEmote,
    Discord.Models.IGuildEmoteModel
>;

global using GatewayGuildStickers = Discord.Gateway.GatewayEnumerableIndexableActor<
    Discord.Gateway.GatewayGuildStickerActor,
    ulong,
    Discord.Gateway.GatewayGuildSticker,
    Discord.Rest.RestGuildSticker,
    Discord.IGuildSticker,
    Discord.Models.IGuildStickerModel
>;

global using GatewayGuildInvites = Discord.Gateway.GatewayEnumerableIndexableActor<
    Discord.Gateway.GatewayGuildInviteActor,
    string,
    Discord.Gateway.GatewayGuildInvite,
    Discord.Rest.RestGuildInvite,
    Discord.IGuildInvite,
    Discord.Models.IInviteModel
>;

global using GatewayGuildChannelInvites = Discord.Gateway.GatewayEnumerableIndexableActor<
    Discord.Gateway.GatewayGuildChannelInviteActor,
    string,
    Discord.Gateway.GatewayGuildChannelInvite,
    Discord.Rest.RestGuildChannelInvite,
    Discord.IGuildChannelInvite,
    Discord.Models.IInviteModel
>;

global using GatewayGuildScheduledEvents = Discord.Gateway.GatewayEnumerableIndexableActor<
    Discord.Gateway.GatewayGuildScheduledEventActor,
    ulong,
    Discord.Gateway.GatewayGuildScheduledEvent,
    Discord.Rest.RestGuildScheduledEvent,
    Discord.IGuildScheduledEvent,
    Discord.Models.IGuildScheduledEventModel
>;

global using GatewayActiveThreads = Discord.Gateway.GatewayEnumerableIndexableActor<
    Discord.Gateway.GatewayThreadChannelActor,
    ulong,
    Discord.Gateway.GatewayThreadChannel,
    Discord.Rest.RestThreadChannel,
    Discord.IThreadChannel,
    Discord.Models.IThreadChannelModel,
    Discord.Models.Json.ListActiveGuildThreadsResponse
>;

global using GatewayGuildChannels = Discord.Gateway.GatewayEnumerableIndexableActor<
    Discord.Gateway.GatewayGuildChannelActor,
    ulong,
    Discord.Gateway.GatewayGuildChannel,
    Discord.Rest.RestGuildChannel,
    Discord.IGuildChannel,
    Discord.Models.IGuildChannelModel
>;
global using GatewayTextChannels = Discord.Gateway.GatewayEnumerableIndexableActor<
    Discord.Gateway.GatewayTextChannelActor,
    ulong,
    Discord.Gateway.GatewayTextChannel,
    Discord.Rest.RestTextChannel,
    Discord.ITextChannel,
    Discord.Models.IGuildTextChannelModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IGuildChannelModel>
>;
global using GatewayVoiceChannels = Discord.Gateway.GatewayEnumerableIndexableActor<
    Discord.Gateway.GatewayVoiceChannelActor,
    ulong,
    Discord.Gateway.GatewayVoiceChannel,
    Discord.Rest.RestVoiceChannel,
    Discord.IVoiceChannel,
    Discord.Models.IGuildVoiceChannelModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IGuildChannelModel>
>;
global using GatewayCategoryChannels = Discord.Gateway.GatewayEnumerableIndexableActor<
    Discord.Gateway.GatewayCategoryChannelActor,
    ulong,
    Discord.Gateway.GatewayCategoryChannel,
    Discord.Rest.RestCategoryChannel,
    Discord.ICategoryChannel,
    Discord.Models.IGuildCategoryChannelModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IGuildChannelModel>
>;
global using GatewayNewsChannels = Discord.Gateway.GatewayEnumerableIndexableActor<
    Discord.Gateway.GatewayNewsChannelActor,
    ulong,
    Discord.Gateway.GatewayNewsChannel,
    Discord.Rest.RestNewsChannel,
    Discord.INewsChannel,
    Discord.Models.IGuildNewsChannelModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IGuildChannelModel>
>;
global using GatewayThreadChannels = Discord.Gateway.GatewayEnumerableIndexableActor<
    Discord.Gateway.GatewayThreadChannelActor,
    ulong,
    Discord.Gateway.GatewayThreadChannel,
    Discord.Rest.RestThreadChannel,
    Discord.IThreadChannel,
    Discord.Models.IThreadChannelModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IGuildChannelModel>
>;
global using GatewayStageChannels = Discord.Gateway.GatewayEnumerableIndexableActor<
    Discord.Gateway.GatewayStageChannelActor,
    ulong,
    Discord.Gateway.GatewayStageChannel,
    Discord.Rest.RestStageChannel,
    Discord.IStageChannel,
    Discord.Models.IGuildStageChannelModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IGuildChannelModel>
>;
global using GatewayForumChannels = Discord.Gateway.GatewayEnumerableIndexableActor<
    Discord.Gateway.GatewayForumChannelActor,
    ulong,
    Discord.Gateway.GatewayForumChannel,
    Discord.Rest.RestForumChannel,
    Discord.IForumChannel,
    Discord.Models.IGuildForumChannelModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IGuildChannelModel>
>;
global using GatewayMediaChannels = Discord.Gateway.GatewayEnumerableIndexableActor<
    Discord.Gateway.GatewayMediaChannelActor,
    ulong,
    Discord.Gateway.GatewayMediaChannel,
    Discord.Rest.RestMediaChannel,
    Discord.IMediaChannel,
    Discord.Models.IGuildMediaChannelModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IGuildChannelModel>
>;

global using GatewayIntegrationChannels = Discord.Gateway.GatewayEnumerableIndexableActor<
    Discord.Gateway.GatewayIntegrationChannelTrait,
    ulong,
    Discord.Gateway.GatewayIntegration,
    Discord.Rest.RestMediaChannel,
    Discord.IMediaChannel,
    Discord.Models.IGuildMediaChannelModel,
    System.Collections.Generic.IEnumerable<Discord.Models.IGuildChannelModel>
>;

