namespace Discord.Modules
{
    public static class ModuleExtensions
    {
        public static DiscordClient UsingModules(this DiscordClient client)
        {
            client.AddService(new ModuleService());
            return client;
        }

        public static void AddModule(this DiscordClient client, IModule instance, string name = null, ModuleFilter filter = ModuleFilter.None)
        {
            client.GetService<ModuleService>().Add(instance, name, filter);
        }
        public static void AddModule<T>(this DiscordClient client, string name = null, ModuleFilter filter = ModuleFilter.None)
            where T : class, IModule, new()
        {
            client.GetService<ModuleService>().Add<T>(name, filter);
        }
        public static void AddModule<T>(this DiscordClient client, T instance, string name = null, ModuleFilter filter = ModuleFilter.None)
            where T : class, IModule
        {
            client.GetService<ModuleService>().Add(instance, name, filter);
        }
        public static ModuleManager<T> GetModule<T>(this DiscordClient client)
            where T : class, IModule
            => client.GetService<ModuleService>().Get<T>();
    }
}
