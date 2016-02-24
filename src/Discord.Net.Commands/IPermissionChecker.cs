namespace Discord.Commands.Permissions
{
    public interface IPermissionChecker
    {
		bool CanRun(Command command, User user, ITextChannel channel, out string error);
    }
}
