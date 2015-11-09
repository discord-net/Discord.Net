using System;

namespace Discord.Commands.Permissions.Userlist
{
    public class BlacklistChecker : IPermissionChecker
	{
		private readonly BlacklistService _service;

		internal BlacklistChecker(DiscordClient client)
		{
			_service = client.GetService<BlacklistService>();
			if (_service == null)
				throw new InvalidOperationException($"{nameof(BlacklistService)} must be added to {nameof(DiscordClient)} before this function is called.");
		}

		public bool CanRun(Command command, User user, Channel channel)
		{
			return _service.CanRun(user);
		}
	}
}
