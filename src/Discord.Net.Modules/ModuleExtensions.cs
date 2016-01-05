namespace Discord.Modules
{
    public static class ModuleExtensions
    {
        public static DiscordClient UsingModules(this DiscordClient client)
        {
            client.Services.Add(new ModuleService());
            return client;
        }
        public static DiscordClient AddModule<T>(this DiscordClient client, T instance, string name = null, ModuleFilter filter = ModuleFilter.None)
            where T : class, IModule
        {
            client.Modules().Add(instance, name ?? nameof(T), filter);
            return client;
        }
        public static DiscordClient AddModule<T>(this DiscordClient client, string name = null, ModuleFilter filter = ModuleFilter.None)
            where T : class, IModule, new()
        {
            client.Modules().Add(new T(), name ?? nameof(T), filter);
            return client;
        }

        public static ModuleService Modules(this DiscordClient client, bool required = true)
			=> client.Services.Get<ModuleService>(required);
    }
}
