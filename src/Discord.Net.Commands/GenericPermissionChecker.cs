using System;

namespace Discord.Commands.Permissions
{
    internal class GenericPermissionChecker : IPermissionChecker
    {
		private readonly Func<Command, User, ITextChannel, bool> _checkFunc;
        private readonly string _error;

		public GenericPermissionChecker(Func<Command, User, ITextChannel, bool> checkFunc, string error = null)
		{
			_checkFunc = checkFunc;
            _error = error;
        }

		public bool CanRun(Command command, User user, ITextChannel channel, out string error)
		{
			error = _error;
			return _checkFunc(command, user, channel);
        }
	}
}
