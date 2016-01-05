namespace Discord.Commands.Permissions.Visibility
{
    public static class PrivateExtensions
    {
        public static CommandBuilder PrivateOnly(this CommandBuilder builder)
		{
			builder.AddCheck(new PrivateChecker());
            return builder;
		}
		public static CommandGroupBuilder PrivateOnly(this CommandGroupBuilder builder)
		{
			builder.AddCheck(new PrivateChecker());
			return builder;
		}
		public static CommandService PrivateOnly(this CommandService service)
		{
			service.Root.AddCheck(new PrivateChecker());
			return service;
		}
	}
}
