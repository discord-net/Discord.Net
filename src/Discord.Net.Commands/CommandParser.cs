using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Commands
{
    public class Module
    {
        public string Name { get; }
        public IEnumerable<Command> Commands { get; }
        
        internal Module(object module, TypeInfo typeInfo)
        {
            List<Command> commands = new List<Command>();
            SearchClass(commands);
            Commands = commands;
        }

        private void SearchClass(List<Command> commands)
        {
            //TODO: Implement
        }
    }
    public class Command
    {
        public string SourceName { get; }
    }

    public class CommandParser
    {
        private readonly SemaphoreSlim _moduleLock;
        private readonly Dictionary<object, Module> _modules;

        public CommandParser()
        {
            _modules = new Dictionary<object, Module>();
            _moduleLock = new SemaphoreSlim(1, 1);
        }
        public async Task<Module> Load(object module)
        {
            await _moduleLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_modules.ContainsKey(module))
                    throw new ArgumentException($"This module has already been loaded.");
                return LoadInternal(module, module.GetType().GetTypeInfo());
            }
            finally
            {
                _moduleLock.Release();
            }
        }
        private Module LoadInternal(object module, TypeInfo typeInfo)
        {
            var loadedModule = new Module(module, typeInfo);
            _modules[module] = loadedModule;
            return loadedModule;
        }
        public async Task<IEnumerable<Module>> LoadAssembly(Assembly assembly)
        {
            List<Module> modules = new List<Module>();
            await _moduleLock.WaitAsync().ConfigureAwait(false);
            try
            {
                foreach (var type in assembly.ExportedTypes)
                {
                    var typeInfo = type.GetTypeInfo();
                    if (typeInfo.GetCustomAttribute<ModuleAttribute>() != null)
                    {
                        var constructor = typeInfo.DeclaredConstructors.Where(x => x.GetParameters().Length == 0).FirstOrDefault();
                        if (constructor == null)
                            throw new InvalidOperationException($"Failed to find a valid constructor for \"{typeInfo.FullName}\"");
                        object module;
                        try { module = constructor.Invoke(null); }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException($"Failed to create \"{typeInfo.FullName}\"", ex);
                        }
                        modules.Add(LoadInternal(module, typeInfo));
                    }
                }
                return modules;
            }
            finally
            {
                _moduleLock.Release();
            }
        }

        public async Task<bool> Unload(object module)
        {
            await _moduleLock.WaitAsync().ConfigureAwait(false);
            try
            {
                return _modules.Remove(module);
            }
            finally
            {
                _moduleLock.Release();
            }
        }
    }
}
