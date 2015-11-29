using System;
using System.Collections.Generic;

namespace Discord.Modules
{
	public class ModuleService : IService
	{
		private DiscordClient _client;

		//ModuleServiceConfig Config { get; }
		public IEnumerable<ModuleManager> Modules => _modules.Values;
		private readonly Dictionary<IModule, ModuleManager> _modules;

		public ModuleService(/*ModuleServiceConfig config*/)
		{
			//Config = config;
			_modules = new Dictionary<IModule, ModuleManager>();
		}

		void IService.Install(DiscordClient client)
		{
			_client = client;
        }

		public void Install<T>(T module, string name, FilterType type)
			where T : class, IModule
		{
			if (module == null) throw new ArgumentNullException(nameof(module));
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (_client == null) throw new InvalidOperationException("Service needs to be added to a DiscordClient before modules can be installed.");
            if (_modules.ContainsKey(module)) throw new InvalidOperationException("This module has already been added.");

			var manager = new ModuleManager(_client, name, type);
			_modules.Add(module, manager);
			module.Install(manager);
			_client.AddSingleton(module);
        }

		public ModuleManager GetManager(IModule module)
		{
			if (module == null) throw new ArgumentNullException(nameof(module));

			ModuleManager result = null;
			_modules.TryGetValue(module, out result);
			return result;
		}
	}
}
