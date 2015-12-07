//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Discord.API
{
	//Common
	public class InviteReference
	{
		[JsonProperty("inviter")]
		public UserReference Inviter;
		[JsonProperty("guild")]
		public GuildReference Guild;
		[JsonProperty("channel")]
		public ChannelReference Channel;
		[JsonProperty("code")]
		public string Code;
		[JsonProperty("xkcdpass")]
		public string XkcdPass;
	}
	public class InviteInfo : InviteReference
	{
		[JsonProperty("max_age")]
		public int? MaxAge;
		[JsonProperty("max_uses")]
		public int? MaxUses;
		[JsonProperty("revoked")]
		public bool? IsRevoked;
		[JsonProperty("temporary")]
		public bool? IsTemporary;
		[JsonProperty("uses")]
		public int? Uses;
		[JsonProperty("created_at")]
		public DateTime? CreatedAt;
	}

	//Create
	public class CreateInviteRequest
	{
		[JsonProperty("max_age")]
		public int MaxAge;
		[JsonProperty("max_uses")]
		public int MaxUses;
		[JsonProperty("temporary")]
		public bool IsTemporary;
		[JsonProperty("xkcdpass")]
		public bool WithXkcdPass;
	}
	public class CreateInviteResponse : InviteInfo { }

	//Get
	public class GetInviteResponse : InviteReference { }
	public class GetInvitesResponse : List<InviteReference> { }

	//Accept
	public class AcceptInviteResponse : InviteReference { }
}
