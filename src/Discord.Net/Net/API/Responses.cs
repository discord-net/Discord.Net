//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;
using System;

namespace Discord.Net.API
{
    internal static class Responses
	{
		//Auth
		public sealed class Gateway
		{
			[JsonProperty(PropertyName = "url")]
			public string Url;
		}
		public sealed class AuthFingerprint
		{
			[JsonProperty(PropertyName = "fingerprint")]
			public string Fingerprint;
		}
		public sealed class AuthRegister
		{
			[JsonProperty(PropertyName = "token")]
			public string Token;
		}
		public sealed class AuthLogin
		{
			[JsonProperty(PropertyName = "token")]
			public string Token;
		}

		//Users
		public sealed class ChangeProfile : SelfUserInfo { }

		//Servers
		public sealed class CreateServer : GuildInfo { }
		public sealed class DeleteServer : GuildInfo { }

		//Channels
		public sealed class CreateChannel : ChannelInfo { }
		public sealed class DestroyChannel : ChannelInfo { }

		//Invites
		public sealed class CreateInvite : ExtendedInvite { }
		public sealed class GetInvite : Invite { }
		public sealed class AcceptInvite : Invite { }

		//Messages
		public sealed class SendMessage : Message { }
		public sealed class EditMessage : Message { }
		public sealed class GetMessages : Message { }

		//Voice
		public sealed class GetRegions
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
		public sealed class GetIce
		{
			[JsonProperty(PropertyName = "ttl")]
			public string TTL;
			[JsonProperty(PropertyName = "servers")]
			public Server[] Servers;

			public sealed class Server
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
