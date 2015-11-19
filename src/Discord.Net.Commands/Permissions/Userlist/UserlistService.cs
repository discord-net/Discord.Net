using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Commands.Permissions.Userlist
{
    public class UserlistService : IService
	{
		protected readonly ConcurrentDictionary<long, bool> _userList;
		private DiscordClient _client;

		public DiscordClient Client => _client;
		public IEnumerable<long> UserIds => _userList.Select(x => x.Key);

		public UserlistService(IEnumerable<long> initialList = null)
		{
			if (initialList != null)
				_userList = new ConcurrentDictionary<long, bool>(initialList.Select(x => new KeyValuePair<long, bool>(x, true)));
			else
				_userList = new ConcurrentDictionary<long, bool>();
		}

		public void Add(User user)
		{
			if (user == null) throw new ArgumentNullException(nameof(user));
			_userList[user.Id] = true;
		}
		public void Add(long userId)
		{
			if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));
			_userList[userId] = true;
		}
		public bool Remove(User user)
		{
			if (user == null) throw new ArgumentNullException(nameof(user));
			bool ignored;
			return _userList.TryRemove(user.Id, out ignored);
		}
		public bool Remove(long userId)
		{
			if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));
			bool ignored;
			return _userList.TryRemove(userId, out ignored);
		}

		public void Install(DiscordClient client)
		{
			_client = client;
		}
	}
}
