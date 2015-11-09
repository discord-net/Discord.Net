namespace Discord.Commands.Permissions.Userlist
{
    public static class WhitelistExtensions
	{
		public static CommandBuilder UseGlobalWhitelist(this CommandBuilder builder)
		{
			builder.AddCheck(new WhitelistChecker(builder.Service.Client));
			return builder;
		}
		public static CommandGroupBuilder UseGlobalWhitelist(this CommandGroupBuilder builder)
		{
			builder.AddCheck(new WhitelistChecker(builder.Service.Client));
			return builder;
		}
		public static CommandService UseGlobalWhitelist(this CommandService service)
		{
			service.Root.AddCheck(new BlacklistChecker(service.Client));
			return service;
		}
	}
}
