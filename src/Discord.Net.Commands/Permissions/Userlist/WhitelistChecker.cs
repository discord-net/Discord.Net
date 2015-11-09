using System;

namespace Discord.Commands.Permissions.Userlist
{
    public class WhitelistChecker : IPermissionChecker
	{
		private readonly WhitelistService _service;

		internal WhitelistChecker(DiscordClient client)
		{
			_service = client.GetService<WhitelistService>();
			if (_service == null)
				throw new InvalidOperationException($"{nameof(WhitelistService)} must be added to {nameof(DiscordClient)} before this function is called.");
		}

		public bool CanRun(Command command, User user, Channel channel)
		{
			return _service.CanRun(user);
		}
	}
}
