namespace Discord.Commands.Permissions.Userlist
{
    public class WhitelistChecker : IPermissionChecker
	{
		private readonly WhitelistService _service;

		internal WhitelistChecker(DiscordClient client)
		{
			_service = client.GetService<WhitelistService>(true);
		}

		public bool CanRun(Command command, User user, Channel channel, out string error)
		{
			error = null; //Use default error text.
			return _service.CanRun(user);
		}
	}
}
