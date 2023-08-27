global using Discord.Models;

global using GuildCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayGuild, Discord.Rest.RestGuild, Discord.IGuild>;
global using GuildUserCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayGuildUser, Discord.Rest.RestGuildUser, Discord.IGuildUser>;
global using UserCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayUser, Discord.Rest.RestUser, Discord.IUser>;
global using MessageCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayMessage, Discord.Rest.RestMessage, Discord.IMessage>;
global using GuildChannelCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketGuildChannel, Discord.Rest.RestGuildChannel, Discord.IGuildChannel>;
global using GuildStickerCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayCustomSticker, Discord.Rest.CustomSticker, Discord.ICustomSticker>;
global using GuildRoleCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayRole, Discord.Rest.RestRole, Discord.IRole>;
global using GuildMemberCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayGuildUser, Discord.Rest.RestGuildUser, Discord.IGuildUser>;
global using GuildEmoteCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayGuildEmote>;
global using GuildEventCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayGuildEvent, Discord.Rest.RestGuildEvent, Discord.IGuildScheduledEvent>;
global using GuildStageInstanceCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayStageInstance, Discord.Rest.RestStageInstance, Discord.IStageInstance>;
global using MessageChannelCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.ISocketMessageChannel, Discord.Rest.IRestMessageChannel, Discord.IMessageChannel>;
global using ThreadChannelCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketThreadChannel, Discord.Rest.RestThreadChannel, Discord.IThreadChannel>;

global using GuildStageInstancesCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayStageInstance, Discord.Rest.RestStageInstance, Discord.IStageInstance>,
    ulong, Discord.Gateway.GatewayStageInstance>;

global using GuildEventsCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayGuildEvent, Discord.Rest.RestGuildEvent, Discord.IGuildScheduledEvent>,
    ulong, Discord.Gateway.GatewayGuildEvent>;

global using GuildEmotesCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayGuildEmote>,
    ulong, Discord.Gateway.GatewayGuildEmote>;

global using GuildStickersCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayCustomSticker, Discord.Rest.CustomSticker, Discord.ICustomSticker>,
    ulong, Discord.Gateway.GatewayCustomSticker>;

global using GuildChannelsCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketGuildChannel, Discord.Rest.RestGuildChannel, Discord.IGuildChannel>,
    ulong, Discord.Gateway.SocketGuildChannel>;

global using GuildRolesCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayRole, Discord.Rest.RestRole, Discord.IRole>,
    ulong, Discord.Gateway.GatewayRole>;

global using MessagesCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.GatewayMessage, Discord.Rest.RestMessage, Discord.IMessage>,
    ulong, Discord.Gateway.GatewayMessage>;