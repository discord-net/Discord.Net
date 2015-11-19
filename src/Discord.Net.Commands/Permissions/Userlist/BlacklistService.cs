using System.Collections.Generic;

namespace Discord.Commands.Permissions.Userlist
{
    public class BlacklistService : UserlistService
	{
		public BlacklistService(IEnumerable<long> initialList = null)
			: base(initialList)
		{
		}

        public bool CanRun(User user)
		{
			return !_userList.ContainsKey(user.Id);
		}
	}
}
