using Discord.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
	internal sealed class GlobalUsers : AsyncCollection<GlobalUser>
	{
		public GlobalUsers(DiscordClient client, object writerLock)
			: base(client, writerLock) { }

		public GlobalUser GetOrAdd(string id) => GetOrAdd(id, () => new GlobalUser(_client, id));
	}

	public partial class DiscordClient
	{
		public event EventHandler<MemberEventArgs> UserAdded;
		private void RaiseUserAdded(User member)
		{
			if (UserAdded != null)
				RaiseEvent(nameof(UserAdded), () => UserAdded(this, new MemberEventArgs(member)));
		}
		public event EventHandler<MemberEventArgs> UserRemoved;
		private void RaiseUserRemoved(User member)
		{
			if (UserRemoved != null)
				RaiseEvent(nameof(UserRemoved), () => UserRemoved(this, new MemberEventArgs(member)));
		}
		public event EventHandler ProfileUpdated;
		private void RaiseProfileUpdated()
		{
			if (ProfileUpdated != null)
				RaiseEvent(nameof(ProfileUpdated), () => ProfileUpdated(this, EventArgs.Empty));
		}
		public event EventHandler<MemberEventArgs> MemberUpdated;
		private void RaiseMemberUpdated(User member)
		{
			if (MemberUpdated != null)
				RaiseEvent(nameof(MemberUpdated), () => MemberUpdated(this, new MemberEventArgs(member)));
		}
		public event EventHandler<MemberEventArgs> UserPresenceUpdated;
		private void RaiseUserPresenceUpdated(User member)
		{
			if (UserPresenceUpdated != null)
				RaiseEvent(nameof(UserPresenceUpdated), () => UserPresenceUpdated(this, new MemberEventArgs(member)));
		}
		public event EventHandler<MemberEventArgs> UserVoiceStateUpdated;
		private void RaiseUserVoiceStateUpdated(User member)
		{
			if (UserVoiceStateUpdated != null)
				RaiseEvent(nameof(UserVoiceStateUpdated), () => UserVoiceStateUpdated(this, new MemberEventArgs(member)));
		}
	
		/// <summary> Returns a collection of all users this client can currently see. </summary>
		internal GlobalUsers GlobalUsers => _globalUsers;
		private readonly GlobalUsers _globalUsers;

		public Task<EditUserResponse> EditProfile(string currentPassword = "",
			string username = null, string email = null, string password = null,
			ImageType avatarType = ImageType.Png, byte[] avatar = null)
		{
			if (currentPassword == null) throw new ArgumentNullException(nameof(currentPassword));
			CheckReady();

			return _api.EditUser(currentPassword: currentPassword, 
				username: username ?? _currentUser?.Name,  email: email ?? _currentUser?.GlobalUser.Email, password: password,
				avatarType: avatarType, avatar: avatar);
		}

		public Task SetStatus(UserStatus status)
		{
			if (status == (string)null) throw new ArgumentNullException(nameof(status));
			CheckReady();

			if (status != UserStatus.Online && status != UserStatus.Idle)
				throw new ArgumentException($"Invalid status, must be {UserStatus.Online} or {UserStatus.Idle}");
			_status = status;
			return SendStatus();
		}
		public Task SetGame(int? gameId)
		{
			CheckReady();

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