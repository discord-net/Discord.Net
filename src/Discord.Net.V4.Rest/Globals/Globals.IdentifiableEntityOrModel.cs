#region Channels



#endregion

#region Guilds


#endregion

#region Users

global using MemberIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Guilds.RestGuildMember, Discord.Models.IMemberModel>;
global using UserIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.RestUser, Discord.Models.IUserModel>;
global using ThreadMemberIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.RestThreadMember, Discord.Models.IThreadMemberModel>;
global using SelfUserIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.RestSelfUser, Discord.Models.ISelfUserModel>;

#endregion

#region Channels

global using ThreadIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.RestThreadChannel, Discord.Models.IThreadChannelModel>;
global using ChannelIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Channels.RestChannel, Discord.Models.IChannelModel>;
global using GuildChannelIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Channels.RestGuildChannel, Discord.Models.IGuildChannelModel>;
global using ThreadableChannelIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Channels.RestThreadableChannel, Discord.Models.IThreadableChannelModel>;
global using VoiceChannelIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Channels.RestVoiceChannel, Discord.Models.IGuildVoiceChannelModel>;
global using ForumChannelIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Channels.RestForumChannel, Discord.Models.IGuildForumChannelModel>;
global using NewsChannelIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Channels.RestNewsChannel, Discord.Models.IGuildNewsChannelModel>;
global using TextChannelIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Channels.RestTextChannel, Discord.Models.IGuildTextChannelModel>;
#endregion

global using GuildIdentity = Discord.IdentifiableEntityOrModel<ulong, Discord.Rest.Guilds.RestGuild, Discord.Models.IGuildModel>;
