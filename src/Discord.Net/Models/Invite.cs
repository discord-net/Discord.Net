using System;
using Discord.API;
using Newtonsoft.Json;

namespace Discord
{
	public sealed class Invite : CachedObject
	{
		private readonly string _serverId;
        private string _inviterId, _channelId;
		
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
		/// <summary> Returns, if enabled, an alternative human-readable code for URLs. </summary>
		public string XkcdPass { get; }

		/// <summary> Returns a URL for this invite using XkcdPass if available or Id if not. </summary>
		public string Url => API.Endpoints.InviteUrl(XkcdPass ?? Id);
		
		/// <summary> Returns the user that created this invite. </summary>
		[JsonIgnore]
		public User Inviter => _client.Users[_inviterId, _serverId];
		
		/// <summary> Returns the server this invite is to. </summary>
		[JsonIgnore]
		public Server Server => _client.Servers[_serverId];
		
		/// <summary> Returns the channel this invite is to. </summary>
		[JsonIgnore]
		public Channel Channel => _client.Channels[_channelId];

		internal Invite(DiscordClient client, string code, string xkcdPass, string serverId)
			: base(client, code)
		{
			XkcdPass = xkcdPass;
			_serverId = serverId;
		}
		internal override void OnCached() { }
		internal override void OnUncached() { }

		public override string ToString() => XkcdPass ?? Id;


		internal void Update(InviteReference model)
		{
			if (model.Channel != null)
				_channelId = model.Channel.Id;
			if (model.Inviter != null)
				_inviterId = model.Inviter.Id;
		}

		internal void Update(InviteInfo model)
		{
			Update(model as InviteReference);
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
