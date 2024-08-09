
#region Guilds

global using GuildIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestGuild, Discord.Rest.RestGuildActor, Discord.Models.IGuildModel>;
global using RoleIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestRole, Discord.Rest.RestRoleActor, Discord.Models.IRoleModel>;
global using GuildEmoteIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestGuildEmote, Discord.Rest.RestGuildEmoteActor, Discord.Models.IGuildEmoteModel>;
global using BanIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestBan, Discord.Rest.RestBanActor, Discord.Models.IBanModel>;
global using IntegrationIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestIntegration, Discord.Rest.RestIntegrationActor, Discord.Models.IIntegrationModel>;

#endregion

#region Stickers

global using GuildStickerIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestGuildSticker, Discord.Rest.RestGuildStickerActor, Discord.Models.IGuildStickerModel>;
global using StickerIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestSticker, Discord.Rest.RestStickerActor, Discord.Models.IStickerModel>;
global using StickerPackIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestStickerPack, Discord.Rest.RestStickerPackActor, Discord.Models.IStickerPackModel>;

#endregion

#region Users

global using MemberIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestMember, Discord.Rest.RestMemberActor, Discord.Models.IMemberModel>;
global using CurrentMemberIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestCurrentMember, Discord.Rest.RestCurrentMemberActor, Discord.Models.IMemberModel>;
global using UserIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestUser, Discord.Rest.RestUserActor, Discord.Models.IUserModel>;
global using SelfUserIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestCurrentUser, Discord.Rest.RestCurrentUserActor, Discord.Models.ISelfUserModel>;
global using ThreadMemberIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestThreadMember, Discord.Rest.RestThreadMemberActor, Discord.Models.IThreadMemberModel>;
global using GuildScheduledEventUserIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestGuildScheduledEventUser, Discord.Rest.RestGuildScheduledEventUserActor, Discord.Models.IGuildScheduledEventUserModel>;
global using VoiceStateIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestVoiceState, Discord.Rest.RestVoiceStateActor, Discord.Models.IVoiceStateModel>;
global using CurrentUserVoiceStateIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestCurrentUserVoiceState, Discord.Rest.RestCurrentUserVoiceStateActor, Discord.Models.IVoiceStateModel>;

#endregion

#region Channels

global using DMChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestDMChannel, Discord.Rest.RestDMChannelActor, Discord.Models.IDMChannelModel>;
global using GroupChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestGroupChannel, Discord.Rest.RestGroupChannelActor, Discord.Models.IGroupDMChannelModel>;
global using ThreadIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestThreadChannel, Discord.Rest.RestThreadChannelActor, Discord.Models.IThreadChannelModel>;
global using ChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestChannel, Discord.Rest.RestChannelActor, Discord.Models.IChannelModel>;
global using MessageChannelIdentity = Discord.IIdentifiable<ulong, Discord.IMessageChannel, Discord.IMessageChannelActor, Discord.Models.IChannelModel>;
global using GuildChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestGuildChannel, Discord.Rest.RestGuildChannelActor, Discord.Models.IGuildChannelModel>;
global using ThreadableChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestThreadableChannel, Discord.Rest.RestThreadableChannelActor, Discord.Models.IThreadableChannelModel>;
global using VoiceChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestVoiceChannel, Discord.Rest.RestVoiceChannelActor, Discord.Models.IGuildVoiceChannelModel>;
global using ForumChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestForumChannel, Discord.Rest.RestForumChannelActor, Discord.Models.IGuildForumChannelModel>;
global using MediaChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestMediaChannel, Discord.Rest.RestMediaChannelActor, Discord.Models.IGuildMediaChannelModel>;
global using NewsChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestNewsChannel, Discord.Rest.RestNewsChannelActor, Discord.Models.IGuildNewsChannelModel>;
global using TextChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestTextChannel, Discord.Rest.RestTextChannelActor, Discord.Models.IGuildTextChannelModel>;
global using StageChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestStageChannel, Discord.Rest.RestStageChannelActor, Discord.Models.IGuildStageChannelModel>;
global using CategoryChannelIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestCategoryChannel, Discord.Rest.RestCategoryChannelActor, Discord.Models.IGuildCategoryChannelModel>;
global using IntegrationChannelIdentity = Discord.IIdentifiable<ulong, Discord.IIntegrationChannel, Discord.IIntegrationChannelActor, Discord.Models.IGuildChannelModel>;
#endregion

#region Messages

global using MessageIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestMessage, Discord.Rest.RestMessageActor, Discord.Models.IMessageModel>;

#endregion

#region Invites

global using InviteIdentity = Discord.IIdentifiable<string, Discord.Rest.RestInvite, Discord.Rest.RestInviteActor, Discord.Models.IInviteModel>;

#endregion

#region Stage

global using StageInstanceIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestStageInstance, Discord.Rest.RestStageInstanceActor, Discord.Models.IStageInstanceModel>;

#endregion

#region Events

global using GuildScheduledEventIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestGuildScheduledEvent, Discord.Rest.RestGuildScheduledEventActor, Discord.Models.IGuildScheduledEventModel>;

#endregion

#region Webhooks

global using WebhookIdentity = Discord.IIdentifiable<ulong, Discord.Rest.RestWebhook, Discord.Rest.RestWebhookActor, Discord.Models.IWebhookModel>;

#endregion
