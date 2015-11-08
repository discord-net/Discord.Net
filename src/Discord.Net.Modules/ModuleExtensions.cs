namespace Discord.Modules
{
    public static class ModuleExtensions
    {
		public static ModuleService Modules(this DiscordClient client)
			=> client.GetService<ModuleService>();
    }
}
