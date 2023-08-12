global using Discord.Models;

global using GuildCacheable = Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketGuild, Discord.Rest.RestGuild, Discord.IGuild>;
global using GuildUserCacheable = Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketGuildUser, Discord.Rest.RestGuildUser, Discord.IGuildUser>;
global using UserCacheable = Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketUser, Discord.Rest.RestUser, Discord.IUser>;
global using MessageCacheable = Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketMessage, Discord.Rest.RestMessage, Discord.IMessage>;
global using GuildChannelCacheable = Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketGuildChannel, Discord.Rest.RestGuildChannel, Discord.IGuildChannel>;
global using GuildStickerCacheable = Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketCustomSticker, Discord.Rest.CustomSticker, Discord.ICustomSticker>;
global using GuildRoleCacheable = Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketRole, Discord.Rest.RestRole, Discord.IRole>;
global using GuildMemberCacheable = Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketGuildUser, Discord.Rest.RestGuildUser, Discord.IGuildUser>;
global using GuildEmoteCacheable = Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketGuildEmote>;
global using GuildEventCacheable = Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketGuildEvent, Discord.Rest.RestGuildEvent, Discord.IGuildScheduledEvent>;
global using GuildStageInstanceCacheable = Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketStageInstance, Discord.Rest.RestStageInstance, Discord.IStageInstance>;


global using GuildStageInstancesCacheable = Discord.WebSocket.CacheableCollection<
    Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketStageInstance, Discord.Rest.RestStageInstance, Discord.IStageInstance>,
    ulong, Discord.WebSocket.SocketStageInstance>;

global using GuildEventsCacheable = Discord.WebSocket.CacheableCollection<
    Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketGuildEvent, Discord.Rest.RestGuildEvent, Discord.IGuildScheduledEvent>,
    ulong, Discord.WebSocket.SocketGuildEvent>;

global using GuildEmotesCacheable = Discord.WebSocket.CacheableCollection<
    Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketGuildEmote>,
    ulong, Discord.WebSocket.SocketGuildEmote>;

global using GuildStickersCacheable = Discord.WebSocket.CacheableCollection<
    Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketCustomSticker, Discord.Rest.CustomSticker, Discord.ICustomSticker>,
    ulong, Discord.WebSocket.SocketCustomSticker>;

global using GuildChannelsCacheable = Discord.WebSocket.CacheableCollection<
    Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketGuildChannel, Discord.Rest.RestGuildChannel, Discord.IGuildChannel>,
    ulong, Discord.WebSocket.SocketGuildChannel>;

global using GuildRolesCacheable = Discord.WebSocket.CacheableCollection<
    Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketRole, Discord.Rest.RestRole, Discord.IRole>,
    ulong, Discord.WebSocket.SocketRole>;