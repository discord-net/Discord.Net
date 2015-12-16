using Discord.API.Client.Rest;
using Discord.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	internal sealed class GlobalUsers : AsyncCollection<ulong, GlobalUser>
	{
		public GlobalUsers(DiscordClient client, object writerLock)
			: base(client, writerLock) { }

		public GlobalUser GetOrAdd(ulong id) => GetOrAdd(id, () => new GlobalUser(_client, id));
	}
	internal sealed class Users : AsyncCollection<User.CompositeKey, User>
	{
		public Users(DiscordClient client, object writerLock)
			: base(client, writerLock)
		{ }

		public User this[ulong userId, ulong? serverId]
			=> base[new User.CompositeKey(userId, serverId)];
		public User GetOrAdd(ulong userId, ulong? serverId)
			=> GetOrAdd(new User.CompositeKey(userId, serverId), () => new User(_client, userId, serverId));
		public User TryRemove(ulong userId, ulong? serverId)
			=> TryRemove(new User.CompositeKey(userId, serverId));
	}

	public class UserEventArgs : EventArgs
	{
		public User User { get; }
		public Server Server => User.Server;

		public UserEventArgs(User user) { User = user; }
	}
	public class UserChannelEventArgs : UserEventArgs
	{
		public Channel Channel { get; }

		public UserChannelEventArgs(User user, Channel channel)
			: base(user)
		{
			Channel = channel;
		}
	}
	public class BanEventArgs : EventArgs
	{
		public ulong UserId { get; }
		public Server Server { get; }

		public BanEventArgs(ulong userId, Server server)
		{
			UserId = userId;
			Server = server;
		}
	}

	public partial class DiscordClient : IDisposable
	{
		public event EventHandler<UserEventArgs> UserJoined;
		private void RaiseUserJoined(User user)
		{
			if (UserJoined != null)
				EventHelper.Raise(_logger, nameof(UserJoined), () => UserJoined(this, new UserEventArgs(user)));
		}
		public event EventHandler<UserEventArgs> UserLeft;
		private void RaiseUserLeft(User user)
		{
			if (UserLeft != null)
				EventHelper.Raise(_logger, nameof(UserLeft), () => UserLeft(this, new UserEventArgs(user)));
		}
		public event EventHandler<UserEventArgs> UserUpdated;
		private void RaiseUserUpdated(User user)
		{
			if (UserUpdated != null)
				EventHelper.Raise(_logger, nameof(UserUpdated), () => UserUpdated(this, new UserEventArgs(user)));
		}
		public event EventHandler<UserEventArgs> UserPresenceUpdated;
		private void RaiseUserPresenceUpdated(User user)
		{
			if (UserPresenceUpdated != null)
				EventHelper.Raise(_logger, nameof(UserPresenceUpdated), () => UserPresenceUpdated(this, new UserEventArgs(user)));
		}
		public event EventHandler<UserEventArgs> UserVoiceStateUpdated;
		private void RaiseUserVoiceStateUpdated(User user)
		{
			if (UserVoiceStateUpdated != null)
				EventHelper.Raise(_logger, nameof(UserVoiceStateUpdated), () => UserVoiceStateUpdated(this, new UserEventArgs(user)));
		}
		public event EventHandler<UserChannelEventArgs> UserIsTypingUpdated;
		private void RaiseUserIsTyping(User user, Channel channel)
		{
			if (UserIsTypingUpdated != null)
				EventHelper.Raise(_logger, nameof(UserIsTypingUpdated), () => UserIsTypingUpdated(this, new UserChannelEventArgs(user, channel)));
		}
		public event EventHandler ProfileUpdated;
		private void RaiseProfileUpdated()
		{
			if (ProfileUpdated != null)
				EventHelper.Raise(_logger, nameof(ProfileUpdated), () => ProfileUpdated(this, EventArgs.Empty));
		}
		public event EventHandler<BanEventArgs> UserBanned;
		private void RaiseUserBanned(ulong userId, Server server)
		{
			if (UserBanned != null)
				EventHelper.Raise(_logger, nameof(UserBanned), () => UserBanned(this, new BanEventArgs(userId, server)));
		}
		public event EventHandler<BanEventArgs> UserUnbanned;
		private void RaiseUserUnbanned(ulong userId, Server server)
		{
			if (UserUnbanned != null)
				EventHelper.Raise(_logger, nameof(UserUnbanned), () => UserUnbanned(this, new BanEventArgs(userId, server)));
		}

		/// <summary> Returns the current logged-in user used in private channels. </summary>
		internal User PrivateUser => _privateUser;
		private User _privateUser;

        /// <summary> Returns information about the currently logged-in account. </summary>
        public GlobalUser CurrentUser => _currentUser;
        private GlobalUser _currentUser;

        /// <summary> Returns a collection of all unique users this client can currently see. </summary>
        public IEnumerable<GlobalUser> AllUsers { get { CheckReady(); return _globalUsers; } }
		internal GlobalUsers GlobalUsers => _globalUsers;
		private readonly GlobalUsers _globalUsers;

		internal Users Users => _users;
		private readonly Users _users;

		public GlobalUser GetUser(ulong userId)
		{
			CheckReady();

			return _globalUsers[userId];
		}
		/// <summary> Returns the user with the specified id, along with their server-specific data, or null if none was found. </summary>
		public User GetUser(Server server, ulong userId)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			CheckReady();

			return _users[userId, server.Id];
		}
		/// <summary> Returns the user with the specified name and discriminator, along withtheir server-specific data, or null if they couldn't be found. </summary>
		public User GetUser(Server server, string username, ushort discriminator)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (username == null) throw new ArgumentNullException(nameof(username));
			CheckReady();

			return FindUsers(server.Members, server.Id, username, discriminator, true).FirstOrDefault();
		}

		/// <summary> Returns all users with the specified server and name, along with their server-specific data. </summary>
		/// <remarks> Name formats supported: Name, @Name and &lt;@Id&gt;. Search is case-insensitive if exactMatch is false.</remarks>
		public IEnumerable<User> FindUsers(Server server, string name, bool exactMatch = false)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (name == null) throw new ArgumentNullException(nameof(name));
			CheckReady();

			return FindUsers(server.Members, server.Id, name, exactMatch: exactMatch);
		}
		/// <summary> Returns all users with the specified channel and name, along with their server-specific data. </summary>
		/// <remarks> Name formats supported: Name, @Name and &lt;@Id&gt;. Search is case-insensitive if exactMatch is false.</remarks>
		public IEnumerable<User> FindUsers(Channel channel, string name, bool exactMatch = false)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (name == null) throw new ArgumentNullException(nameof(name));
			CheckReady();

			return FindUsers(channel.Members, channel.IsPrivate ? (ulong?)null : channel.Server.Id, name, exactMatch: exactMatch);
        }

		private IEnumerable<User> FindUsers(IEnumerable<User> users, ulong? serverId, string name, ushort? discriminator = null, bool exactMatch = false)
		{
			var query = users.Where(x => string.Equals(x.Name, name, exactMatch ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase));

			if (!exactMatch && name.Length >= 2)
			{
				if (name[0] == '<' && name[1] == '@' && name[name.Length - 1] == '>') //Parse mention
				{
                    ulong id = IdConvert.ToLong(name.Substring(2, name.Length - 3));
					var user = _users[id, serverId];
					if (user != null)
						query = query.Concat(new User[] { user });
				}
				else if (name[0] == '@') //If we somehow get text starting with @ but isn't a mention
				{
					string name2 = name.Substring(1);
					query = query.Concat(users.Where(x => string.Equals(x.Name, name2, StringComparison.OrdinalIgnoreCase)));
				}
			}

			if (discriminator != null)
				query = query.Where(x => x.Discriminator == discriminator.Value);
			return query;
		}

        public Task EditUser(User user, bool? isMuted = null, bool? isDeafened = null, Channel voiceChannel = null, IEnumerable<Role> roles = null)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (user.IsPrivate) throw new InvalidOperationException("Unable to edit users in a private channel");
            CheckReady();

            //Modify the roles collection and filter out the everyone role
            var roleIds = roles == null ? null : user.Roles.Where(x => !x.IsEveryone) .Select(x => x.Id);

            var request = new UpdateMemberRequest(user.Server.Id, user.Id)
            {
                IsMuted = isMuted ?? user.IsServerMuted,
                IsDeafened = isDeafened ?? user.IsServerDeafened,
                VoiceChannelId = voiceChannel?.Id,
                RoleIds = roleIds.ToArray()
            };
            return _clientRest.Send(request);
		}

		public Task KickUser(User user)
		{
			if (user == null) throw new ArgumentNullException(nameof(user));
			if (user.IsPrivate) throw new InvalidOperationException("Unable to kick users from a private channel");
			CheckReady();

            var request = new KickMemberRequest(user.Server.Id, user.Id);
            return _clientRest.Send(request);
		}
		public Task BanUser(User user, int pruneDays = 0)
		{
			if (user == null) throw new ArgumentNullException(nameof(user));
			if (user.IsPrivate) throw new InvalidOperationException("Unable to ban users from a private channel");
			CheckReady();

            var request = new AddGuildBanRequest(user.Server.Id, user.Id);
            request.PruneDays = pruneDays;
            return _clientRest.Send(request);
		}
		public async Task UnbanUser(Server server, ulong userId)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));
			CheckReady();

			try { await _clientRest.Send(new RemoveGuildBanRequest(server.Id, userId)).ConfigureAwait(false); }
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}

		public async Task<int> PruneUsers(Server server, int days, bool simulate = false)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (days <= 0) throw new ArgumentOutOfRangeException(nameof(days));
			CheckReady();

            var request = new PruneMembersRequest(server.Id)
            {
                Days = days,
                IsSimulation = simulate
            };
            var response = await _clientRest.Send(request).ConfigureAwait(false);
			return response.Pruned;
		}

		/// <summary>When Config.UseLargeThreshold is enabled, running this command will request the Discord server to provide you with all offline users for a particular server.</summary>
		public void RequestOfflineUsers(Server server)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));

			_webSocket.SendRequestMembers(server.Id, "", 0);
		}

        public async Task EditProfile(string currentPassword = "",
            string username = null, string email = null, string password = null,
            Stream avatar = null, ImageType avatarType = ImageType.Png)
        {
            if (currentPassword == null) throw new ArgumentNullException(nameof(currentPassword));
            CheckReady();

            var request = new UpdateProfileRequest()
            {
                CurrentPassword = currentPassword,
                Email = email ?? _currentUser?.Email,
                Password = password,
                Username = username ?? _privateUser?.Name,
                AvatarBase64 = Base64Image(avatarType, avatar, _privateUser?.AvatarId)
            };

            await _clientRest.Send(request).ConfigureAwait(false);

			if (password != null)
			{
                var loginRequest = new LoginRequest()
                {
                    Email = _currentUser.Email,
                    Password = password
                };
                var loginResponse = await _clientRest.Send(loginRequest).ConfigureAwait(false);
				_clientRest.SetToken(loginResponse.Token);
			}
		}

		public Task SetStatus(UserStatus status)
		{
			if (status == null) throw new ArgumentNullException(nameof(status));
			if (status != UserStatus.Online && status != UserStatus.Idle)
				throw new ArgumentException($"Invalid status, must be {UserStatus.Online} or {UserStatus.Idle}", nameof(status));
			CheckReady();
			
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
			_webSocket.SendUpdateStatus(_status == UserStatus.Idle ? EpochTime.GetMilliseconds() - (10 * 60 * 1000) : (long?)null, _gameId);
			return TaskHelper.CompletedTask;
		}
    }
}