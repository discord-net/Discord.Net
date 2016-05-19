using Discord.API.Client;
using Discord.API.Client.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using APIMember = Discord.API.Client.Member;

namespace Discord
{
	public class User
    {
        private readonly static Action<User, User> _cloner = DynamicIL.CreateCopyMethod<User>();

        internal static string GetAvatarUrl(ulong userId, string avatarId) 
            => avatarId != null ? $"{DiscordConfig.ClientAPIUrl}users/{userId}/avatars/{avatarId}.jpg" : null;

        [Flags]
        private enum VoiceState : byte
        {
            None = 0x0,
            SelfMuted = 0x01,
            SelfDeafened = 0x02,
            ServerMuted = 0x04,
            ServerDeafened = 0x08,
            ServerSuppressed = 0x10,
        }

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
        
        private VoiceState _voiceState;
        private DateTime? _lastOnline;
        private ulong? _voiceChannelId;
        private Dictionary<ulong, Role> _roles;

        public DiscordClient Client { get; }

        /// <summary> Gets the unique identifier for this user. </summary>
        public ulong Id { get; }
        /// <summary> Gets the server this user is a member of. </summary>
        public Server Server { get; }

        /// <summary> Gets the name of this user. </summary>
        public string Name { get; private set; }
        /// <summary> Gets an id uniquely identifying from others with the same name. </summary>
        public ushort Discriminator { get; private set; }
        /// <summary> Gets a user's nickname in a server </summary>
        public string Nickname { get; internal set; }
        /// <summary> Gets the unique identifier for this user's current avatar. </summary>
        public string AvatarId { get; private set; }
        /// <summary> Gets the name of the game this user is currently playing. </summary>
        public Game? CurrentGame { get; internal set; }
        /// <summary> Determines whether this user is a Bot account. </summary>
        public bool IsBot { get; internal set; }
        /// <summary> Gets the current status for this user. </summary>
        public UserStatus Status { get; internal set; }
        /// <summary> Gets the datetime that this user joined this server. </summary>
        public DateTime JoinedAt { get; private set; }
        /// <summary> Returns the time this user last sent/edited a message, started typing or sent voice data in this server. </summary>
        public DateTime? LastActivityAt { get; private set; }
        // /// <summary> Gets this user's voice session id. </summary>
        // public string SessionId { get; private set; }
        // /// <summary> Gets this user's voice token. </summary>
        // public string Token { get; private set; }

        /// <summary> Gets the path to this object. </summary>
        internal string Path => $"{Server?.Name ?? "[Private]"}/{Name}";
        /// <summary> Gets the current private channel for this user if one exists. </summary>
        public Channel PrivateChannel => Client.GetPrivateChannel(Id);
        /// <summary> Returns the string used to mention this user. </summary>
        public string Mention => $"<@{Id}>";
        /// <summary> Returns the string used to mention this user by nickname. </summary>
        public string NicknameMention => $"<@!{Id}>";
        /// <summary> Returns true if this user has marked themselves as muted. </summary>
        public bool IsSelfMuted => (_voiceState & VoiceState.SelfMuted) != 0;
        /// <summary> Returns true if this user has marked themselves as deafened. </summary>
        public bool IsSelfDeafened => (_voiceState & VoiceState.SelfDeafened) != 0;
        /// <summary> Returns true if the server is blocking audio from this user. </summary>
        public bool IsServerMuted => (_voiceState & VoiceState.ServerMuted) != 0;
        /// <summary> Returns true if the server is blocking audio to this user. </summary>
        public bool IsServerDeafened => (_voiceState & VoiceState.ServerDeafened) != 0;
        /// <summary> Returns true if the server is temporarily blocking audio to/from this user. </summary>
        public bool IsServerSuppressed => (_voiceState & VoiceState.ServerSuppressed) != 0;
		/// <summary> Returns the time this user was last seen online in this server. </summary>
		public DateTime? LastOnlineAt => Status != UserStatus.Offline ? DateTime.UtcNow : _lastOnline; 
        /// <summary> Gets this user's current voice channel. </summary>
        public Channel VoiceChannel => _voiceChannelId != null ? Server.GetChannel(_voiceChannelId.Value) : null;
        /// <summary> Gets the URL to this user's current avatar. </summary>
        public string AvatarUrl => GetAvatarUrl(Id, AvatarId);
        /// <summary> Gets all roles that have been assigned to this user, including the everyone role. </summary>
        public IEnumerable<Role> Roles => _roles.Select(x => x.Value);

