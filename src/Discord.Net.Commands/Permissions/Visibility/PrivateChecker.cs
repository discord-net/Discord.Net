namespace Discord.Commands.Permissions.Visibility
{
	public class PrivateChecker : IPermissionChecker
	{
		internal PrivateChecker() { }

		public bool CanRun(Command command, User user, Channel channel, out string error)
		{
			if (user.Server != null)
			{
				error = "This command may only be run in a private chat.";
				return false;
			}
			else
			{
				error = null;
				return true;
			}
		}
	}
}
