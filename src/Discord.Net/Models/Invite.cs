using System;
using Discord.API;
using Newtonsoft.Json;

namespace Discord
{
	public sealed class Invite : CachedObject
	{		
		/// <summary> Returns, if enabled, an alternative human-readable code for URLs. </summary>
		public string XkcdCode { get; }
		/// <summary> Time (in seconds) until the invite expires. Set to 0 to never expire. </summary>
		public int MaxAge { get; private set; }
		/// <summary> The amount  of times this invite has been used. </summary>
		public int Uses { get; private set; }
		/// <summary> The max amount  of times this invite may be used. </summary>
		public int MaxUses { get; private set; }
		/// <summary> Returns true if this invite has been destroyed, or you are banned from that server. </summary>
		public bool IsRevoked { get; private set; }
		/// <summary> If true, a user accepting this invite will be kicked from the server after closing their client. </summary>
		public bool IsTemporary { get; private set; }

		/// <summary> Returns a URL for this invite using XkcdCode if available or Id if not. </summary>
		public string Url => API.Endpoints.InviteUrl(XkcdCode ?? Id);

		/// <summary> Returns the user that created this invite. </summary>
		[JsonIgnore]
		public User Inviter => _inviter.Value;
        private readonly Reference<User> _inviter;

		/// <summary> Returns the server this invite is to. </summary>
		[JsonIgnore]
		public Server Server => _server.Value;
		private readonly Reference<Server> _server;

		/// <summary> Returns the channel this invite is to. </summary>
		[JsonIgnore]
		public Channel Channel => _channel.Value;
		private readonly Reference<Channel> _channel;

		internal Invite(DiscordClient client, string code, string xkcdPass, string serverId, string inviterId, string channelId)
			: base(client, code)
		{
			XkcdCode = xkcdPass;
			_server = new Reference<Server>(serverId, x => _client.Servers[x] ?? new Server(client, x));
			_inviter = new Reference<User>(serverId, x => _client.Users[x, _server.Id] ?? new User(client, x, _server.Id));
			_channel = new Reference<Channel>(serverId, x => _client.Channels[x] ?? new Channel(client, x, _server.Id, null));
		}
		internal override void LoadReferences()
		{
			_server.Load();
			_inviter.Load();
			_channel.Load();
		}
		internal override void UnloadReferences() { }

		public override string ToString() => XkcdCode ?? Id;
		

		internal void Update(InviteInfo model)
		{
			if (model.IsRevoked != null)
				IsRevoked = model.IsRevoked.Value;
			if (model.IsTemporary != null)
				IsTemporary = model.IsTemporary.Value;
			if (model.MaxAge != null)
				MaxAge = model.MaxAge.Value;
			if (model.MaxUses != null)
				MaxUses = model.MaxUses.Value;
			if (model.Uses != null)
				Uses = model.Uses.Value;
        }
	}
}
