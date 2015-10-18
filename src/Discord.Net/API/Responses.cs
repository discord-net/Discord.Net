//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Discord.API
{
	//Auth
	public sealed class GatewayResponse
	{
		[JsonProperty("url")]
		public string Url;
	}
	public sealed class LoginResponse
	{
		[JsonProperty("token")]
		public string Token;
	}

	//Channels
	public sealed class CreateChannelResponse : ChannelInfo { }
	public sealed class DestroyChannelResponse : ChannelInfo { }
	public sealed class EditChannelResponse : ChannelInfo { }

	//Invites
	public sealed class CreateInviteResponse : ExtendedInvite { }
	public sealed class GetInviteResponse : Invite { }
	public sealed class AcceptInviteResponse : Invite { }

	//Messages
	public sealed class SendMessageResponse : Message { }
	public sealed class EditMessageResponse : Message { }
	public sealed class GetMessagesResponse : List<Message> { }

	//Profile
	public sealed class EditProfileResponse : SelfUserInfo { }

	//Roles
	public sealed class CreateRoleResponse : RoleInfo { }
	public sealed class EditRoleResponse : RoleInfo { }
	
	//Servers
	public sealed class CreateServerResponse : GuildInfo { }
	public sealed class DeleteServerResponse : GuildInfo { }
	public sealed class EditServerResponse : GuildInfo { }

	//Voice
	public sealed class GetRegionsResponse : List<GetRegionsResponse.RegionData>
	{
		public sealed class RegionData
		{
			[JsonProperty("sample_hostname")]
			public string Hostname;
			[JsonProperty("sample_port")]
			public int Port;
			[JsonProperty("id")]
			public string Id;
			[JsonProperty("name")]
			public string Name;
		}
	}
	public sealed class GetIceResponse
	{
		[JsonProperty("ttl")]
		public string TTL;
		[JsonProperty("servers")]
		public ServerData[] Servers;

		public sealed class ServerData
		{
			[JsonProperty("url")]
			public string URL;
			[JsonProperty("username")]
			public string Username;
			[JsonProperty("credential")]
			public string Credential;
		}
	}

	public sealed class GetIncidentsResponse
	{
		[JsonProperty("page")]
		public PageData Page;
		[JsonProperty("scheduled_maintenances")]
		public MaintenanceData[] ScheduledMaintenances;

		public sealed class PageData
		{
			[JsonProperty("id")]
			public string Id;
			[JsonProperty("name")]
			public string Name;
			[JsonProperty("url")]
			public string Url;
			[JsonProperty("updated-at")]
			public DateTime? UpdatedAt;
		}

		public sealed class MaintenanceData
		{
		}
	}
}
