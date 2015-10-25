using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
	internal sealed class Users : AsyncCollection<User>
	{
		public Users(DiscordClient client, object writerLock)
			: base(client, writerLock, x => x.OnCached(), x => x.OnUncached()) { }
		private string GetKey(string userId, string serverId)
			=> User.GetId(userId, serverId);

		public User this[string userId, string serverId]
			=> this[GetKey(userId, serverId)];
		public User GetOrAdd(string userId, string serverId)
			=> GetOrAdd(GetKey(userId, serverId), () => new User(_client, userId, serverId));
		public User TryRemove(string userId, string serverId)
			=> TryRemove(GetKey(userId, serverId));
	}

	public class MemberEventArgs : EventArgs
	{
		public User User { get; }
		public Server Server => User.Server;

		internal MemberEventArgs(User user) { User = user; }
	}
	public class UserChannelEventArgs : MemberEventArgs
	{
		public Channel Channel { get; }
		public string ChannelId => Channel.Id;

		internal UserChannelEventArgs(User user, Channel channel)
			: base(user)
		{
			Channel = channel;
		}
	}
	public class MemberIsSpeakingEventArgs : UserChannelEventArgs
	{
		public bool IsSpeaking { get; }

		internal MemberIsSpeakingEventArgs(User member, Channel channel, bool isSpeaking)
			: base(member, channel)
		{
			IsSpeaking = isSpeaking;
		}
	}

	public partial class DiscordClient
	{
		public event EventHandler<UserChannelEventArgs> UserIsTyping;
		private void RaiseUserIsTyping(User member, Channel channel)
		{
			if (UserIsTyping != null)
				RaiseEvent(nameof(UserIsTyping), () => UserIsTyping(this, new UserChannelEventArgs(member, channel)));
		}
		public event EventHandler<MemberIsSpeakingEventArgs> UserIsSpeaking;
		private void RaiseUserIsSpeaking(User member, Channel channel, bool isSpeaking)
		{
			if (UserIsSpeaking != null)
				RaiseEvent(nameof(UserIsSpeaking), () => UserIsSpeaking(this, new MemberIsSpeakingEventArgs(member, channel, isSpeaking)));
		}

		private User _currentUser;

		internal Users Users => _users;
		private readonly Users _users;

		/// <summary> Returns the user with the specified id, along with their server-specific data, or null if none was found. </summary>
		public User GetMember(Server server, string userId)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (userId == null) throw new ArgumentNullException(nameof(userId));
			CheckReady();

			return _users[userId, server.Id];
		}
		/// <summary> Returns the user with the specified name and discriminator, along withtheir server-specific data, or null if they couldn't be found. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
		public User GetMember(Server server, string username, string discriminator)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (username == null) throw new ArgumentNullException(nameof(username));
			if (discriminator == null) throw new ArgumentNullException(nameof(discriminator));
			CheckReady();

			User member = FindMembers(server, username, discriminator, true).FirstOrDefault();
			return _users[member?.Id, server.Id];
		}

		/// <summary> Returns all users in with the specified server and name, along with their server-specific data. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive.</remarks>
		public IEnumerable<User> FindMembers(Server server, string name, string discriminator = null, bool exactMatch = false)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (name == null) throw new ArgumentNullException(nameof(name));
			CheckReady();

			IEnumerable<User> query;
			if (!exactMatch && name.StartsWith("@"))
			{
				string name2 = name.Substring(1);
				query = server.Members.Where(x => 
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) ||
					string.Equals(x.Name, name2, StringComparison.OrdinalIgnoreCase));
			}
			else
			{
				query = server.Members.Where(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
			}
			if (discriminator != null)
				query = query.Where(x => x.Discriminator == discriminator);
			return query;
        }

		public Task EditMember(User member, bool? mute = null, bool? deaf = null, IEnumerable<Role> roles = null)
		{
			if (member == null) throw new ArgumentNullException(nameof(member));
			CheckReady();
			
			return _api.EditMember(member.ServerId, member.Id, mute: mute, deaf: deaf, roles: roles.Select(x => x.Id));
		}
	}
}