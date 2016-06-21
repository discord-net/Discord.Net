using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Commands
{
    public class CommandService
    {
        private readonly SemaphoreSlim _moduleLock;
        private readonly ConcurrentDictionary<object, Module> _modules;
        private readonly ConcurrentDictionary<string, List<Command>> _map;

        public IEnumerable<Module> Modules => _modules.Select(x => x.Value);
        public IEnumerable<Command> Commands => _modules.SelectMany(x => x.Value.Commands);

        public CommandService()
        {
            _moduleLock = new SemaphoreSlim(1, 1);
            _modules = new ConcurrentDictionary<object, Module>();
            _map = new ConcurrentDictionary<string, List<Command>>();
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

            foreach (var cmd in loadedModule.Commands)
            {
                var list = _map.GetOrAdd(cmd.Text, _ => new List<Command>());
                lock (list)
                    list.Add(cmd);
            }

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
                        var module = ReflectionUtils.CreateObject(typeInfo);
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
                return UnloadInternal(module);
            }
            finally
            {
                _moduleLock.Release();
            }
        }
        private bool UnloadInternal(object module)
        {
            Module unloadedModule;
            if (_modules.TryRemove(module, out unloadedModule))
            {
                foreach (var cmd in unloadedModule.Commands)
                {
                    List<Command> list;
                    if (_map.TryGetValue(cmd.Text, out list))
                    {
                        lock (list)
                            list.Remove(cmd);
                    }
                }
                return true;
            }
            else
                return false;
        }

        //TODO: C#7 Candidate for tuple
        public SearchResults Search(string input)
        {
            string lowerInput = input.ToLowerInvariant();

            List<Command> bestGroup = null, group;
            int startPos = 0, endPos;

            while (true)
            {
                endPos = input.IndexOf(' ', startPos);
                string cmdText = endPos == -1 ? input.Substring(startPos) : input.Substring(startPos, endPos - startPos);
                startPos = endPos + 1;
                if (!_map.TryGetValue(cmdText, out group))
                    break;
                bestGroup = group;
            }

            ImmutableArray<Command> cmds;
            if (bestGroup != null)
            {
                lock (bestGroup)
                    cmds = bestGroup.ToImmutableArray();
            }
            else
                cmds = ImmutableArray.Create<Command>();
            return new SearchResults(cmds, startPos);
        }
    }
}
