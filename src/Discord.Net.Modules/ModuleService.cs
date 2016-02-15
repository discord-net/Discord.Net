using System;
using System.Collections.Generic;

namespace Discord.Modules
{
	public class ModuleService : IService
	{
		public DiscordClient Client { get; private set; }
        
		public IEnumerable<ModuleManager> Modules => _modules.Values;
		private readonly Dictionary<Type, ModuleManager> _modules;

		public ModuleService()
		{
			_modules = new Dictionary<Type, ModuleManager>();
		}

		void IService.Install(DiscordClient client)
		{
            Client = client;
        }

		public T Add<T>(T module, string name, ModuleFilter filterType)
			where T : class, IModule
		{
			if (module == null) throw new ArgumentNullException(nameof(module));
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (Client == null)
                throw new InvalidOperationException("Service needs to be added to a DiscordClient before modules can be installed.");

            Type type = typeof(T);
            if (_modules.ContainsKey(type))
                throw new InvalidOperationException("This module has already been added.");

			var manager = new ModuleManager<T>(Client, module, name, filterType);
			_modules.Add(type, manager);
			module.Install(manager);
            return module;
        }
        public ModuleManager<T> Get<T>()
            where T : class, IModule
        {
            ModuleManager manager;
            if (_modules.TryGetValue(typeof(T), out manager))
                return manager as ModuleManager<T>;
            return null;
        }
	}
}
