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
        private static readonly TypeInfo _moduleTypeInfo = typeof(ModuleBase).GetTypeInfo();

        private readonly SemaphoreSlim _moduleLock;
        private readonly ConcurrentDictionary<Type, ModuleInfo> _moduleDefs;
        private readonly ConcurrentDictionary<Type, TypeReader> _typeReaders;
        private readonly CommandMap _map;

        public IEnumerable<ModuleInfo> Modules => _moduleDefs.Select(x => x.Value);
        public IEnumerable<CommandInfo> Commands => _moduleDefs.SelectMany(x => x.Value.Commands);

        public CommandService()
        {
            _moduleLock = new SemaphoreSlim(1, 1);
            _moduleDefs = new ConcurrentDictionary<Type, ModuleInfo>();
            _map = new CommandMap();
            _typeReaders = new ConcurrentDictionary<Type, TypeReader>
            {
                [typeof(bool)] = new SimpleTypeReader<bool>(),
                [typeof(char)] = new SimpleTypeReader<char>(),
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
                [typeof(IChannel)] = new ChannelTypeReader<IChannel>(),
                [typeof(IDMChannel)] = new ChannelTypeReader<IDMChannel>(),
                [typeof(IGroupChannel)] = new ChannelTypeReader<IGroupChannel>(),
                [typeof(IGuildChannel)] = new ChannelTypeReader<IGuildChannel>(),
                [typeof(IMessageChannel)] = new ChannelTypeReader<IMessageChannel>(),
                [typeof(IPrivateChannel)] = new ChannelTypeReader<IPrivateChannel>(),
                [typeof(ITextChannel)] = new ChannelTypeReader<ITextChannel>(),
                [typeof(IVoiceChannel)] = new ChannelTypeReader<IVoiceChannel>(),

                [typeof(IRole)] = new RoleTypeReader<IRole>(),

                [typeof(IUser)] = new UserTypeReader<IUser>(),
                [typeof(IGroupUser)] = new UserTypeReader<IGroupUser>(),
                [typeof(IGuildUser)] = new UserTypeReader<IGuildUser>(),
            };
        }

        //Modules
        public async Task<ModuleInfo> AddModule<T>()
        {
            await _moduleLock.WaitAsync().ConfigureAwait(false);
            try
            {
                var typeInfo = typeof(T).GetTypeInfo();
                if (!_moduleTypeInfo.IsAssignableFrom(typeInfo))
                    throw new ArgumentException($"Modules must inherit ModuleBase.");

                if (typeInfo.IsAbstract)
                    throw new InvalidOperationException("Modules must not be abstract.");

                if (_moduleDefs.ContainsKey(typeof(T)))
                    throw new ArgumentException($"This module has already been added.");

                return AddModuleInternal(typeInfo);
            }
            finally
            {
                _moduleLock.Release();
            }
        }
        public async Task<IEnumerable<ModuleInfo>> AddModules(Assembly assembly)
        {
            var moduleDefs = ImmutableArray.CreateBuilder<ModuleInfo>();
            await _moduleLock.WaitAsync().ConfigureAwait(false);
            try
            {
                foreach (var type in assembly.ExportedTypes)
                {
                    //Ensure that we weren't declared as a submodule
                    if (type.DeclaringType != null)
                    {
                        if (_moduleDefs.ContainsKey(type.DeclaringType))
                            continue;

                        var declaringTypeInfo = type.DeclaringType.GetTypeInfo();
                        if (_moduleTypeInfo.IsAssignableFrom(declaringTypeInfo))
                            continue;
                    }
                    if (!_moduleDefs.ContainsKey(type))
                    {
                        var typeInfo = type.GetTypeInfo();
                        if (_moduleTypeInfo.IsAssignableFrom(typeInfo))
                        {
                            var dontAutoLoad = typeInfo.GetCustomAttribute<DontAutoLoadAttribute>();
                            if (dontAutoLoad == null && !typeInfo.IsAbstract)
                                moduleDefs.Add(AddModuleInternal(typeInfo));
                        }
                    }
                }
                return moduleDefs.ToImmutable();
            }
            finally
            {
                _moduleLock.Release();
            }
        }
        private ModuleInfo AddModuleInternal(TypeInfo typeInfo)
        {
            var moduleDef = new ModuleInfo(typeInfo, this);
            _moduleDefs[typeInfo.AsType()] = moduleDef;

            foreach (var cmd in moduleDef.Commands)
                _map.AddCommand(cmd);

            return moduleDef;
        }

        public async Task<bool> RemoveModule(ModuleInfo module)
        {
            await _moduleLock.WaitAsync().ConfigureAwait(false);
            try
            {
                return RemoveModuleInternal(module.Source.BaseType);
            }
            finally
            {
                _moduleLock.Release();
            }
        }
        public async Task<bool> RemoveModule<T>()
        {
            await _moduleLock.WaitAsync().ConfigureAwait(false);
            try
            {
                return RemoveModuleInternal(typeof(T));
            }
            finally
            {
                _moduleLock.Release();
            }
        }
        private bool RemoveModuleInternal(Type type)
        {
            ModuleInfo unloadedModule;
            if (_moduleDefs.TryRemove(type, out unloadedModule))
            {
                foreach (var cmd in unloadedModule.Commands)
                    _map.RemoveCommand(cmd);
                return true;
            }
            else
                return false;
        }

        //Type Readers
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

        //Execution
        public SearchResult Search(CommandContext context, int argPos) => Search(context, context.Message.Content.Substring(argPos));
        public SearchResult Search(CommandContext context, string input)
        {
            string lowerInput = input.ToLowerInvariant();
            var matches = _map.GetCommands(input).OrderByDescending(x => x.Priority).ToImmutableArray();
            
            if (matches.Length > 0)
                return SearchResult.FromSuccess(input, matches);
            else
                return SearchResult.FromError(CommandError.UnknownCommand, "Unknown command.");
        }

        public Task<IResult> Execute(CommandContext context, int argPos, IDependencyMap dependencyMap = null, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception) 
            => Execute(context, context.Message.Content.Substring(argPos), dependencyMap, multiMatchHandling);
        public async Task<IResult> Execute(CommandContext context, string input, IDependencyMap dependencyMap = null, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception)
        {
            dependencyMap = dependencyMap ?? DependencyMap.Empty;

            var searchResult = Search(context, input);
            if (!searchResult.IsSuccess)
                return searchResult;

            var commands = searchResult.Commands;
            for (int i = commands.Count - 1; i >= 0; i--)
            {
                var preconditionResult = await commands[i].CheckPreconditions(context, dependencyMap).ConfigureAwait(false);
                if (!preconditionResult.IsSuccess)
                {
                    if (commands.Count == 1)
                        return preconditionResult;
                    else
                        continue;
                }

                var parseResult = await commands[i].Parse(context, searchResult, preconditionResult).ConfigureAwait(false);
                if (!parseResult.IsSuccess)
                {
                    if (parseResult.Error == CommandError.MultipleMatches)
                    {
                        IReadOnlyList<TypeReaderValue> argList, paramList;
                        switch (multiMatchHandling)
                        {
                            case MultiMatchHandling.Best:
                                argList = parseResult.ArgValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
                                paramList = parseResult.ParamValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
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

                return await commands[i].Execute(context, parseResult, dependencyMap).ConfigureAwait(false);
            }
            
            return SearchResult.FromError(CommandError.UnknownCommand, "This input does not match any overload.");
        }
    }
}
