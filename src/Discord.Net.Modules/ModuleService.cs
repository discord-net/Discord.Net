using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Discord.Modules
{
	public class ModuleService : IService
	{
		public DiscordClient Client { get; private set; }

        private static readonly MethodInfo addMethod = typeof(ModuleService).GetTypeInfo().GetDeclaredMethods(nameof(Add))
            .Single(x => x.IsGenericMethodDefinition && x.GetParameters().Length == 3);

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

        public void Add(IModule instance, string name, ModuleFilter filter)
        {
            Type type = instance.GetType();
            addMethod.MakeGenericMethod(type).Invoke(this, new object[] { instance, name, filter });
        }
        public void Add<T>(string name, ModuleFilter filter)
            where T : class, IModule, new()
            => Add(new T(), name, filter);
        public void Add<T>(T instance, string name, ModuleFilter filter)
			where T : class, IModule
		{
			if (instance == null) throw new ArgumentNullException(nameof(instance));
			if (Client == null)
                throw new InvalidOperationException("Service needs to be added to a DiscordClient before modules can be installed.");

            Type type = typeof(T);
            if (name == null) name = type.Name;
            if (_modules.ContainsKey(type))
                throw new InvalidOperationException("This module has already been added.");

			var manager = new ModuleManager<T>(Client, instance, name, filter);
			_modules.Add(type, manager);
            instance.Install(manager);
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
