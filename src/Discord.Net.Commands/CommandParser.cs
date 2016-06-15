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
        
        internal Module(object parent, TypeInfo typeInfo)
        {
            List<Command> commands = new List<Command>();
            SearchClass(parent, commands, typeInfo);
            Commands = commands;
        }

        private void SearchClass(object parent, List<Command> commands, TypeInfo typeInfo)
        {
            foreach (var method in typeInfo.DeclaredMethods)
            {
                if (typeInfo.GetCustomAttribute<CommandAttribute>() != null)
                {

                }
            }
            foreach (var type in typeInfo.DeclaredNestedTypes)
            {
                if (typeInfo.GetCustomAttribute<GroupAttribute>() != null)
                {
                    SearchClass(CommandParser.CreateObject(typeInfo), commands, type);
                }
            }
        }
    }
    public class Command
    {
        public string SourceName { get; }

        internal Command(TypeInfo typeInfo)
        {
        }
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
                var typeInfo = module.GetType().GetTypeInfo();
                if (typeInfo.GetCustomAttribute<ModuleAttribute>() == null)
                    throw new ArgumentException($"Modules must be marked with ModuleAttribute.");
                return LoadInternal(module, typeInfo);
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
                        var module = CreateObject(typeInfo);
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

        internal static object CreateObject(TypeInfo typeInfo)
        {
            var constructor = typeInfo.DeclaredConstructors.Where(x => x.GetParameters().Length == 0).FirstOrDefault();
            if (constructor == null)
                throw new InvalidOperationException($"Failed to find a valid constructor for \"{typeInfo.FullName}\"");
            try
            {
                return constructor.Invoke(null);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create \"{typeInfo.FullName}\"", ex);
            }
        }
    }
}
