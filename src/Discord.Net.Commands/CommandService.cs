using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
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
        private readonly Dictionary<Type, TypeReader> _typeReaders;

        public IEnumerable<Module> Modules => _modules.Select(x => x.Value);
        public IEnumerable<Command> Commands => _modules.SelectMany(x => x.Value.Commands);

        public CommandService()
        {
            _moduleLock = new SemaphoreSlim(1, 1);
            _modules = new ConcurrentDictionary<object, Module>();
            _map = new ConcurrentDictionary<string, List<Command>>();
            _typeReaders = new Dictionary<Type, TypeReader>
            {
                [typeof(string)] = new GenericTypeReader((m, s) => Task.FromResult(TypeReaderResult.FromSuccess(s))),
                [typeof(byte)] = new GenericTypeReader((m, s) =>
                {
                    byte value;
                    if (byte.TryParse(s, out value)) return Task.FromResult(TypeReaderResult.FromSuccess(value));
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Failed to parse Byte"));
                }),
                [typeof(sbyte)] = new GenericTypeReader((m, s) =>
                {
                    sbyte value;
                    if (sbyte.TryParse(s, out value)) return Task.FromResult(TypeReaderResult.FromSuccess(value));
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Failed to parse SByte"));
                }),
                [typeof(ushort)] = new GenericTypeReader((m, s) =>
                {
                    ushort value;
                    if (ushort.TryParse(s, out value)) return Task.FromResult(TypeReaderResult.FromSuccess(value));
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Failed to parse UInt16"));
                }),
                [typeof(short)] = new GenericTypeReader((m, s) =>
                {
                    short value;
                    if (short.TryParse(s, out value)) return Task.FromResult(TypeReaderResult.FromSuccess(value));
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Failed to parse Int16"));
                }),
                [typeof(uint)] = new GenericTypeReader((m, s) =>
                {
                    uint value;
                    if (uint.TryParse(s, out value)) return Task.FromResult(TypeReaderResult.FromSuccess(value));
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Failed to parse UInt32"));
                }),
                [typeof(int)] = new GenericTypeReader((m, s) =>
                {
                    int value;
                    if (int.TryParse(s, out value)) return Task.FromResult(TypeReaderResult.FromSuccess(value));
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Failed to parse Int32"));
                }),
                [typeof(ulong)] = new GenericTypeReader((m, s) =>
                {
                    ulong value;
                    if (ulong.TryParse(s, out value)) return Task.FromResult(TypeReaderResult.FromSuccess(value));
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Failed to parse UInt64"));
                }),
                [typeof(long)] = new GenericTypeReader((m, s) =>
                {
                    long value;
                    if (long.TryParse(s, out value)) return Task.FromResult(TypeReaderResult.FromSuccess(value));
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Failed to parse Int64"));
                }),
                [typeof(float)] = new GenericTypeReader((m, s) =>
                {
                    float value;
                    if (float.TryParse(s, out value)) return Task.FromResult(TypeReaderResult.FromSuccess(value));
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Failed to parse Single"));
                }),
                [typeof(double)] = new GenericTypeReader((m, s) =>
                {
                    double value;
                    if (double.TryParse(s, out value)) return Task.FromResult(TypeReaderResult.FromSuccess(value));
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Failed to parse Double"));
                }),
                [typeof(decimal)] = new GenericTypeReader((m, s) =>
                {
                    decimal value;
                    if (decimal.TryParse(s, out value)) return Task.FromResult(TypeReaderResult.FromSuccess(value));
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Failed to parse Decimal"));
                }),
                [typeof(DateTime)] = new GenericTypeReader((m, s) =>
                {
                    DateTime value;
                    if (DateTime.TryParse(s, out value)) return Task.FromResult(TypeReaderResult.FromSuccess(value));
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Failed to parse DateTime"));
                }),
                [typeof(DateTimeOffset)] = new GenericTypeReader((m, s) =>
                {
                    DateTimeOffset value;
                    if (DateTimeOffset.TryParse(s, out value)) return Task.FromResult(TypeReaderResult.FromSuccess(value));
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Failed to parse DateTimeOffset"));
                }),

                [typeof(IMessage)] = new MessageTypeReader(),
                [typeof(IChannel)] = new ChannelTypeReader<IChannel>(),
                [typeof(IGuildChannel)] = new ChannelTypeReader<IGuildChannel>(),
                [typeof(ITextChannel)] = new ChannelTypeReader<ITextChannel>(),
                [typeof(IVoiceChannel)] = new ChannelTypeReader<IVoiceChannel>(),
                [typeof(IRole)] = new RoleTypeReader(),
                [typeof(IUser)] = new UserTypeReader<IUser>(),
                [typeof(IGuildUser)] = new UserTypeReader<IGuildUser>()
            };
        }

        public void AddTypeReader<T>(TypeReader reader)
        {
            _typeReaders[typeof(T)] = reader;
        }
        public void AddTypeReader(Type type, TypeReader reader)
        {
            _typeReaders[type] = reader;
        }
        internal TypeReader GetTypeReader(Type type)
        {
            TypeReader reader;
            if (_typeReaders.TryGetValue(type, out reader))
                return reader;
            return null;
        }

        public async Task<Module> Load(object moduleInstance)
        {
            await _moduleLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_modules.ContainsKey(moduleInstance))
                    throw new ArgumentException($"This module has already been loaded.");

                var typeInfo = moduleInstance.GetType().GetTypeInfo();
                if (typeInfo.GetCustomAttribute<ModuleAttribute>() == null)
                    throw new ArgumentException($"Modules must be marked with ModuleAttribute.");

                return LoadInternal(moduleInstance, typeInfo);
            }
            finally
            {
                _moduleLock.Release();
            }
        }
        private Module LoadInternal(object moduleInstance, TypeInfo typeInfo)
        {
            var loadedModule = new Module(this, moduleInstance, typeInfo);
            _modules[moduleInstance] = loadedModule;

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
            var modules = ImmutableArray.CreateBuilder<Module>();
            await _moduleLock.WaitAsync().ConfigureAwait(false);
            try
            {
                foreach (var type in assembly.ExportedTypes)
                {
                    var typeInfo = type.GetTypeInfo();
                    if (typeInfo.GetCustomAttribute<ModuleAttribute>() != null)
                    {
                        var moduleInstance = ReflectionUtils.CreateObject(typeInfo);
                        modules.Add(LoadInternal(moduleInstance, typeInfo));
                    }
                }
                return modules.ToImmutable();
            }
            finally
            {
                _moduleLock.Release();
            }
        }

        public async Task<bool> Unload(Module module)
        {
            await _moduleLock.WaitAsync().ConfigureAwait(false);
            try
            {
                return UnloadInternal(module.Instance);
            }
            finally
            {
                _moduleLock.Release();
            }
        }
        public async Task<bool> Unload(object moduleInstance)
        {
            await _moduleLock.WaitAsync().ConfigureAwait(false);
            try
            {
                return UnloadInternal(moduleInstance);
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
        
        public SearchResult Search(string input)
        {
            string lowerInput = input.ToLowerInvariant();

            List<Command> bestGroup = null, group;
            int startPos = 0, endPos;

            while (true)
            {
                endPos = input.IndexOf(' ', startPos);
                string cmdText = endPos == -1 ? input.Substring(startPos) : input.Substring(startPos, endPos - startPos);
                if (!_map.TryGetValue(cmdText, out group))
                    break;
                bestGroup = group;
                if (endPos == -1)
                {
                    startPos = input.Length;
                    break;
                }
                else
                    startPos = endPos + 1;
            }
            
            if (bestGroup != null)
            {
                lock (bestGroup)
                    return SearchResult.FromSuccess(bestGroup.ToImmutableArray(), input.Substring(startPos));
            }
            else
                return SearchResult.FromError(CommandError.UnknownCommand, "Unknown command.");
        }

        public async Task<IResult> Execute(IMessage message, string input)
        {
            var searchResult = Search(input);
            if (!searchResult.IsSuccess)
                return searchResult;

            var commands = searchResult.Commands;
            for (int i = 0; i < commands.Count; i++)
            {
                var parseResult = await commands[i].Parse(message, searchResult);
                if (!parseResult.IsSuccess)
                    continue;
                var executeResult = await commands[i].Execute(message, parseResult);
                return executeResult;
            }
            
            return ParseResult.FromError(CommandError.ParseFailed, "This input does not match any overload.");
        }
    }
}
