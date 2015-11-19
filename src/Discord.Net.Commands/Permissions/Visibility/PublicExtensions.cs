namespace Discord.Commands.Permissions.Visibility
{
    public static class PublicExtensions
	{
		public static CommandBuilder PublicOnly(this CommandBuilder builder)
		{
			builder.AddCheck(new PublicChecker());
            return builder;
		}
		public static CommandGroupBuilder PublicOnly(this CommandGroupBuilder builder)
		{
			builder.AddCheck(new PublicChecker());
			return builder;
		}
		public static CommandService PublicOnly(this CommandService service)
		{
			service.Root.AddCheck(new PublicChecker());
			return service;
		}
	}
}
