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
        private readonly ConcurrentDictionary<Type, Module> _modules;
        private readonly ConcurrentDictionary<Type, TypeReader> _typeReaders;
        private readonly CommandMap _map;

        public IEnumerable<Module> Modules => _modules.Select(x => x.Value);
        public IEnumerable<Command> Commands => _modules.SelectMany(x => x.Value.Commands);

        public CommandService()
        {
            _moduleLock = new SemaphoreSlim(1, 1);
            _modules = new ConcurrentDictionary<Type, Module>();
            _map = new CommandMap();
            _typeReaders = new ConcurrentDictionary<Type, TypeReader>
            {
                [typeof(string)] = new SimpleTypeReader<string>(),
                [typeof(byte)] = new SimpleTypeReader<byte>(),
                [typeof(sbyte)] = new SimpleTypeReader<sbyte>(),
                [typeof(ushort)] = new SimpleTypeReader<ushort>(),
                [typeof(short)] = new SimpleTypeReader<short>(),
                [typeof(uint)] = new SimpleTypeReader<uint>(),
                [typeof(int)] = new SimpleTypeReader<int>(),
                [typeof(ulong)] = new SimpleTypeReader<ulong>(),
                [typeof(long)] = new SimpleTypeReader<long>(),
                [typeof(float)] = new SimpleTypeReader<float>(),
                [typeof(double)] = new SimpleTypeReader<double>(),
                [typeof(decimal)] = new SimpleTypeReader<decimal>(),
                [typeof(DateTime)] = new SimpleTypeReader<DateTime>(),
                [typeof(DateTimeOffset)] = new SimpleTypeReader<DateTimeOffset>(),
                
                [typeof(IMessage)] = new MessageTypeReader<IMessage>(),
                [typeof(IUserMessage)] = new MessageTypeReader<IUserMessage>(),
                //[typeof(ISystemMessage)] = new MessageTypeReader<ISystemMessage>(),
                [typeof(IChannel)] = new ChannelTypeReader<IChannel>(),
                [typeof(IDMChannel)] = new ChannelTypeReader<IDMChannel>(),
                [typeof(IGroupChannel)] = new ChannelTypeReader<IGroupChannel>(),
                [typeof(IGuildChannel)] = new ChannelTypeReader<IGuildChannel>(),
                [typeof(IMessageChannel)] = new ChannelTypeReader<IMessageChannel>(),
                [typeof(IPrivateChannel)] = new ChannelTypeReader<IPrivateChannel>(),
                [typeof(ITextChannel)] = new ChannelTypeReader<ITextChannel>(),
                [typeof(IVoiceChannel)] = new ChannelTypeReader<IVoiceChannel>(),

                //[typeof(IGuild)] = new GuildTypeReader<IGuild>(),

                [typeof(IRole)] = new RoleTypeReader<IRole>(),

                //[typeof(IInvite)] = new InviteTypeReader<IInvite>(),
                //[typeof(IInviteMetadata)] = new InviteTypeReader<IInviteMetadata>(),

                [typeof(IUser)] = new UserTypeReader<IUser>(),
                [typeof(IGroupUser)] = new UserTypeReader<IGroupUser>(),
                [typeof(IGuildUser)] = new UserTypeReader<IGuildUser>(),
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
                if (_modules.ContainsKey(moduleInstance.GetType()))
                    throw new ArgumentException($"This module has already been loaded.");

                var typeInfo = moduleInstance.GetType().GetTypeInfo();
                var moduleAttr = typeInfo.GetCustomAttribute<ModuleAttribute>();
                if (moduleAttr == null)
                    throw new ArgumentException($"Modules must be marked with ModuleAttribute.");

                return LoadInternal(moduleInstance, moduleAttr, typeInfo, null);
            }
            finally
            {
                _moduleLock.Release();
            }
        }
        private Module LoadInternal(object moduleInstance, ModuleAttribute moduleAttr, TypeInfo typeInfo, IDependencyMap dependencyMap)
        {
            if (_modules.ContainsKey(moduleInstance.GetType()))
                return _modules[moduleInstance.GetType()];

            var loadedModule = new Module(typeInfo, this, moduleInstance, moduleAttr, dependencyMap);
            _modules[moduleInstance.GetType()] = loadedModule;

            foreach (var cmd in loadedModule.Commands)
                _map.AddCommand(cmd);

            return loadedModule;
        }
        public async Task<IEnumerable<Module>> LoadAssembly(Assembly assembly, IDependencyMap dependencyMap = null)
        {
            var modules = ImmutableArray.CreateBuilder<Module>();
            await _moduleLock.WaitAsync().ConfigureAwait(false);
            try
            {
                foreach (var type in assembly.ExportedTypes)
                {
                    var typeInfo = type.GetTypeInfo();
                    var moduleAttr = typeInfo.GetCustomAttribute<ModuleAttribute>();
                    if (moduleAttr != null && moduleAttr.AutoLoad)
                    {
                        var moduleInstance = ReflectionUtils.CreateObject(typeInfo, this, dependencyMap);
                        modules.Add(LoadInternal(moduleInstance, moduleAttr, typeInfo, dependencyMap));
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
            if (_modules.TryRemove(module.GetType(), out unloadedModule))
            {
                foreach (var cmd in unloadedModule.Commands)
                    _map.RemoveCommand(cmd);
                return true;
            }
            else
                return false;
        }

        public SearchResult Search(IUserMessage message, int argPos) => Search(message, message.Content.Substring(argPos));
        public SearchResult Search(IUserMessage message, string input)
        {
            string lowerInput = input.ToLowerInvariant();
            var matches = _map.GetCommands(input).ToImmutableArray();
            
            if (matches.Length > 0)
                return SearchResult.FromSuccess(input, matches);
            else
                return SearchResult.FromError(CommandError.UnknownCommand, "Unknown command.");
        }

        public Task<IResult> Execute(IUserMessage message, int argPos, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception) 
            => Execute(message, message.Content.Substring(argPos), multiMatchHandling);
        public async Task<IResult> Execute(IUserMessage message, string input, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception)
        {
            var searchResult = Search(message, input);
            if (!searchResult.IsSuccess)
                return searchResult;

            var commands = searchResult.Commands;
            for (int i = commands.Count - 1; i >= 0; i--)
            {
                var preconditionResult = await commands[i].CheckPreconditions(message);
                if (!preconditionResult.IsSuccess)
                {
                    if (commands.Count == 1)
                        return preconditionResult;
                    else
                        continue;
                }

                var parseResult = await commands[i].Parse(message, searchResult, preconditionResult);
                if (!parseResult.IsSuccess)
                {
                    if (parseResult.Error == CommandError.MultipleMatches)
                    {
                        TypeReaderValue[] argList, paramList;
                        switch (multiMatchHandling)
                        {
                            case MultiMatchHandling.Best:
                                argList = parseResult.ArgValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToArray();
                                paramList = parseResult.ParamValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToArray();
                                parseResult = ParseResult.FromSuccess(argList, paramList);
                                break;
                        }
                    }

                    if (!parseResult.IsSuccess)
                    {
                        if (commands.Count == 1)
                            return parseResult;
                        else
                            continue;
                    }
                }

                return await commands[i].Execute(message, parseResult);
            }
            
            return SearchResult.FromError(CommandError.UnknownCommand, "This input does not match any overload.");
        }
    }
}
