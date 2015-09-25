using Newtonsoft.Json;

namespace Discord
{
	public sealed class Invite
	{
		private readonly DiscordClient _client;

		/// <summary> Returns the unique identifier for this invite. </summary>
		public string Id { get; }

		/// <summary> Time (in seconds) until the invite expires. Set to 0 to never expire. </summary>
		public int MaxAge { get; internal set; }
		/// <summary> The amount  of times this invite has been used. </summary>
		public int Uses { get; internal set; }
		/// <summary> The max amount  of times this invite may be used. </summary>
		public int MaxUses { get; internal set; }
		/// <summary> Returns true if this invite has been destroyed, or you are banned from that server. </summary>
		public bool IsRevoked { get; internal set; }
		/// <summary> If true, a user accepting this invite will be kicked from the server after closing their client. </summary>
		public bool IsTemporary { get; internal set; }
		/// <summary> Returns, if enabled, an alternative human-readable code for URLs. </summary>
		public string XkcdPass { get; }

		/// <summary> Returns a URL for this invite using XkcdPass if available or Id if not. </summary>
		public string Url => API.Endpoints.InviteUrl(XkcdPass ?? Id);

		/// <summary> Returns the id of the user that created this invite. </summary>
		public string InviterId { get; internal set; }
		/// <summary> Returns the user that created this invite. </summary>
		[JsonIgnore]
		public User Inviter => _client.Users[InviterId];

		/// <summary> Returns the id of the server this invite is to. </summary>
		public string ServerId { get; internal set; }
		/// <summary> Returns the server this invite is to. </summary>
		[JsonIgnore]
		public Server Server => _client.Servers[ServerId];

		/// <summary> Returns the id of the channel this invite is to. </summary>
		public string ChannelId { get; internal set; }
		/// <summary> Returns the channel this invite is to. </summary>
		[JsonIgnore]
		public Channel Channel => _client.Channels[ChannelId];

		internal Invite(DiscordClient client, string code, string xkcdPass, string serverId)
		{
			_client = client;
			Id = code;
			XkcdPass = xkcdPass;
			ServerId = serverId;
		}

		public override string ToString() => XkcdPass ?? Id;

		internal void Update(API.Invite model)
		{
			ChannelId = model.Channel.Id;
			InviterId = model.Inviter?.Id;
			ServerId = model.Guild.Id;
		}

		internal void Update(API.ExtendedInvite model)
		{
			Update(model as API.Invite);
            IsRevoked = model.IsRevoked;
			IsTemporary = model.IsTemporary;
			MaxAge = model.MaxAge;
			MaxUses = model.MaxUses;
			Uses = model.Uses;
        }
	}
}
