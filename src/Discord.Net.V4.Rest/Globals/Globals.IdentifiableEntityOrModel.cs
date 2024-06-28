#region Channels



#endregion

#region Guilds

global using GuildIdentity = Discord.IdentifiableEntityOrModel<ulong, Discord.Rest.Guilds.RestGuild, Discord.Models.IGuildModel>;

#endregion

#region Users

global using MemberIdentity = Discord.IdentifiableEntityOrModel<ulong, Discord.Rest.Guilds.RestGuildMember, Discord.Models.IMemberModel>;
global using UserIdentity = Discord.IdentifiableEntityOrModel<ulong, Discord.Rest.RestUser, Discord.Models.IUserModel>;
global using ThreadMemberIdentity = Discord.IdentifiableEntityOrModel<ulong, Discord.Rest.RestThreadMember, Discord.Models.IThreadMemberModel>;

#endregion

#region Channels

global using ThreadIdentity = Discord.IdentifiableEntityOrModel<ulong, Discord.Rest.RestThreadChannel, Discord.Models.IThreadChannelModel>;
global using ChannelIdentity = Discord.IdentifiableEntityOrModel<ulong, Discord.Rest.Channels.RestChannel, Discord.Models.IChannelModel>;
global using IChannelIdentity = Discord.IIdentifiableEntityOrModel<ulong, Discord.Rest.Channels.RestChannel>;
#endregion
