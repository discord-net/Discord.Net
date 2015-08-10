//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;
using System;

namespace Discord.API.Models
{

	internal static class APIResponses
	{
		public class AuthFingerprint
		{
			[JsonProperty(PropertyName = "fingerprint")]
			public string Fingerprint;
		}
		public class AuthRegister : AuthLogin { }
		public class AuthLogin
		{
			[JsonProperty(PropertyName = "token")]
			public string Token;
		}

		public class CreateServer : ServerInfo { }
		public class DeleteServer : ServerInfo { }

		public class CreateInvite : GetInvite
		{
			[JsonProperty(PropertyName = "max_age")]
			public int MaxAge;
			[JsonProperty(PropertyName = "max_uses")]
			public int MaxUses;
			[JsonProperty(PropertyName = "revoked")]
			public bool IsRevoked;
			[JsonProperty(PropertyName = "temporary")]
			public bool IsTemporary;
			[JsonProperty(PropertyName = "uses")]
			public int Uses;
			[JsonProperty(PropertyName = "created_at")]
			public DateTime CreatedAt;
		}

		public class GetInvite
		{
			[JsonProperty(PropertyName = "inviter")]
			public UserReference Inviter;
			[JsonProperty(PropertyName = "guild")]
			public ServerReference Server;
			[JsonProperty(PropertyName = "channel")]
			public ChannelReference Channel;
			[JsonProperty(PropertyName = "code")]
			public string Code;
			[JsonProperty(PropertyName = "xkcdpass")]
			public string XkcdPass;
		}
		public class AcceptInvite : GetInvite { }

		public class SendMessage : Message { }
		public class GetMessages : Message { }

		public class GetRegions
		{
			[JsonProperty(PropertyName = "sample_hostname")]
			public string Hostname;
			[JsonProperty(PropertyName = "sample_port")]
			public int Port;
			[JsonProperty(PropertyName = "id")]
			public string Id;
			[JsonProperty(PropertyName = "name")]
			public string Name;
		}

		public class GetIce
		{
			[JsonProperty(PropertyName = "ttl")]
			public string TTL;
			[JsonProperty(PropertyName = "servers")]
			public Server[] Servers;

			public class Server
			{
				[JsonProperty(PropertyName = "url")]
				public string URL;
				[JsonProperty(PropertyName = "username")]
				public string Username;
				[JsonProperty(PropertyName = "credential")]
				public string Credential;
			}
		}
	}
}
