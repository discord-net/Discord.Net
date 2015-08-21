using Discord.API;
using Newtonsoft.Json;
using System;

namespace Discord
{
	public sealed class User
	{
		private readonly DiscordClient _client;

		public string Id { get; }
		public string Name { get; internal set; }

		public string AvatarId { get; internal set; }
		public string AvatarUrl => Endpoints.UserAvatar(Id, AvatarId);
		public string Discriminator { get; internal set; }
		[JsonIgnore]
		public string Email { get; internal set; }
		[JsonIgnore]
		public bool IsVerified { get; internal set; } = true;
		public string GameId { get; internal set; }
		public string Status { get; internal set; }
		public string Mention { get { return $"<@{Id}>"; } }

		public DateTime LastActivity { get; private set; }

		internal User(string id, DiscordClient client)
		{
			Id = id;
			_client = client;
			LastActivity = DateTime.UtcNow;
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
