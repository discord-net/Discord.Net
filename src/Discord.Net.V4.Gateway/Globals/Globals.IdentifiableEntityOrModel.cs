#region Invites

global using InviteIdentity = Discord.IIdentifiable<string, Discord.Gateway.GatewayInvite, Discord.Gateway.GatewayInviteActor, Discord.Models.IInviteModel>;
global using GuildInviteIdentity = Discord.IIdentifiable<string, Discord.Gateway.GatewayGuildInvite, Discord.Gateway.GatewayGuildInviteActor, Discord.Models.IInviteModel>;
global using GuildChannelInviteIdentity = Discord.IIdentifiable<string, Discord.Gateway.GatewayGuildChannelInvite, Discord.Gateway.GatewayGuildChannelInviteActor, Discord.Models.IInviteModel>;

#endregion

#region Voice

global using VoiceStateIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayVoiceState, Discord.Gateway.GatewayVoiceStateActor,
        Discord.Models.IVoiceStateModel>;
global using CurrentUserVoiceStateIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayCurrentUserVoiceState,
        Discord.Gateway.GatewayCurrentUserVoiceStateActor,
        Discord.Models.IVoiceStateModel>;

#endregion

#region Stickers

global using StickerIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewaySticker, Discord.Gateway.GatewayStickerActor,
        Discord.Models.IStickerModel>;
global using StickerPackIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayStickerPack, Discord.Gateway.GatewayStickerPackActor,
        Discord.Models.IStickerPackModel>;

#endregion

#region Users

global using UserIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayUser, Discord.Gateway.GatewayUserActor,
        Discord.Models.IUserModel>;
global using SelfUserIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayCurrentUser, Discord.Gateway.GatewayCurrentUserActor,
        Discord.Models.ISelfUserModel>;
global using MemberIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayMember, Discord.Gateway.GatewayMemberActor,
        Discord.Models.IMemberModel>;
global using CurrentMemberIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayCurrentMember, Discord.Gateway.GatewayCurrentMemberActor,
        Discord.Models.IMemberModel>;
global using GuildScheduledEventUserIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayGuildScheduledEventUser,
        Discord.Gateway.GatewayGuildScheduledEventUserActor, Discord.Models.IGuildScheduledEventUserModel>;
global using ThreadMemberIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayThreadMember, Discord.Gateway.GatewayThreadMemberActor,
        Discord.Models.IThreadMemberModel>;

#endregion

#region Channels

global using ChannelIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayChannel, Discord.Gateway.GatewayChannelActor,
        Discord.Models.IChannelModel>;
global using DMChannelIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayDMChannel, Discord.Gateway.GatewayDMChannelActor,
        Discord.Models.IDMChannelModel>;
global using GroupChannelIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayGroupChannel, Discord.Gateway.GatewayGroupChannelActor,
        Discord.Models.IGroupDMChannelModel>;
global using MessageChannelIdentity =
    Discord.IIdentifiable<ulong, Discord.IMessageChannel, Discord.IMessageChannelActor, Discord.Models.IChannelModel>;
global using GuildChannelIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayGuildChannel, Discord.Gateway.GatewayGuildChannelActor,
        Discord.Models.IGuildChannelModel>;
global using CategoryChannelIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayCategoryChannel, Discord.Gateway.GatewayCategoryChannelActor,
        Discord.Models.IGuildCategoryChannelModel>;
global using ThreadableChannelIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayThreadableChannel, Discord.Gateway.GatewayThreadableChannelActor
        , Discord.Models.IThreadableChannelModel>;
global using ForumChannelIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayForumChannel, Discord.Gateway.GatewayForumChannelActor,
        Discord.Models.IGuildForumChannelModel>;
global using IntegrationChannelIdentity =
    Discord.IIdentifiable<ulong, Discord.IIntegrationChannel, Discord.IIntegrationChannelActor,
        Discord.Models.IGuildChannelModel>;
global using MediaChannelIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayMediaChannel, Discord.Gateway.GatewayMediaChannelActor,
        Discord.Models.IGuildMediaChannelModel>;
global using TextChannelIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayTextChannel, Discord.Gateway.GatewayTextChannelActor,
        Discord.Models.IGuildTextChannelModel>;
global using VoiceChannelIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayVoiceChannel, Discord.Gateway.GatewayVoiceChannelActor,
        Discord.Models.IGuildVoiceChannelModel>;
global using NewsChannelIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayNewsChannel, Discord.Gateway.GatewayNewsChannelActor,
        Discord.Models.IGuildNewsChannelModel>;
global using StageChannelIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayStageChannel, Discord.Gateway.GatewayStageChannelActor,
        Discord.Models.IGuildStageChannelModel>;
global using ThreadChannelIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayThreadChannel, Discord.Gateway.GatewayThreadChannelActor,
        Discord.Models.IThreadChannelModel>;

#endregion

#region Guilds

global using GuildIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayGuild, Discord.Gateway.GatewayGuildActor,
        Discord.Models.IGuildModel>;
global using GuildScheduledEventIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayGuildScheduledEvent,
        Discord.Gateway.GatewayGuildScheduledEventActor, Discord.Models.IGuildScheduledEventModel>;
global using BanIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayBan, Discord.Gateway.GatewayBanActor, Discord.Models.IBanModel>;
global using RoleIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayRole, Discord.Gateway.GatewayRoleActor,
        Discord.Models.IRoleModel>;
global using GuildEmoteIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayGuildEmote, Discord.Gateway.GatewayGuildEmoteActor,
        Discord.Models.IGuildEmoteModel>;
global using GuildStickerIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayGuildSticker,
        Discord.Gateway.GatewayGuildStickerActor, Discord.Models.IGuildStickerModel>;
global using IntegrationIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayIntegration, Discord.Gateway.GatewayIntegrationActor,
        Discord.Models.IIntegrationModel>;

#endregion

global using StageInstanceIdentity =
    Discord.IIdentifiable<ulong, Discord.Gateway.GatewayStageInstance, Discord.Gateway.GatewayStageInstanceActor,
        Discord.Models.IStageInstanceModel>;
