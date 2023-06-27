global using GuildCacheable = Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketGuild, Discord.Rest.RestGuild, Discord.IGuild>;
global using UserCacheable = Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketUser, Discord.Rest.RestUser, Discord.IUser>;
global using MessageCacheable = Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketMessage, Discord.Rest.RestMessage, Discord.IMessage>;
global using MessageChannelCacheable = Discord.WebSocket.Cacheable<ulong, Discord.WebSocket.SocketMessageChannel, Discord.Rest.IRestMessageChannel, Discord.IMessageChannel>;
