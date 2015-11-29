using Discord.API;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public sealed class GlobalUser : CachedObject<long>
	{

		/// <summary> Returns the email for this user. Note: this field is only ever populated for the current logged in user. </summary>
		[JsonIgnore]
		public string Email { get; private set; }
		/// <summary> Returns if the email for this user has been verified. Note: this field is only ever populated for the current logged in user. </summary>
		[JsonIgnore]
		public bool? IsVerified { get; private set; }

		/// <summary> Returns the private messaging channel with this user, if one exists. </summary>
		[JsonIgnore]
		public Channel PrivateChannel
		{
			get { return _privateChannel; }
			set
			{
				_privateChannel = value;
				if (value == null)
					CheckUser();
            }
		}
		[JsonProperty]
		private long? PrivateChannelId => _privateChannel?.Id;
        private Channel _privateChannel;

		/// <summary> Returns a collection of all server-specific data for every server this user is a member of. </summary>
		[JsonIgnore]
		public IEnumerable<User> Memberships => _users.Select(x => x.Value);
		[JsonProperty]
		private IEnumerable<long> ServerIds => _users.Select(x => x.Key);
		private readonly ConcurrentDictionary<long, User> _users;

		internal GlobalUser(DiscordClient client, long id)
			: base(client, id)
		{
			_users = new ConcurrentDictionary<long, User>();
		}
		internal override bool LoadReferences() { return true; }
		internal override void UnloadReferences()
		{
			//Don't need to clean _users - they're considered owned by server
		}
		
		internal void Update(UserInfo model)
		{
			if (model.Email != null)
				Email = model.Email;
			if (model.IsVerified != null)
				IsVerified = model.IsVerified;
		}

		internal void AddUser(User user) => _users.TryAdd(user.Server?.Id ?? 0, user);
		internal void RemoveUser(User user)
		{
			if (_users.TryRemove(user.Server?.Id ?? 0, out user))
				CheckUser();
        }
		internal void CheckUser()
		{
			if (_users.Count == 0 && PrivateChannel == null)
				_client.GlobalUsers.TryRemove(Id);
		}

		public override bool Equals(object obj) => obj is GlobalUser && (obj as GlobalUser).Id == Id;
		public override int GetHashCode() => unchecked(Id.GetHashCode() + 7891);
		public override string ToString() => IdConvert.ToString(Id);
	}
}
