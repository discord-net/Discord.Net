global using Discord.Models;

global using GuildCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketGuild, Discord.Rest.RestGuild, Discord.IGuild>;
global using GuildUserCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketGuildUser, Discord.Rest.RestGuildUser, Discord.IGuildUser>;
global using UserCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketUser, Discord.Rest.RestUser, Discord.IUser>;
global using MessageCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketMessage, Discord.Rest.RestMessage, Discord.IMessage>;
global using GuildChannelCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketGuildChannel, Discord.Rest.RestGuildChannel, Discord.IGuildChannel>;
global using GuildStickerCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketCustomSticker, Discord.Rest.CustomSticker, Discord.ICustomSticker>;
global using GuildRoleCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketRole, Discord.Rest.RestRole, Discord.IRole>;
global using GuildMemberCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketGuildUser, Discord.Rest.RestGuildUser, Discord.IGuildUser>;
global using GuildEmoteCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketGuildEmote>;
global using GuildEventCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketGuildEvent, Discord.Rest.RestGuildEvent, Discord.IGuildScheduledEvent>;
global using GuildStageInstanceCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketStageInstance, Discord.Rest.RestStageInstance, Discord.IStageInstance>;
global using MessageChannelCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.ISocketMessageChannel, Discord.Rest.IRestMessageChannel, Discord.IMessageChannel>;
global using ThreadChannelCacheable = Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketThreadChannel, Discord.Rest.RestThreadChannel, Discord.IThreadChannel>;

global using GuildStageInstancesCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketStageInstance, Discord.Rest.RestStageInstance, Discord.IStageInstance>,
    ulong, Discord.Gateway.SocketStageInstance>;

global using GuildEventsCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketGuildEvent, Discord.Rest.RestGuildEvent, Discord.IGuildScheduledEvent>,
    ulong, Discord.Gateway.SocketGuildEvent>;

global using GuildEmotesCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketGuildEmote>,
    ulong, Discord.Gateway.SocketGuildEmote>;

global using GuildStickersCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketCustomSticker, Discord.Rest.CustomSticker, Discord.ICustomSticker>,
    ulong, Discord.Gateway.SocketCustomSticker>;

global using GuildChannelsCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketGuildChannel, Discord.Rest.RestGuildChannel, Discord.IGuildChannel>,
    ulong, Discord.Gateway.SocketGuildChannel>;

global using GuildRolesCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketRole, Discord.Rest.RestRole, Discord.IRole>,
    ulong, Discord.Gateway.SocketRole>;

global using MessagesCacheable = Discord.Gateway.CacheableCollection<
    Discord.Gateway.Cacheable<ulong, Discord.Gateway.SocketMessage, Discord.Rest.RestMessage, Discord.IMessage>,
    ulong, Discord.Gateway.SocketMessage>;