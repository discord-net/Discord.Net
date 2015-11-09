using System;

namespace Discord.Commands.Permissions.Levels
{
	public class PermissionLevelChecker : IPermissionChecker
	{
		private readonly PermissionLevelService _service;
		private readonly int _minPermissions;

		public PermissionLevelService Service => _service;
		public int MinPermissions => _minPermissions;

		internal PermissionLevelChecker(DiscordClient client, int minPermissions)
		{
			_service = client.GetService<PermissionLevelService>();
			_minPermissions = minPermissions;
            if (_service == null)
				throw new InvalidOperationException($"{nameof(PermissionLevelService)} must be added to {nameof(DiscordClient)} before this function is called.");
        }

		public bool CanRun(Command command, User user, Channel channel)
		{
			int permissions = _service.GetPermissionLevel(user, channel);
			return permissions >= _minPermissions;
		}
	}
}
