namespace Discord.Commands.Permissions.Visibility
{
	public class PublicChecker : IPermissionChecker
	{
		internal PublicChecker() { }

		public bool CanRun(Command command, User user, Channel channel, out string error)
		{
			if (user.Server == null)
			{
				error = "This command can't be run in a private chat.";
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
