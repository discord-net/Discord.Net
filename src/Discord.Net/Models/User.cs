using Discord.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public sealed class User
	{
		private readonly DiscordClient _client;

		/// <summary> Returns the unique identifier for this user. </summary>
		public string Id { get; }
		/// <summary> Returns the name of this channel. </summary>
		public string Name { get; internal set; }

		/// <summary> Returns the unique identifier for this user's current avatar. </summary>
		public string AvatarId { get; internal set; }
		/// <summary> Returns the URL to this user's current avatar. </summary>
		public string AvatarUrl => Endpoints.UserAvatar(Id, AvatarId);
		/// <summary> Returns a by-name unique identifier separating this user from others with the same name. </summary>
		public string Discriminator { get; internal set; }
		[JsonIgnore]
		/// <summary> Returns the email for this user. </summary>
		/// <remarks> This field is only ever populated for the current logged in user. </remarks>
		public string Email { get; internal set; }
		[JsonIgnore]
		/// <summary> Returns if the email for this user has been verified. </summary>
		/// <remarks> This field is only ever populated for the current logged in user. </remarks>
		public bool IsVerified { get; internal set; }

		/// <summary> Returns the Id of the private messaging channel with this user, if one exists. </summary>
		public string PrivateChannelId { get; set; }
		/// <summary> Returns the private messaging channel with this user, if one exists. </summary>
		public Channel PrivateChannel => _client.GetChannel(PrivateChannelId);

		/// <summary> Returns a collection of all server-specific data for every server this user is a member of. </summary>
		public IEnumerable<Member> Memberships => _client._servers.Select(x => x._members[Id]).Where(x => x != null);
		/// <summary> Returns a collection of all servers this user is a member of. </summary>
		public IEnumerable<Server> Servers => _client._servers.Where(x => x._members[Id] != null);

		//TODO: Add voice triggering lastactivity
		/// <summary> Returns the time this user last sent a message. </summary>
		/// <remarks> Is not currently affected by voice activity </remarks>
		public DateTime LastActivity { get; private set; }

		internal User(string id, DiscordClient client)
		{
			Id = id;
			_client = client;
			LastActivity = DateTime.UtcNow;
			IsVerified = true;
        }

		internal void UpdateActivity(DateTime activity)
		{
			if (activity > LastActivity)
				LastActivity = activity;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
