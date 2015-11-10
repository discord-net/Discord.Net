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
			_service = client.GetService<PermissionLevelService>(true);
			_minPermissions = minPermissions;
        }

		public bool CanRun(Command command, User user, Channel channel, out string error)
		{
			error = null; //Use default error text.
			int permissions = _service.GetPermissionLevel(user, channel);
			return permissions >= _minPermissions;
		}
	}
}
