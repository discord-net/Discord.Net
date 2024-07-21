
#region Guilds

global using GuildIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Guilds.RestGuild, Discord.Rest.Guilds.RestGuildActor, Discord.Models.IGuildModel>;
global using RoleIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Guilds.RestRole, Discord.Rest.Guilds.RestRoleActor, Discord.Models.IRoleModel>;
global using GuildEmoteIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Guilds.RestGuildEmote, Discord.Rest.Guilds.RestGuildEmoteActor, Discord.Models.IGuildEmoteModel>;
global using BanIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Guilds.RestBan, Discord.Rest.Guilds.RestBanActor, Discord.Models.IBanModel>;
global using IntegrationIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Guilds.Integrations.RestIntegration, Discord.Rest.Guilds.Integrations.RestIntegrationActor, Discord.Models.IIntegrationModel>;

#endregion

#region Stickers

global using GuildStickerIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Stickers.RestGuildSticker, Discord.Rest.Stickers.RestGuildStickerActor, Discord.Models.IGuildStickerModel>;
global using StickerIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Stickers.RestSticker, Discord.Models.IStickerModel>;

#endregion

#region Users

global using MemberIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Guilds.RestGuildMember, Discord.Rest.Guilds.RestGuildMemberActor, Discord.Models.IMemberModel>;
global using UserIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestUser, Discord.Rest.RestUserActor, Discord.Models.IUserModel>;
global using ThreadMemberIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestThreadMember, Discord.Rest.RestThreadMemberActor, Discord.Models.IThreadMemberModel>;
global using SelfUserIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestSelfUser, Discord.Rest.RestSelfUserActor, Discord.Models.ISelfUserModel>;
global using GuildScheduledEventIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Guilds.RestGuildScheduledEvent, Discord.Rest.Guilds.RestGuildScheduledEventActor, Discord.Models.IGuildScheduledEventModel>;

#endregion

#region Channels

global using DMChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Channels.RestDMChannel, Discord.Rest.Channels.RestDMChannelActor, Discord.Models.IDMChannelModel>;
global using GroupChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Channels.RestGroupChannel, Discord.Rest.Channels.RestGroupChannelActor, Discord.Models.IGroupDMChannelModel>;
global using ThreadIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestThreadChannel, Discord.Rest.RestThreadChannelActor, Discord.Models.IThreadChannelModel>;
global using ChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Channels.RestChannel, Discord.Rest.Channels.RestChannelActor, Discord.Models.IChannelModel>;
global using MessageChannelIdentity = Discord.IIdentifiable<ulong, Discord.IMessageChannel, Discord.IMessageChannelActor, Discord.Models.IChannelModel>;
global using GuildChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Channels.RestGuildChannel, Discord.Rest.Channels.RestGuildChannelActor, Discord.Models.IGuildChannelModel>;
global using ThreadableChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Channels.RestThreadableChannel, Discord.Rest.Channels.RestThreadableChannelActor, Discord.Models.IThreadableChannelModel>;
global using VoiceChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Channels.RestVoiceChannel, Discord.Rest.Channels.RestVoiceChannelActor, Discord.Models.IGuildVoiceChannelModel>;
global using ForumChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Channels.RestForumChannel, Discord.Rest.Channels.RestForumChannelActor, Discord.Models.IGuildForumChannelModel>;
global using MediaChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Channels.RestMediaChannel, Discord.Rest.Channels.RestMediaChannelActor, Discord.Models.IGuildMediaChannelModel>;
global using NewsChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Channels.RestNewsChannel, Discord.Rest.Channels.RestNewsChannelActor, Discord.Models.IGuildNewsChannelModel>;
global using TextChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Channels.RestTextChannel, Discord.Rest.Channels.RestTextChannelActor, Discord.Models.IGuildTextChannelModel>;
global using StageChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Channels.RestStageChannel, Discord.Rest.Channels.RestStageChannelActor, Discord.Models.IGuildStageChannelModel>;
global using CategoryChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Channels.RestCategoryChannel, Discord.Rest.Channels.RestCategoryChannelActor, Discord.Models.IGuildCategoryChannelModel>;
global using IntegrationChannelIdentity = Discord.IIdentifiable<ulong, Discord.IIntegrationChannel, Discord.IIntegrationChannelActor, Discord.Models.IGuildChannelModel>;
#endregion

#region Messages

global using MessageIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Messages.RestMessage, Discord.Rest.Messages.RestMessageActor, Discord.Models.IMessageModel>;

#endregion

#region Invites

global using InviteIdentity = Discord.IIdentifiable<string, Discord.Rest.Invites.RestInvite, Discord.Rest.Invites.RestInviteActor, Discord.Models.IInviteModel>;

#endregion

#region Stage

global using StageInstanceIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Stage.RestStageInstance, Discord.Rest.Stage.RestStageInstanceActor, Discord.Models.IStageInstanceModel>;

#endregion

#region Webhooks

global using WebhookIdentity = Discord.IIdentifiable<ulong, Discord.Rest.Webhooks.RestWebhook, Discord.Rest.Webhooks.RestWebhookActor, Discord.Models.IWebhookModel>;

#endregion
