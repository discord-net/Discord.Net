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
		public User Inviter { get; private set; }
		[JsonProperty("InviterId")]
		private readonly string _inviterId;

		/// <summary> Returns the server this invite is to. </summary>
		[JsonIgnore]
		public Server Server { get; private set; }
		[JsonProperty("ServerId")]
		private readonly string _serverId;

		/// <summary> Returns the channel this invite is to. </summary>
		[JsonIgnore]
		public Channel Channel { get; private set; }
		[JsonProperty("ChannelId")]
		private readonly string _channelId;

		internal Invite(DiscordClient client, string code, string xkcdPass, string serverId, string inviterId, string channelId)
			: base(client, code)
		{
			XkcdCode = xkcdPass;
			_serverId = serverId;
			_inviterId = inviterId;
			_channelId = channelId;
		}

		internal override void OnCached()
		{
			var server = _client.Servers[_serverId];
			if (server == null)
				server = new Server(_client, _serverId);
			Server = server;

			if (_inviterId != null)
			{
				var inviter = _client.Users[_inviterId, _serverId];
				if (inviter == null)
					inviter = new User(_client, _inviterId, _serverId);
				Inviter = inviter;
			}

			if (_channelId != null)
			{
				var channel = _client.Channels[_channelId];
				if (channel == null)
					channel = new Channel(_client, _channelId, _serverId, null);
				Channel = channel;
			}
		}
		internal override void OnUncached()
		{
			Server = null;
			Inviter = null;
			Channel = null;
		}

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
