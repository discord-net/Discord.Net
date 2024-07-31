#region Users

global using UserIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayUser, Discord.Gateway.GatewayUserActor, Discord.Models.IUserModel>;
global using SelfUserIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewaySelfUser, Discord.Gateway.GatewaySelfUserActor, Discord.Models.ISelfUserModel>;

#endregion

#region Channels

global using ChannelIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayChannel, Discord.Gateway.GatewayChannelActor, Discord.Models.IChannelModel>;
global using DMChannelIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayDMChannel, Discord.Gateway.GatewayDMChannelActor, Discord.Models.IDMChannelModel>;
global using GroupChannelIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayGroupChannel, Discord.Gateway.GatewayGroupChannelActor, Discord.Models.IGroupDMChannelModel>;
global using MessageChannelIdentity = Discord.IIdentifiable<ulong, Discord.IMessageChannel, Discord.IMessageChannelActor, Discord.Models.IChannelModel>;
global using GuildChannelIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayGuildChannel, Discord.Gateway.GatewayGuildChannelActor, Discord.Models.IGuildChannelModel>;
global using CategoryChannelIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayCategoryChannel, Discord.Gateway.GatewayCategoryChannelActor, Discord.Models.IGuildCategoryChannelModel>;
global using ThreadableChannelIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayThreadableChannel, Discord.Gateway.GatewayThreadableChannelActor, Discord.Models.IThreadableChannelModel>;
global using ForumChannelIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayForumChannel, Discord.Gateway.GatewayForumChannelActor, Discord.Models.IGuildForumChannelModel>;
global using IntegrationChannelIdentity = Discord.IIdentifiable<ulong, Discord.IIntegrationChannel, Discord.IIntegrationChannelActor, Discord.Models.IGuildChannelModel>;
global using MediaChannelIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayMediaChannel, Discord.Gateway.GatewayMediaChannelActor, Discord.Models.IGuildMediaChannelModel>;
global using TextChannelIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayTextChannel, Discord.Gateway.GatewayTextChannelActor, Discord.Models.IGuildTextChannelModel>;
global using VoiceChannelIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayVoiceChannel, Discord.Gateway.GatewayVoiceChannelActor, Discord.Models.IGuildVoiceChannelModel>;
global using NewsChannelIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayNewsChannel, Discord.Gateway.GatewayNewsChannelActor, Discord.Models.IGuildNewsChannelModel>;
global using StageChannelIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayStageChannel, Discord.Gateway.GatewayStageChannelActor, Discord.Models.IGuildStageChannelModel>;
global using ThreadChannelIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayThreadChannel, Discord.Gateway.GatewayThreadChannelActor, Discord.Models.IThreadChannelModel>;

#endregion

#region Guilds

global using GuildIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayGuild, Discord.Gateway.GatewayGuildActor, Discord.Models.IGuildModel>;
global using GuildScheduledEventIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayGuildScheduledEvent, Discord.Gateway.GatewayGuildScheduledEventActor, Discord.Models.IGuildScheduledEventModel>;


#endregion


global using StageInstanceIdentity = Discord.IIdentifiable<ulong, Discord.Gateway.GatewayStageInstance, Discord.Gateway.GatewayStageInstanceActor, Discord.Models.IStageInstanceModel>;
