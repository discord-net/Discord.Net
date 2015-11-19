//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Discord.API.Converters;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace Discord.API
{
	//Common
	public class RoleReference
	{
		[JsonProperty("guild_id")]
		[JsonConverter(typeof(LongStringConverter))]
		public long GuildId;
		[JsonProperty("role_id")]
		[JsonConverter(typeof(LongStringConverter))]
		public long RoleId;
	}
	public class RoleInfo
	{
		[JsonProperty("id")]
		[JsonConverter(typeof(LongStringConverter))]
		public long Id;
		[JsonProperty("permissions")]
		public uint? Permissions;
		[JsonProperty("name")]
		public string Name;
		[JsonProperty("position")]
		public int? Position;
		[JsonProperty("hoist")]
		public bool? Hoist;
		[JsonProperty("color")]
		public uint? Color;
		[JsonProperty("managed")]
		public bool? Managed;
	}

	//Create
	public sealed class CreateRoleResponse : RoleInfo { }

	//Edit
	public sealed class EditRoleRequest
	{
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name;
		[JsonProperty("permissions", NullValueHandling = NullValueHandling.Ignore)]
		public uint? Permissions;
		[JsonProperty("hoist", NullValueHandling = NullValueHandling.Ignore)]
		public bool? Hoist;
		[JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
		public uint? Color;
	}
	public sealed class EditRoleResponse : RoleInfo { }

	//Reorder
	public sealed class ReorderRolesRequest : IEnumerable<ReorderRolesRequest.Role>
	{
		public sealed class Role
		{
			[JsonProperty("id")]
			[JsonConverter(typeof(LongStringConverter))]
			public long Id;
			[JsonProperty("position")]
			public uint Position;
		}
		private IEnumerable<Role> _roles;
		public ReorderRolesRequest(IEnumerable<Role> roles) { _roles = roles; }

		public IEnumerator<Role> GetEnumerator() => _roles.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => _roles.GetEnumerator();
	}

	//Events
	internal sealed class RoleCreateEvent
	{
		[JsonProperty("guild_id")]
		[JsonConverter(typeof(LongStringConverter))]
		public long GuildId;
		[JsonProperty("role")]
		public RoleInfo Data;
	}
	internal sealed class RoleUpdateEvent
	{
		[JsonProperty("guild_id")]
		[JsonConverter(typeof(LongStringConverter))]
		public long GuildId;
		[JsonProperty("role")]
		public RoleInfo Data;
	}
	internal sealed class RoleDeleteEvent : RoleReference { }
}
