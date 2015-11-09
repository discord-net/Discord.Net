namespace Discord.Modules
{
    public static class ModuleExtensions
    {
		public static ModuleService Modules(this DiscordClient client, bool required = true)
			=> client.GetService<ModuleService>(required);
    }
}
