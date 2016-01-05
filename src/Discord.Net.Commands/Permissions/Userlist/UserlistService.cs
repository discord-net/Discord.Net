using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Commands.Permissions.Userlist
{
    public class UserlistService : IService
	{
		protected readonly ConcurrentDictionary<ulong, bool> _userList;
		private DiscordClient _client;

		public DiscordClient Client => _client;
		public IEnumerable<ulong> UserIds => _userList.Select(x => x.Key);

		public UserlistService(params ulong[] initialUserIds)
		{
			_userList = new ConcurrentDictionary<ulong, bool>();
            for (int i = 0; i < initialUserIds.Length; i++)
                _userList.TryAdd(initialUserIds[i], true);
		}

		public void Add(User user)
		{
			if (user == null) throw new ArgumentNullException(nameof(user));

			_userList[user.Id] = true;
		}
		public void Add(ulong userId)
		{
			_userList[userId] = true;
		}

		public bool Remove(User user)
		{
			if (user == null) throw new ArgumentNullException(nameof(user));

			bool ignored;
			return _userList.TryRemove(user.Id, out ignored);
		}
		public bool Remove(ulong userId)
		{
			bool ignored;
			return _userList.TryRemove(userId, out ignored);
		}

		void IService.Install(DiscordClient client)
		{
			_client = client;
		}
	}
}
