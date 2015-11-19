using System.Collections.Generic;

namespace Discord.Commands.Permissions.Userlist
{
	public class WhitelistService : UserlistService
	{
		public WhitelistService(IEnumerable<long> initialList = null)
			: base(initialList)
		{
		}

		public bool CanRun(User user)
		{
			return _userList.ContainsKey(user.Id);
		}
	}
}
