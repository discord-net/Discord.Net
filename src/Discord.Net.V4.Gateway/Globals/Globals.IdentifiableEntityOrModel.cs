#region Users

global using UserIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.Users.GatewayUser, Discord.Gateway.Users.GatewayUserActor, Discord.Models.IUserModel>;
global using SelfUserIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.Users.GatewaySelfUser, Discord.Gateway.Users.GatewaySelfUserActor, Discord.Models.ISelfUserModel>;

#endregion

#region Channels

global using ChannelIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayChannel, Discord.Gateway.GatewayChannelActor, Discord.Models.IChannelModel>;

#endregion

global using GuildIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.Guilds.GatewayGuild, Discord.Gateway.Guilds.GatewayGuildActor, Discord.Models.IGuildModel>;
