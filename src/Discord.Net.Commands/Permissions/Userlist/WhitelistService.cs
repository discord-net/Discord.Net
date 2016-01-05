namespace Discord.Commands.Permissions.Userlist
{
	public class WhitelistService : UserlistService
	{
		public WhitelistService(params ulong[] initialList)
			: base(initialList)
		{
		}

		public bool CanRun(User user)
            => _userList.ContainsKey(user.Id);
	}
}
