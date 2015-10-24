using Discord.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
	internal sealed class Users : AsyncCollection<GlobalUser>
	{
		public Users(DiscordClient client, object writerLock)
			: base(client, writerLock, x => x.OnCached(), x => x.OnUncached()) { }

		public GlobalUser GetOrAdd(string id) => GetOrAdd(id, () => new GlobalUser(_client, id));
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
		public event EventHandler ProfileUpdated;
		private void RaiseProfileUpdated(GlobalUser user)
		{
			if (ProfileUpdated != null)
				RaiseEvent(nameof(ProfileUpdated), () => ProfileUpdated(this, EventArgs.Empty));
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
		internal Users Users => _users;
		private readonly Users _users;

		public Task<EditUserResponse> EditProfile(string currentPassword = "",
			string username = null, string email = null, string password = null,
			ImageType avatarType = ImageType.Png, byte[] avatar = null)
		{
			if (currentPassword == null) throw new ArgumentNullException(nameof(currentPassword));

			return _api.EditUser(currentPassword: currentPassword, 
				username: username ?? _currentUser?.Name,  email: email ?? _currentUser?.GlobalUser.Email, password: password,
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