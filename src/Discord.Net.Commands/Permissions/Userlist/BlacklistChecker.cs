namespace Discord.Commands.Permissions.Userlist
{
    public class BlacklistChecker : IPermissionChecker
	{
		private readonly BlacklistService _service;

		internal BlacklistChecker(DiscordClient client)
		{
			_service = client.GetService<BlacklistService>(true);
		}

		public bool CanRun(Command command, User user, Channel channel)
		{
			return _service.CanRun(user);
		}
	}
}
