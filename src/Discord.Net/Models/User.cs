using Discord.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public class User : CachedObject<ulong>
	{
		internal struct CompositeKey : IEquatable<CompositeKey>
		{
			public ulong ServerId, UserId;
			public CompositeKey(ulong userId, ulong? serverId)
			{
				ServerId = serverId ?? 0;
				UserId = userId;
			}

			public bool Equals(CompositeKey other)
				=> UserId == other.UserId && ServerId == other.ServerId;
			public override int GetHashCode()
				=> unchecked(ServerId.GetHashCode() + UserId.GetHashCode() + 23);
		}

		/// <summary> Returns a unique identifier combining this user's id with its server's. </summary>
		internal CompositeKey UniqueId => new CompositeKey(_server.Id ?? 0, Id);
		/// <summary> Returns the name of this user on this server. </summary>
		public string Name { get; private set; }
		/// <summary> Returns a by-name unique identifier separating this user from others with the same name. </summary>
		public ushort Discriminator { get; private set; }
		/// <summary> Returns the unique identifier for this user's current avatar. </summary>
		public string AvatarId { get; private set; }
		/// <summary> Returns the URL to this user's current avatar. </summary>
		public string AvatarUrl => AvatarId != null ? Endpoints.UserAvatar(Id, AvatarId) : null;
		/// <summary> Returns the datetime that this user joined this server. </summary>
		public DateTime JoinedAt { get; private set; }

		public bool IsSelfMuted { get; private set; }
		public bool IsSelfDeafened { get; private set; }
		public bool IsServerMuted { get; private set; }
		public bool IsServerDeafened { get; private set; }
		public bool IsServerSuppressed { get; private set; }
		public bool IsPrivate => _server.Id == null;
        public bool IsOwner => _server.Value.OwnerId == Id;

		public string SessionId { get; private set; }
		public string Token { get; private set; }

		/// <summary> Returns the id for the game this user is currently playing. </summary>
		public int? GameId { get; private set; }
		/// <summary> Returns the current status for this user. </summary>
		public UserStatus Status { get; private set; }
		/// <summary> Returns the time this user last sent/edited a message, started typing or sent voice data in this server. </summary>
		public DateTime? LastActivityAt { get; private set; }
		/// <summary> Returns the time this user was last seen online in this server. </summary>
		public DateTime? LastOnlineAt => Status != UserStatus.Offline ? DateTime.UtcNow : _lastOnline;
		private DateTime? _lastOnline;

		//References
		[JsonIgnore]
		public GlobalUser Global => _globalUser.Value;
		private readonly Reference<GlobalUser> _globalUser;

		[JsonIgnore]
		public Server Server => _server.Value;
        private readonly Reference<Server> _server;
		[JsonProperty]
		private ulong? ServerId { get { return _server.Id; } set { _server.Id = value; } }

		[JsonIgnore]
		public Channel VoiceChannel => _voiceChannel.Value;
		private Reference<Channel> _voiceChannel;
		[JsonProperty]
		private ulong? VoiceChannelId { get { return _voiceChannel.Id; } set { _voiceChannel.Id = value; } }

		//Collections
		[JsonIgnore]
		public IEnumerable<Role> Roles => _roles.Select(x => x.Value);
		private Dictionary<ulong, Role> _roles;
		[JsonProperty]
		private IEnumerable<ulong> RoleIds => _roles.Select(x => x.Key);

		/// <summary> Returns a collection of all messages this user has sent on this server that are still in cache. </summary>
		[JsonIgnore]
		public IEnumerable<Message> Messages
		{
			get
			{
				if (_server.Id != null)
					return Server.Channels.SelectMany(x => x.Messages.Where(y => y.User.Id == Id));
				else
					return Global.PrivateChannel.Messages.Where(x => x.User.Id == Id);
            }
		}

		/// <summary> Returns a collection of all channels this user has permissions to join on this server. </summary>
		[JsonIgnore]
		public IEnumerable<Channel> Channels
		{
			get
			{
				if (_server.Id != null)
				{
                    if (_client.Config.UsePermissionsCache)
                    {
                        return Server.Channels
                            .Where(x => (x.Type == ChannelType.Text && x.GetPermissions(this).ReadMessages) ||
                            (x.Type == ChannelType.Voice && x.GetPermissions(this).Connect));
                    }
                    else
                    {
                        ChannelPermissions perms = new ChannelPermissions();
                        return Server.Channels
                            .Where(x =>
                            {
                                x.UpdatePermissions(this, perms);
                                return (x.Type == ChannelType.Text && perms.ReadMessages) ||
                                        (x.Type == ChannelType.Voice && perms.Connect);
                            });
                    }
				}
				else
				{
					var privateChannel = Global.PrivateChannel;
					if (privateChannel != null)
						return new Channel[] { privateChannel };
					else
						return new Channel[0];
				}
			}
		}

		/// <summary> Returns the string used to mention this user. </summary>
		public string Mention => $"<@{Id}>";

		internal User(DiscordClient client, ulong id, ulong? serverId)
			: base(client, id)
		{
			_globalUser = new Reference<GlobalUser>(id, 
				x => _client.GlobalUsers.GetOrAdd(x), 
				x => x.AddUser(this), 
				x => x.RemoveUser(this));
			_server = new Reference<Server>(serverId, 
				x => _client.Servers[x], 
				x =>
				{
					x.AddMember(this);
					if (Id == _client.CurrentUser.Id)
						x.CurrentUser = this;
                }, 
				x =>
				{
					x.RemoveMember(this);
					if (Id == _client.CurrentUser.Id)
						x.CurrentUser = null;
				});
			_voiceChannel = new Reference<Channel>(x => _client.Channels[x]);
			_roles = new Dictionary<ulong, Role>();

			Status = UserStatus.Offline;

			if (serverId == null)
				UpdateRoles(null);
		}
		internal override bool LoadReferences()
		{
			return _globalUser.Load() && 
				(IsPrivate || _server.Load());
		}
		internal override void UnloadReferences()
		{
			_globalUser.Unload();
			_server.Unload();
		}

		internal void Update(UserReference model)
		{
			if (model.Username != null)
				Name = model.Username;
			if (model.Discriminator != null)
				Discriminator = model.Discriminator.Value;
			if (model.Avatar != null)
				AvatarId = model.Avatar;
		}
		internal void Update(MemberInfo model)
		{
			if (model.User != null)
				Update(model.User);

			if (model.JoinedAt.HasValue)
				JoinedAt = model.JoinedAt.Value;
			if (model.Roles != null)
				UpdateRoles(model.Roles.Select(x => _client.Roles[x]));
        }
		internal void Update(ExtendedMemberInfo model)
		{
			Update(model as API.MemberInfo);

			if (model.IsServerDeafened != null)
				IsServerDeafened = model.IsServerDeafened.Value;
			if (model.IsServerMuted != null)
				IsServerMuted = model.IsServerMuted.Value;
		}
		internal void Update(PresenceInfo model)
		{
			if (model.User != null)
				Update(model.User as UserReference);

			if (model.Roles != null)
				UpdateRoles(model.Roles.Select(x => _client.Roles[x]));
			if (model.Status != null && Status != model.Status)
			{
				Status = UserStatus.FromString(model.Status);
				if (Status == UserStatus.Offline)
					_lastOnline = DateTime.UtcNow;
			}
			
			GameId = model.GameId; //Allows null
		}
		internal void Update(VoiceMemberInfo model)
		{
			if (model.IsServerDeafened != null)
				IsServerDeafened = model.IsServerDeafened.Value;
			if (model.IsServerMuted != null)
				IsServerMuted = model.IsServerMuted.Value;
			if (model.SessionId != null)
				SessionId = model.SessionId;
			if (model.Token != null)
				Token = model.Token;
			
			if (model.IsSelfDeafened != null)
				IsSelfDeafened = model.IsSelfDeafened.Value;
			if (model.IsSelfMuted != null)
				IsSelfMuted = model.IsSelfMuted.Value;
			if (model.IsServerSuppressed != null)
				IsServerSuppressed = model.IsServerSuppressed.Value;
			
			_voiceChannel.Id = model.ChannelId; //Allows null
		}
		private void UpdateRoles(IEnumerable<Role> roles)
		{
			var newRoles = new Dictionary<ulong, Role>();
			if (roles != null)
			{
				foreach (var r in roles)
					newRoles[r.Id] = r;
			}

			if (_server.Id != null)
			{
				var everyone = Server.EveryoneRole;
				newRoles.Add(everyone.Id, everyone);
			}
			_roles = newRoles;

			if (!IsPrivate)
				Server.UpdatePermissions(this);
		}

		internal void UpdateActivity(DateTime? activity = null)
		{
			if (LastActivityAt == null || activity > LastActivityAt.Value)
				LastActivityAt = activity ?? DateTime.UtcNow;
		}
		
		public ServerPermissions ServerPermissions => Server.GetPermissions(this);
		public ChannelPermissions GetPermissions(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));

			return channel.GetPermissions(this);
		}

		public bool HasRole(Role role)
		{
			if (role == null) throw new ArgumentNullException(nameof(role));
			
			return _roles.ContainsKey(role.Id);
		}

		public override bool Equals(object obj) => obj is User && (obj as User).Id == Id;
		public override int GetHashCode() => unchecked(Id.GetHashCode() + 7230);
		public override string ToString() => Name != null ? $"{Name}#{Discriminator}" : IdConvert.ToString(Id);
	}
}