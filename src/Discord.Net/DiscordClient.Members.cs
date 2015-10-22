using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
	internal sealed class Members : AsyncCollection<Member>
	{
		public Members(DiscordClient client, object writerLock)
			: base(client, writerLock, x => x.OnCached(), x => x.OnUncached()) { }
		private string GetKey(string userId, string serverId)
			=> serverId + '_' + userId;

		public Member this[string userId, string serverId]
			=> this[GetKey(userId, serverId)];
		public Member GetOrAdd(string userId, string serverId)
			=> GetOrAdd(GetKey(userId, serverId), () => new Member(_client, userId, serverId));
		public Member TryRemove(string userId, string serverId)
			=> TryRemove(GetKey(userId, serverId));
	}

	public class MemberEventArgs : EventArgs
	{
		public Member Member { get; }
		public User User => Member.User;
		public string UserId => Member.UserId;
		public Server Server => Member.Server;
		public string ServerId => Member.ServerId;

		internal MemberEventArgs(Member member) { Member = member; }
	}
	public class MemberChannelEventArgs : MemberEventArgs
	{
		public Channel Channel { get; }
		public string ChannelId => Channel.Id;

		internal MemberChannelEventArgs(Member member, Channel channel)
			: base(member)
		{
			Channel = channel;
		}
	}
	public class MemberIsSpeakingEventArgs : MemberChannelEventArgs
	{
		public bool IsSpeaking { get; }

		internal MemberIsSpeakingEventArgs(Member member, Channel channel, bool isSpeaking)
			: base(member, channel)
		{
			IsSpeaking = isSpeaking;
		}
	}

	public partial class DiscordClient
	{
		public event EventHandler<MemberChannelEventArgs> UserIsTyping;
		private void RaiseUserIsTyping(Member member, Channel channel)
		{
			if (UserIsTyping != null)
				RaiseEvent(nameof(UserIsTyping), () => UserIsTyping(this, new MemberChannelEventArgs(member, channel)));
		}
		public event EventHandler<MemberIsSpeakingEventArgs> UserIsSpeaking;
		private void RaiseUserIsSpeaking(Member member, Channel channel, bool isSpeaking)
		{
			if (UserIsSpeaking != null)
				RaiseEvent(nameof(UserIsSpeaking), () => UserIsSpeaking(this, new MemberIsSpeakingEventArgs(member, channel, isSpeaking)));
		}
		
		internal Members Members => _members;
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
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (userId == null) throw new ArgumentNullException(nameof(userId));

			var newRoles = CollectionHelper.FlattenRoles(roles);
			return _api.EditMember(serverId, userId, mute: mute, deaf: deaf, roles: newRoles);
		}
	}
}