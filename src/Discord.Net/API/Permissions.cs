//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;

namespace Discord.API
{
	//Create/Edit
	internal sealed class SetChannelPermissionsRequest
	{
		[JsonProperty("id")]
		public string Id;
		[JsonProperty("type")]
		public string Type;
		[JsonProperty("allow")]
		public uint Allow;
		[JsonProperty("deny")]
		public uint Deny;
	}
}
