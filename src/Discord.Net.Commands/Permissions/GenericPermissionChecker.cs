using System;

namespace Discord.Commands.Permissions
{
    internal class GenericPermissionChecker : IPermissionChecker
    {
		private readonly Func<Command, User, Channel, bool> _checkFunc;

		public GenericPermissionChecker(Func<Command, User, Channel, bool> checkFunc)
		{
			_checkFunc = checkFunc;
        }

		public bool CanRun(Command command, User user, Channel channel, out string error)
		{
			error = null; //Use default error text.
			return _checkFunc(command, user, channel);
        }
	}
}
