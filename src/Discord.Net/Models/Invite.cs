using Discord.API.Client;
using Discord.API.Client.Rest;
using Discord.Net;
using System;
using System.Net;
using System.Threading.Tasks;
using APIInvite = Discord.API.Client.Invite;

namespace Discord
{
	public class Invite
    {
        private readonly static Action<Invite, Invite> _cloner = DynamicIL.CreateCopyMethod<Invite>();

        public class ServerInfo
		{
			/// <summary> Returns the unique identifier of this server. </summary>
			public ulong Id { get; }
			/// <summary> Returns the name of this server. </summary>
			public string Name { get; }

			internal ServerInfo(ulong id, string name)
			{
				Id = id;
				Name = name;
			}
		}
		public class ChannelInfo
		{
			/// <summary> Returns the unique identifier of this channel. </summary>
			public ulong Id { get; }
			/// <summary> Returns the name of this channel. </summary>
			public string Name { get; }

			internal ChannelInfo(ulong id, string name)
			{
				Id = id;
				Name = name;
			}
		}
		public class InviterInfo
		{
			/// <summary> Returns the unique identifier for this user. </summary>
			public ulong Id { get; }
			/// <summary> Returns the name of this user. </summary>
			public string Name { get; }
			/// <summary> Returns the by-name unique identifier for this user. </summary>
			public ushort Discriminator { get; }
			/// <summary> Returns the unique identifier for this user's avatar. </summary>
			public string AvatarId { get; }

			/// <summary> Returns the full path to this user's avatar. </summary>
			public string AvatarUrl => User.GetAvatarUrl(Id, AvatarId);

			internal InviterInfo(ulong id, string name, ushort discriminator, string avatarId)
			{
				Id = id;
				Name = name;
				Discriminator = discriminator;
				AvatarId = avatarId;
			}
		}

        public DiscordClient Client { get; }

        /// <summary> Gets the unique code for this invite. </summary>
        public string Code { get; }
        /// <summary> Gets, if enabled, an alternative human-readable invite code. </summary>
        public string XkcdCode { get; }

        /// <summary> Gets information about the server this invite is attached to. </summary>
        public ServerInfo Server { get; private set; }
        /// <summary> Gets information about the channel this invite is attached to. </summary>
        public ChannelInfo Channel { get; private set; }
        /// <summary> Gets the time (in seconds) until the invite expires. </summary>
        public int? MaxAge { get; private set; }
		/// <summary> Gets the amount of times this invite has been used. </summary>
		public int Uses { get; private set; }
		/// <summary> Gets the max amount of times this invite may be used. </summary>
		public int? MaxUses { get; private set; }
		/// <summary> Returns true if this invite has expired, been destroyed, or you are banned from that server. </summary>
		public bool IsRevoked { get; private set; }
		/// <summary> If true, a user accepting this invite will be kicked from the server after closing their client. </summary>
		public bool IsTemporary { get; private set; }
        /// <summary> Gets when this invite was created. </summary>
		public DateTime CreatedAt { get; private set; }

        /// <summary> Gets the path to this object. </summary>
        internal string Path => $"{Server?.Name ?? "[Private]"}/{Code}";
        /// <summary> Returns a URL for this invite using XkcdCode if available or Id if not. </summary>
        public string Url => $"{DiscordConfig.InviteUrl}/{Code}";

        internal Invite(DiscordClient client, string code, string xkcdPass)
		{
            Client = client;
            Code = code;
			XkcdCode = xkcdPass;
		}

		internal void Update(InviteReference model)
		{
			if (model.Guild != null)
				Server = new ServerInfo(model.Guild.Id, model.Guild.Name);
			if (model.Channel != null)
				Channel = new ChannelInfo(model.Channel.Id, model.Channel.Name);
        }
        internal void Update(APIInvite model)
		{
			Update(model as InviteReference);

			if (model.IsRevoked != null)
				IsRevoked = model.IsRevoked.Value;
			if (model.IsTemporary != null)
				IsTemporary = model.IsTemporary.Value;
			if (model.MaxAge != null)
				MaxAge = model.MaxAge.Value != 0 ? model.MaxAge.Value : (int?)null;
			if (model.MaxUses != null)
				MaxUses = model.MaxUses.Value;
			if (model.Uses != null)
				Uses = model.Uses.Value;
			if (model.CreatedAt != null)
				CreatedAt = model.CreatedAt.Value;
		}
        
        public async Task Delete()
        {
            try { await Client.ClientAPI.Send(new DeleteInviteRequest(Code)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }        
        public Task Accept() 
            => Client.ClientAPI.Send(new AcceptInviteRequest(Code));

        internal Invite Clone()
        {
            var result = new Invite();
            _cloner(this, result);
            return result;
        }
        private Invite() { } //Used for cloning

        public override string ToString() => XkcdCode ?? Code;
	}
}
