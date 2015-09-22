using Discord.Net.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Discord
{
	public sealed class User
	{
		private readonly DiscordClient _client;
		private int _refs;
		private DateTime? _lastPrivateActivity;

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
		/// <summary> Returns the email for this user. </summary>
		/// <remarks> This field is only ever populated for the current logged in user. </remarks>
		[JsonIgnore]
		public string Email { get; internal set; }
		/// <summary> Returns if the email for this user has been verified. </summary>
		/// <remarks> This field is only ever populated for the current logged in user. </remarks>
		[JsonIgnore]
		public bool? IsVerified { get; internal set; }

		/// <summary> Returns the Id of the private messaging channel with this user, if one exists. </summary>
		public string PrivateChannelId { get; set; }
		/// <summary> Returns the private messaging channel with this user, if one exists. </summary>
		[JsonIgnore]
		public Channel PrivateChannel => _client.Channels[PrivateChannelId];

		/// <summary> Returns a collection of all server-specific data for every server this user is a member of. </summary>
		public IEnumerable<Member> Memberships => _client.Servers.Where(x => x.HasMember(Id)).Select(x => _client.Members[Id, x?.Id]);
		/// <summary> Returns a collection of all servers this user is a member of. </summary>
		public IEnumerable<Server> Servers => _client.Servers.Where(x => x.HasMember(Id));
		/// <summary> Returns a collection of all messages this user has sent that are still in cache. </summary>
		public IEnumerable<Message> Messages => _client.Messages.Where(x => x.UserId == Id);

		/// <summary> Returns the id for the game this user is currently playing. </summary>
		public string GameId => Memberships.Where(x => x.GameId != null).Select(x => x.GameId).FirstOrDefault();
		/// <summary> Returns the current status for this user. </summary>
		public string Status => Memberships.OrderByDescending(x => x.StatusSince).Select(x => x.Status).FirstOrDefault();
		/// <summary> Returns the time this user's status was last changed. </summary>
		public DateTime StatusSince => Memberships.OrderByDescending(x => x.StatusSince).Select(x => x.StatusSince).First();
		/// <summary> Returns the time this user last sent/edited a message, started typing or sent voice data. </summary>
		public DateTime? LastActivity
		{
			get
			{
				var lastServerActivity = Memberships.OrderByDescending(x => x.LastActivity).Select(x => x.LastActivity).FirstOrDefault();
				if (lastServerActivity == null || (_lastPrivateActivity != null && _lastPrivateActivity.Value > lastServerActivity.Value))
					return _lastPrivateActivity;
				else
					return lastServerActivity;
			}
		}
		/// <summary> Returns the time this user was last seen online. </summary>
		public DateTime? LastOnline => Memberships.OrderByDescending(x => x.LastOnline).Select(x => x.LastOnline).FirstOrDefault();

		internal User(DiscordClient client, string id)
		{
			_client = client;
			Id = id;
		}

		internal void Update(UserReference model)
		{
			AvatarId = model.Avatar;
			Discriminator = model.Discriminator;
			Name = model.Username;
		}
		internal void Update(SelfUserInfo model)
		{
			Update(model as UserReference);
			Email = model.Email;
			IsVerified = model.IsVerified;
		}
		internal void UpdateActivity(DateTime? activity = null)
		{
			if (_lastPrivateActivity == null || activity > _lastPrivateActivity.Value)
				_lastPrivateActivity = activity ?? DateTime.UtcNow;
		}

		public override string ToString() => Name;
		
		public void AddRef()
		{
			Interlocked.Increment(ref _refs);
		}
		public void RemoveRef()
		{
			if (Interlocked.Decrement(ref _refs) == 0)
				_client.Users.TryRemove(Id);
		}
	}
}