        /// <summary> Returns a collection of all channels this user has permissions to join on this server. </summary>
        public IEnumerable<Channel> Channels
		{
			get
			{
				if (Server != null)
				{
                    if (Client.Config.UsePermissionsCache)
                    {
                        return Server.AllChannels.Where(x => 
                            (x.Type == ChannelType.Text && x.GetPermissions(this).ReadMessages) ||
                            (x.Type == ChannelType.Voice && x.GetPermissions(this).Connect));
                    }
                    else
                    {
                        ChannelPermissions perms = new ChannelPermissions();
                        return Server.AllChannels
                            .Where(x =>
                            {
                                x.UpdatePermissions(this, ref perms);
                                return (x.Type == ChannelType.Text && perms.ReadMessages) ||
                                       (x.Type == ChannelType.Voice && perms.Connect);
                            });
                    }
				}
				else
				{
                    if (this == Client.PrivateUser)
                        return Client.PrivateChannels;
                    else
                    {
                        var privateChannel = Client.GetPrivateChannel(Id);
                        if (privateChannel != null)
                            return new Channel[] { privateChannel };
                        else
                            return new Channel[0];
                    }
				}
			}
		}

		internal User(DiscordClient client, ulong id, Server server)
		{
            Client = client;
            Id = id;
            Server = server;

			_roles = new Dictionary<ulong, Role>();
			Status = UserStatus.Offline;

			if (server == null)
				UpdateRoles(null);
		}

		internal void Update(UserReference model)
		{
			if (model.Username != null)
				Name = model.Username;
			if (model.Discriminator != null)
				Discriminator = model.Discriminator.Value;
			if (model.Avatar != null)
				AvatarId = model.Avatar;
            if (model.Bot != null)
                IsBot = model.Bot.Value;

		}
		internal void Update(APIMember model)
		{
			if (model.User != null)
				Update(model.User);

			if (model.JoinedAt.HasValue)
				JoinedAt = model.JoinedAt.Value;
			if (model.Roles != null)
				UpdateRoles(model.Roles.Select(x => Server.GetRole(x)));
            if (model.Nick != "")
                Nickname = model.Nick;
        }
		internal void Update(ExtendedMember model)
		{
			Update(model as APIMember);
            
            if (model.IsServerMuted == true)
                _voiceState |= VoiceState.ServerMuted;
            else if (model.IsServerMuted == false)
                _voiceState &= ~VoiceState.ServerMuted;

            if (model.IsServerDeafened == true)
                _voiceState |= VoiceState.ServerDeafened;
            else if (model.IsServerDeafened == false)
                _voiceState &= ~VoiceState.ServerDeafened;
        }
		internal void Update(MemberPresence model)
		{
			if (model.User != null)
				Update(model.User as UserReference);

            if (model.Roles != null)
                UpdateRoles(model.Roles.Select(x => Server.GetRole(x)));
			if (model.Status != null && Status != model.Status)
			{
				Status = UserStatus.FromString(model.Status);
				if (Status == UserStatus.Offline)
					_lastOnline = DateTime.UtcNow;
			}

            if (model.Game != null)
                CurrentGame = new Game(model.Game.Name, model.Game.Type, model.Game.Url);
            else
                CurrentGame = null;
		}
		internal void Update(MemberVoiceState model)
        {
            if (model.IsSelfMuted == true)
                _voiceState |= VoiceState.SelfMuted;
            else if (model.IsSelfMuted == false)
                _voiceState &= ~VoiceState.SelfMuted;
            if (model.IsSelfDeafened == true)
                _voiceState |= VoiceState.SelfDeafened;
            else if (model.IsSelfDeafened == false)
                _voiceState &= ~VoiceState.SelfDeafened;
            if (model.IsServerMuted == true)
                _voiceState |= VoiceState.ServerMuted;
            else if (model.IsServerMuted == false)
                _voiceState &= ~VoiceState.ServerMuted;
            if (model.IsServerDeafened == true)
                _voiceState |= VoiceState.ServerDeafened;
            else if (model.IsServerDeafened == false)
                _voiceState &= ~VoiceState.ServerDeafened;
            if (model.IsServerSuppressed == true)
                _voiceState |= VoiceState.ServerSuppressed;
            else if (model.IsServerSuppressed == false)
                _voiceState &= ~VoiceState.ServerSuppressed;
            
            /*if (model.SessionId != null)
				SessionId = model.SessionId;
			if (model.Token != null)
				Token = model.Token;*/			
			
			_voiceChannelId = model.ChannelId; //Allows null
		}

