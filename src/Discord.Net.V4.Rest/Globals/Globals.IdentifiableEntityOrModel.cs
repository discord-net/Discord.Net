#region Channels

global using RestChannelIdentifiable = Discord.IdentifiableEntityOrModel<ulong, Discord.Rest.Channels.RestChannel, Discord.Models.IChannelModel>;
global using IRestChannelIdentifiable = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Channels.RestChannel, Discord.Models.IChannelModel>;

#endregion

#region Guilds

global using RestGuildIdentifiable = Discord.IdentifiableEntityOrModel<ulong, Discord.Rest.Guilds.RestGuild, Discord.Models.IGuildModel>;

#endregion
