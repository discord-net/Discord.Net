using System;
using System.Collections.Generic;

namespace Discord.Modules
{
	public class ModuleService : IService
	{
		private DiscordClient _client;

		//ModuleServiceConfig Config { get; }
		public IEnumerable<IModule> Modules => _modules.Keys;
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

		public void Install(IModule module, FilterType type)
		{
			if (_client == null) throw new InvalidOperationException("Service needs to be added to a DiscordClient before modules can be installed.");
            if (_modules.ContainsKey(module)) throw new InvalidOperationException("This module has already been added.");

			var manager = new ModuleManager(_client, type);
			_modules.Add(module, manager);
			module.Install(manager);
        }
	}
}
