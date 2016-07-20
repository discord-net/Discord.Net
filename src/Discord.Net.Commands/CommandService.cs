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
        private readonly ConcurrentDictionary<Type, TypeReader> _typeReaders;
        private readonly CommandMap _map;

        public IEnumerable<Module> Modules => _modules.Select(x => x.Value);
        public IEnumerable<Command> Commands => _modules.SelectMany(x => x.Value.Commands);

        public CommandService()
        {
            _moduleLock = new SemaphoreSlim(1, 1);
            _modules = new ConcurrentDictionary<object, Module>();
            _map = new CommandMap();
            _typeReaders = new ConcurrentDictionary<Type, TypeReader>
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
                var moduleAttr = typeInfo.GetCustomAttribute<ModuleAttribute>();
                if (moduleAttr == null)
                    throw new ArgumentException($"Modules must be marked with ModuleAttribute.");

                return LoadInternal(moduleInstance, moduleAttr, typeInfo);
            }
            finally
            {
                _moduleLock.Release();
            }
        }
        private Module LoadInternal(object moduleInstance, ModuleAttribute moduleAttr, TypeInfo typeInfo)
        {
            var loadedModule = new Module(this, moduleInstance, moduleAttr, typeInfo);
            _modules[moduleInstance] = loadedModule;

            foreach (var cmd in loadedModule.Commands)
                _map.AddCommand(cmd);

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
                    var moduleAttr = typeInfo.GetCustomAttribute<ModuleAttribute>();
                    if (moduleAttr != null && moduleAttr.Autoload)
                    {
                        var moduleInstance = ReflectionUtils.CreateObject(typeInfo);
                        modules.Add(LoadInternal(moduleInstance, moduleAttr, typeInfo));
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
                    _map.RemoveCommand(cmd);
                return true;
            }
            else
                return false;
        }

        public SearchResult Search(IMessage message, int argPos) => Search(message, message.Content.Substring(argPos));
        public SearchResult Search(IMessage message, string input)
        {
            string lowerInput = input.ToLowerInvariant();
            var matches = _map.GetCommands(input).ToImmutableArray();
            
            if (matches.Length > 0)
                return SearchResult.FromSuccess(input, matches);
            else
                return SearchResult.FromError(CommandError.UnknownCommand, "Unknown command.");
        }

        public Task<IResult> Execute(IMessage message, int argPos) => Execute(message, message.Content.Substring(argPos));
        public async Task<IResult> Execute(IMessage message, string input)
        {
            var searchResult = Search(message, input);
            if (!searchResult.IsSuccess)
                return searchResult;

            var commands = searchResult.Commands;
            for (int i = commands.Count - 1; i >= 0; i--)
            {
                var parseResult = await commands[i].Parse(message, searchResult);
                if (!parseResult.IsSuccess)
                {
                    if (commands.Count == 1)
                        return parseResult;
                    else
                        continue;
                }
                var executeResult = await commands[i].Execute(message, parseResult);
                return executeResult;
            }
            
            return ParseResult.FromError(CommandError.ParseFailed, "This input does not match any overload.");
        }
    }
}
