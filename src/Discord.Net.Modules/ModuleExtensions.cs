namespace Discord.Modules
{
    public static class ModuleExtensions
    {
		public static ModuleService Commands(this DiscordClient client)
			=> client.GetService<ModuleService>();
    }
}
