using Discord.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
	public sealed class MemberTypingEventArgs : EventArgs
	{
		public Channel Channel { get; }
		public string ChannelId => Channel.Id;
		public Server Server => Channel.Server;
		public string ServerId => Channel.ServerId;
		public Member Member { get; }
		public string UserId => User.Id;
		public User User => Member.User;

		internal MemberTypingEventArgs(Member member, Channel channel)
		{
			Member = member;
			Channel = channel;
		}
	}

	public sealed class MemberIsSpeakingEventArgs : EventArgs
	{
		public Channel Channel => Member.VoiceChannel;
		public string ChannelId => Member.VoiceChannelId;
		public Server Server => Member.Server;
		public string ServerId => Member.ServerId;
		public User User => Member.User;
		public string UserId => Member.UserId;
		public Member Member { get; }
		public bool IsSpeaking { get; }

		internal MemberIsSpeakingEventArgs(Member member, bool isSpeaking)
		{
			Member = member;
			IsSpeaking = isSpeaking;
		}
	}

	public partial class DiscordClient
	{
		public event EventHandler<MemberTypingEventArgs> UserIsTyping;
		private void RaiseUserIsTyping(Member member, Channel channel)
		{
			if (UserIsTyping != null)
				RaiseEvent(nameof(UserIsTyping), () => UserIsTyping(this, new MemberTypingEventArgs(member, channel)));
		}
		public event EventHandler<MemberIsSpeakingEventArgs> UserIsSpeaking;
		private void RaiseUserIsSpeaking(Member member, bool isSpeaking)
		{
			if (UserIsSpeaking != null)
				RaiseEvent(nameof(UserIsSpeaking), () => UserIsSpeaking(this, new MemberIsSpeakingEventArgs(member, isSpeaking)));
		}

		/// <summary> Returns a collection of all user-server pairs this client can currently see. </summary>
		public Members Members => _members;
		private readonly Members _members;
		
		/// <summary> Returns the user with the specified id, along with their server-specific data, or null if none was found. </summary>
		public Member GetMember(Server server, User user) => _members[user?.Id, server?.Id];
		/// <summary> Returns the user with the specified id, along with their server-specific data, or null if none was found. </summary>
		public Member GetMember(Server server, string userId) => _members[userId, server?.Id];
		/// <summary> Returns the user with the specified id, along with their server-specific data, or null if none was found. </summary>
		public Member GetMember(string serverId, User user) => _members[user?.Id, serverId];
		/// <summary> Returns the user with the specified id, along with their server-specific data, or null if none was found. </summary>
		public Member GetMember(string serverId, string userId) => _members[userId, serverId];
		/// <summary> Returns the user with the specified name and discriminator, along withtheir server-specific data, or null if they couldn't be found. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
		public Member GetMember(Server server, string username, string discriminator) 
			=> GetMember(server?.Id, username, discriminator);
		/// <summary> Returns the user with the specified name and discriminator, along withtheir server-specific data, or null if they couldn't be found. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
		public Member GetMember(string serverId, string username, string discriminator)
		{
			User user = GetUser(username, discriminator);
			return _members[user?.Id, serverId];
		}

		/// <summary> Returns all users in with the specified server and name, along with their server-specific data. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive.</remarks>
		public IEnumerable<Member> FindMembers(string serverId, string name) => FindMembers(_servers[serverId], name);
		/// <summary> Returns all users in with the specified server and name, along with their server-specific data. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive.</remarks>
		public IEnumerable<Member> FindMembers(Server server, string name)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (name == null) throw new ArgumentNullException(nameof(name));

			if (name.StartsWith("@"))
			{
				string name2 = name.Substring(1);
				return server.Members.Where(x =>
				{
					var user = x.User;
					if (user == null)
						return false;
					return string.Equals(user.Name, name, StringComparison.OrdinalIgnoreCase) ||
						string.Equals(user.Name, name2, StringComparison.OrdinalIgnoreCase);
				});
			}
			else
			{
				return server.Members.Where(x =>
				{
					var user = x.User;
					if (user == null)
						return false;
					return string.Equals(x.User.Name, name, StringComparison.OrdinalIgnoreCase);
				});
			}
		}

		public Task EditMember(Member member, bool? mute = null, bool? deaf = null, IEnumerable<object> roles = null)
			=> EditMember(member?.ServerId, member?.UserId, mute, deaf, roles);
		public Task EditMember(Server server, User user, bool? mute = null, bool? deaf = null, IEnumerable<object> roles = null)
			=> EditMember(server?.Id, user?.Id, mute, deaf, roles);
		public Task EditMember(Server server, string userId, bool? mute = null, bool? deaf = null, IEnumerable<string> roles = null)
			=> EditMember(server?.Id, userId, mute, deaf, roles);
		public Task EditMember(string serverId, User user, bool? mute = null, bool? deaf = null, IEnumerable<object> roles = null)
			=> EditMember(serverId, user?.Id, mute, deaf, roles);
		public Task EditMember(string serverId, string userId, bool? mute = null, bool? deaf = null, IEnumerable<object> roles = null)
		{
			CheckReady();
			if (serverId == null) throw new NullReferenceException(nameof(serverId));
			if (userId == null) throw new NullReferenceException(nameof(userId));

			var newRoles = CollectionHelper.FlattenRoles(roles);
			return _api.EditMember(serverId, userId, mute: mute, deaf: deaf, roles: newRoles);
		}
	}
}