		internal void UpdateActivity(DateTime? activity = null)
		{
			if (LastActivityAt == null || activity > LastActivityAt.Value)
				LastActivityAt = activity ?? DateTime.UtcNow;
		}

        public async Task Edit(bool? isMuted = null, bool? isDeafened = null, Channel voiceChannel = null, IEnumerable<Role> roles = null, string nickname = "")
        {
            if (Server == null) throw new InvalidOperationException("Unable to edit users in a private channel");

            //Modify the roles collection and filter out the everyone role
            var roleIds = (roles ?? Roles)
                .Where(x => !x.IsEveryone)
                .Select(x => x.Id)
                .Distinct()
                .ToArray();

            bool isCurrentUser = Id == Server.CurrentUser.Id;
            if (isCurrentUser && nickname != "")
            {
                var request = new UpdateOwnNick(Server.Id, nickname ?? "");
                await Client.ClientAPI.Send(request).ConfigureAwait(false);
                nickname = "";
            }
            if (!isCurrentUser || isMuted != null || isDeafened != null | voiceChannel != null || roles != null)
            {
                if (nickname == "") nickname = Nickname;
                var request = new UpdateMemberRequest(Server.Id, Id)
                {
                    IsMuted = isMuted ?? IsServerMuted,
                    IsDeafened = isDeafened ?? IsServerDeafened,
                    VoiceChannelId = voiceChannel?.Id,
                    RoleIds = roleIds,
                    Nickname = nickname ?? ""
                };
                await Client.ClientAPI.Send(request).ConfigureAwait(false);
            }
        }
        
        public Task Kick()
        {
            if (Server == null) throw new InvalidOperationException("Unable to kick users from a private channel");

            var request = new KickMemberRequest(Server.Id, Id);
            return Client.ClientAPI.Send(request);
        }

        #region Permissions
        public ServerPermissions ServerPermissions
        {
            get
            {
                if (Server == null) throw new InvalidOperationException("Unable to get server permissions from a private channel");

                return Server.GetPermissions(this);
            }
        }

		public ChannelPermissions GetPermissions(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			return channel.GetPermissions(this);
		}
        #endregion

        #region Channels
        public Task<Channel> CreatePMChannel()
            => Client.CreatePMChannel(this);
        #endregion

        #region Messages
        public async Task<Message> SendMessage(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            var channel = await CreatePMChannel().ConfigureAwait(false);
            return await channel.SendMessage(text).ConfigureAwait(false);
        }
        public async Task<Message> SendFile(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));

            var channel = await CreatePMChannel().ConfigureAwait(false);
            return await channel.SendFile(filePath).ConfigureAwait(false);
        }
        public async Task<Message> SendFile(string filename, Stream stream)
        {
            if (filename == null) throw new ArgumentNullException(nameof(filename));
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            var channel = await CreatePMChannel().ConfigureAwait(false);
            return await channel.SendFile(filename, stream).ConfigureAwait(false);
        }
        #endregion

        #region Roles
        private void UpdateRoles(IEnumerable<Role> roles)
        {
            bool updated = false;
            var newRoles = new Dictionary<ulong, Role>();

            var oldRoles = _roles;
            if (roles != null)
            {
                foreach (var r in roles)
                {
                    if (r != null)
                    {
                        newRoles[r.Id] = r;
                        if (!oldRoles.ContainsKey(r.Id))
                            updated = true; //Check for adds
                    }
                }
            }

            if (Server != null)
            {
                var everyone = Server.EveryoneRole;
                newRoles[everyone.Id] = everyone;
            }
            if (oldRoles.Count != newRoles.Count)
                updated = true; //Check for removes

            if (updated)
            {
                _roles = newRoles;
                if (Server != null)
                    Server.UpdatePermissions(this);
            }
        }
        public bool HasRole(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            return _roles.ContainsKey(role.Id);
        }

        public Task AddRoles(params Role[] roles)
            => Edit(roles: Roles.Concat(roles));
        public Task RemoveRoles(params Role[] roles)
            => Edit(roles: Roles.Except(roles));
        #endregion

        internal User Clone()
        {
            var result = new User();
            _cloner(this, result);
            return result;
        }
        private User() { } //Used for cloning

        public override string ToString() => Name != null ? $"{Name}#{Discriminator}" : Id.ToIdString();
	}
}