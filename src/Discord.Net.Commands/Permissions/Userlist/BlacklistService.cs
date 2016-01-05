namespace Discord.Commands.Permissions.Userlist
{
    public class BlacklistService : UserlistService
	{
		public BlacklistService(params ulong[] initialList)
			: base(initialList)
		{
		}

        public bool CanRun(User user)
            => !_userList.ContainsKey(user.Id);
	}
}
