using Discord.Commands;
using Discord.Commands.Permissions;

namespace Discord.Modules
{
	public class ModuleChecker : IPermissionChecker
	{
		private readonly ModuleManager _manager;

		internal ModuleChecker(ModuleManager manager)
		{
			_manager = manager;
		}

		public bool CanRun(Command command, User user, Channel channel)
		{
			return _manager.FilterType.HasFlag(FilterType.Unrestricted) || _manager.HasChannel(channel);
		}
	}
}
