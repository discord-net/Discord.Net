global using Discord.Models;

global using GuildCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayGuild, Discord.Rest.RestGuild, Discord.IGuild>;
global using GuildUserCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayGuildMember, Discord.Rest.RestGuildUser, Discord.IGuildMember>;
global using UserCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayUser, Discord.Rest.RestUser, Discord.IUser>;
global using MessageCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayMessage, Discord.Rest.RestMessage, Discord.IMessage>;
global using GuildChannelCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayGuildChannel, Discord.Rest.RestGuildChannel, Discord.IGuildChannel>;
global using GuildStickerCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayCustomSticker, Discord.Rest.CustomSticker, Discord.ICustomSticker>;
global using GuildRoleCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayRole, Discord.Rest.RestRole, Discord.IRole>;
global using GuildMemberCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayGuildMember, Discord.Rest.RestGuildUser, Discord.IGuildMember>;
global using GuildEmoteCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayGuildEmote>;
global using GuildEventCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayGuildEvent, Discord.Rest.RestGuildEvent, Discord.IGuildScheduledEvent>;
global using GuildStageInstanceCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayStageInstance, Discord.Rest.RestStageInstance, Discord.IStageInstance>;
global using MessageChannelCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.IGatewayMessageChannel, Discord.Rest.IRestMessageChannel, Discord.IMessageChannel>;
global using ThreadChannelCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketThreadChannel, Discord.Rest.RestThreadChannel, Discord.IThreadChannel>;
global using GuildCategoryChannelCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayCategoryChannel, Discord.Rest.RestCategoryChannel, Discord.ICategoryChannel>;

global using UsersCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayUser, Discord.Rest.RestUser, Discord.IUser>,
    ulong, Discord.Gateway.GatewayUser, Discord.IUser>;

global using GuildStageInstancesCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayStageInstance, Discord.Rest.RestStageInstance, Discord.IStageInstance>,
    ulong, Discord.Gateway.GatewayStageInstance, Discord.IStageInstance>;

global using GuildEventsCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayGuildEvent, Discord.Rest.RestGuildEvent, Discord.IGuildScheduledEvent>,
    ulong, Discord.Gateway.GatewayGuildEvent, Discord.IGuildScheduledEvent>;

global using GuildEmotesCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayGuildEmote>,
    ulong, Discord.Gateway.GatewayGuildEmote, Discord.Emote>; // TODO: IGuildEmote

global using GuildStickersCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayCustomSticker, Discord.Rest.CustomSticker, Discord.ICustomSticker>,
    ulong, Discord.Gateway.GatewayCustomSticker, Discord.ICustomSticker>;

global using GuildChannelsCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayGuildChannel, Discord.Rest.RestGuildChannel, Discord.IGuildChannel>,
    ulong, Discord.Gateway.GatewayGuildChannel, Discord.IGuildChannel>;

global using GuildRolesCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayRole, Discord.Rest.RestRole, Discord.IRole>,
    ulong, Discord.Gateway.GatewayRole, Discord.IRole>;

global using MessagesCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayMessage, Discord.Rest.RestMessage, Discord.IMessage>,
    ulong, Discord.Gateway.GatewayMessage, Discord.IMessage>;