namespace Discord.Commands.Permissions.Levels
{
    public static class PermissionLevelExtensions
    {
		public static CommandBuilder MinPermissions(this CommandBuilder builder, int minPermissions)
		{
			builder.AddCheck(new PermissionLevelChecker(builder.Service.Client, minPermissions));
            return builder;
		}
		public static CommandGroupBuilder MinPermissions(this CommandGroupBuilder builder, int minPermissions)
		{
			builder.AddCheck(new PermissionLevelChecker(builder.Service.Client, minPermissions));
			return builder;
		}
		public static CommandService MinPermissions(this CommandService service, int minPermissions)
		{
			service.Root.AddCheck(new PermissionLevelChecker(service.Client, minPermissions));
			return service;
		}
	}
}
