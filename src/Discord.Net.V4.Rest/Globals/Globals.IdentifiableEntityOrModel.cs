
#region Guilds

global using GuildIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Guilds.RestGuild, Discord.Models.IGuildModel>;
global using RoleIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Guilds.RestRole, Discord.Models.IRoleModel>;
global using GuildEmoteIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Guilds.RestGuildEmote, Discord.Models.IGuildEmoteModel>;
global using BanIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Guilds.RestBan, Discord.Models.IBanModel>;
global using IntegrationIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Guilds.Integrations.RestIntegration, Discord.Models.IIntegrationModel>;

#endregion

#region Stickers

global using GuildStickerIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Stickers.RestGuildSticker, Discord.Models.IGuildStickerModel>;
global using StickerIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Stickers.RestSticker, Discord.Models.IStickerModel>;

#endregion

#region Users

global using MemberIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Guilds.RestGuildMember, Discord.Models.IMemberModel>;
global using UserIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.RestUser, Discord.Models.IUserModel>;
global using ThreadMemberIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.RestThreadMember, Discord.Models.IThreadMemberModel>;
global using SelfUserIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.RestSelfUser, Discord.Models.ISelfUserModel>;
global using GuildScheduledEventIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Guilds.RestGuildScheduledEvent, Discord.Models.IGuildScheduledEventModel>;

#endregion

#region Channels

global using ThreadIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.RestThreadChannel, Discord.Models.IThreadChannelModel>;
global using ChannelIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Channels.RestChannel, Discord.Models.IChannelModel>;
global using MessageChannelIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.IMessageChannel, Discord.Models.IChannelModel>;
global using GuildChannelIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Channels.RestGuildChannel, Discord.Models.IGuildChannelModel>;
global using ThreadableChannelIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Channels.RestThreadableChannel, Discord.Models.IThreadableChannelModel>;
global using VoiceChannelIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Channels.RestVoiceChannel, Discord.Models.IGuildVoiceChannelModel>;
global using ForumChannelIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Channels.RestForumChannel, Discord.Models.IGuildForumChannelModel>;
global using NewsChannelIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Channels.RestNewsChannel, Discord.Models.IGuildNewsChannelModel>;
global using TextChannelIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Channels.RestTextChannel, Discord.Models.IGuildTextChannelModel>;

#endregion

#region Messages

global using MessageIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Messages.RestMessage, Discord.Models.IMessageModel>;

#endregion

