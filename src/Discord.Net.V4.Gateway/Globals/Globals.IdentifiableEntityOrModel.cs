#region Users

global using UserIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.Users.GatewayUser, Discord.Gateway.Users.GatewayUserActor, Discord.Models.IUserModel>;
global using SelfUserIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.Users.GatewaySelfUser, Discord.Gateway.Users.GatewaySelfUserActor, Discord.Models.ISelfUserModel>;

#endregion

#region Channels

global using ChannelIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayChannel, Discord.Gateway.GatewayChannelActor, Discord.Models.IChannelModel>;
global using DMChannelIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayDMChannel, Discord.Gateway.GatewayDMChannelActor, Discord.Models.IDMChannelModel>;
global using MessageChannelIdentity = Discord.IIdentifiable<ulong, Discord.IMessageChannel, Discord.IMessageChannelActor, Discord.Models.IChannelModel>;
global using GuildChannelIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayGuildChannel, Discord.Gateway.GatewayGuildChannelActor, Discord.Models.IGuildChannelModel>;
global using CategoryChannelIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayCategoryChannel, Discord.Gateway.GatewayCategoryChannelActor, Discord.Models.IGuildCategoryChannelModel>;

#endregion

global using GuildIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.Guilds.GatewayGuild, Discord.Gateway.Guilds.GatewayGuildActor, Discord.Models.IGuildModel>;
