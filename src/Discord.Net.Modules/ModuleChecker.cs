using Discord.Commands;
using Discord.Commands.Permissions;

namespace Discord.Modules
{
	public class ModuleChecker : IPermissionChecker
	{
		private readonly ModuleManager _manager;
		private readonly ModuleFilter _filterType;

		internal ModuleChecker(ModuleManager manager)
		{
			_manager = manager;
			_filterType = manager.FilterType;
        }

		public bool CanRun(Command command, User user, Channel channel, out string error)
		{
			if (_filterType == ModuleFilter.None || _filterType == ModuleFilter.AlwaysAllowPrivate || _manager.HasChannel(channel))
			{
				error = null;
				return true;
			}
			else
			{
				error = "This module is currently disabled.";
				return false;
			}
		}
	}
}
