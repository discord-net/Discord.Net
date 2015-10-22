using Discord.API;
using Discord.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
	public sealed class UserEventArgs : EventArgs
	{
		public User User { get; }
		public string UserId => User.Id;

		internal UserEventArgs(User user) { User = user; }
	}

	public partial class DiscordClient
	{
		public event EventHandler<MemberEventArgs> UserAdded;
		private void RaiseUserAdded(Member member)
		{
			if (UserAdded != null)
				RaiseEvent(nameof(UserAdded), () => UserAdded(this, new MemberEventArgs(member)));
		}
		public event EventHandler<MemberEventArgs> UserRemoved;
		private void RaiseUserRemoved(Member member)
		{
			if (UserRemoved != null)
				RaiseEvent(nameof(UserRemoved), () => UserRemoved(this, new MemberEventArgs(member)));
		}
		public event EventHandler<UserEventArgs> UserUpdated;
		private void RaiseUserUpdated(User user)
		{
			if (UserUpdated != null)
				RaiseEvent(nameof(UserUpdated), () => UserUpdated(this, new UserEventArgs(user)));
		}
		public event EventHandler<MemberEventArgs> MemberUpdated;
		private void RaiseMemberUpdated(Member member)
		{
			if (MemberUpdated != null)
				RaiseEvent(nameof(MemberUpdated), () => MemberUpdated(this, new MemberEventArgs(member)));
		}
		public event EventHandler<MemberEventArgs> UserPresenceUpdated;
		private void RaiseUserPresenceUpdated(Member member)
		{
			if (UserPresenceUpdated != null)
				RaiseEvent(nameof(UserPresenceUpdated), () => UserPresenceUpdated(this, new MemberEventArgs(member)));
		}
		public event EventHandler<MemberEventArgs> UserVoiceStateUpdated;
		private void RaiseUserVoiceStateUpdated(Member member)
		{
			if (UserVoiceStateUpdated != null)
				RaiseEvent(nameof(UserVoiceStateUpdated), () => UserVoiceStateUpdated(this, new MemberEventArgs(member)));
		}
	
		/// <summary> Returns a collection of all users this client can currently see. </summary>
		public Users Users => _users;
		private readonly Users _users;
		/// <summary> Returns the current logged-in user. </summary>
		public User CurrentUser => _currentUser;
		private User _currentUser;

		/// <summary> Returns the user with the specified id, or null if none was found. </summary>
		public User GetUser(string id) => _users[id];
		/// <summary> Returns the user with the specified name and discriminator, or null if none was found. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
		public User GetUser(string username, string discriminator)
		{
			if (username == null) throw new ArgumentNullException(nameof(username));
			if (discriminator == null) throw new ArgumentNullException(nameof(discriminator));

			if (username.StartsWith("@"))
				username = username.Substring(1);

			return _users.Where(x =>
					string.Equals(x.Name, username, StringComparison.OrdinalIgnoreCase) &&
					x.Discriminator == discriminator
				)
				.FirstOrDefault();
		}

		/// <summary> Returns all users with the specified name across all servers. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
		public IEnumerable<User> FindUsers(string name)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));

			if (name.StartsWith("@"))
			{
				string name2 = name.Substring(1);
				return _users.Where(x =>
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) || string.Equals(x.Name, name2, StringComparison.OrdinalIgnoreCase));
			}
			else
			{
				return _users.Where(x =>
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
			}
		}

		public Task<EditUserResponse> EditProfile(string currentPassword = "",
			string username = null, string email = null, string password = null,
			ImageType avatarType = ImageType.Png, byte[] avatar = null)
		{
			if (currentPassword == null) throw new ArgumentNullException(nameof(currentPassword));

			return _api.EditUser(currentPassword: currentPassword, username: username ?? _currentUser?.Name, email: email ?? _currentUser?.Email, password: password,
				avatarType: avatarType, avatar: avatar);
		}

		public Task SetStatus(string status)
		{
			if (status != UserStatus.Online && status != UserStatus.Idle)
				throw new ArgumentException($"Invalid status, must be {UserStatus.Online} or {UserStatus.Idle}");
			_status = status;
			return SendStatus();
		}
		public Task SetGame(int? gameId)
		{
			_gameId = gameId;
			return SendStatus();
		}
		private Task SendStatus()
		{
			_dataSocket.SendStatus(_status == UserStatus.Idle ? EpochTime.GetMilliseconds() - (10 * 60 * 1000) : (ulong?)null, _gameId);
			return TaskHelper.CompletedTask;
		}
	}
}