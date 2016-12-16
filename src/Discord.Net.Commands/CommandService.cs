using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Discord.Commands.Builders;

namespace Discord.Commands
{
    public class CommandService
    {
        private readonly SemaphoreSlim _moduleLock;
        private readonly ConcurrentDictionary<Type, ModuleInfo> _typedModuleDefs;
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, TypeReader>> _typeReaders;
        private readonly ConcurrentDictionary<Type, TypeReader> _defaultTypeReaders;
        private readonly ConcurrentBag<ModuleInfo> _moduleDefs;
        private readonly CommandMap _map;

        internal readonly bool _caseSensitive;
        internal readonly RunMode _defaultRunMode;
        internal readonly char _splitCharacter;

        public IEnumerable<ModuleInfo> Modules => _moduleDefs.Select(x => x);
        public IEnumerable<CommandInfo> Commands => _moduleDefs.SelectMany(x => x.Commands);
        public ILookup<Type, TypeReader> TypeReaders => _typeReaders.SelectMany(x => x.Value.Select(y => new {y.Key, y.Value})).ToLookup(x => x.Key, x => x.Value);

        public CommandService() : this(new CommandServiceConfig()) { }
        public CommandService(CommandServiceConfig config)
        {
            _moduleLock = new SemaphoreSlim(1, 1);
            _typedModuleDefs = new ConcurrentDictionary<Type, ModuleInfo>();
            _moduleDefs = new ConcurrentBag<ModuleInfo>();
            _map = new CommandMap();
            _typeReaders = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, TypeReader>>();

            _defaultTypeReaders = new ConcurrentDictionary<Type, TypeReader>
            {                
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
            foreach (var type in PrimitiveParsers.SupportedTypes)
                _defaultTypeReaders[type] = PrimitiveTypeReader.Create(type);

            _caseSensitive = config.CaseSensitiveCommands;
            _defaultRunMode = config.DefaultRunMode;
            _splitCharacter = config.CommandSplitCharacter;
        }

        //Modules
        public async Task<ModuleInfo> CreateModuleAsync(string primaryAlias, Action<ModuleBuilder> buildFunc)
        {
            await _moduleLock.WaitAsync().ConfigureAwait(false);
            try
            {
                var builder = new ModuleBuilder(this, null, primaryAlias);
                buildFunc(builder);

                var module = builder.Build(this);
                return LoadModuleInternal(module);
            }
            finally
            {
                _moduleLock.Release();
            }
        }
        public async Task<ModuleInfo> AddModuleAsync<T>()
        {
            await _moduleLock.WaitAsync().ConfigureAwait(false);
            try
            {
                var typeInfo = typeof(T).GetTypeInfo();

                if (_typedModuleDefs.ContainsKey(typeof(T)))
                    throw new ArgumentException($"This module has already been added.");

                var module = ModuleClassBuilder.Build(this, typeInfo).FirstOrDefault();

                if (module.Value == default(ModuleInfo))
                    throw new InvalidOperationException($"Could not build the module {typeof(T).FullName}, did you pass an invalid type?");

                _typedModuleDefs[module.Key] = module.Value;

                return LoadModuleInternal(module.Value);
            }
            finally
            {
                _moduleLock.Release();
            }
        }
        public async Task<IEnumerable<ModuleInfo>> AddModulesAsync(Assembly assembly)
        {
            await _moduleLock.WaitAsync().ConfigureAwait(false);
            try
            {
                var types = ModuleClassBuilder.Search(assembly).ToArray();
                var moduleDefs = ModuleClassBuilder.Build(types, this);

                foreach (var info in moduleDefs)
                {
                    _typedModuleDefs[info.Key] = info.Value;
                    LoadModuleInternal(info.Value);
                }

                return moduleDefs.Select(x => x.Value).ToImmutableArray();
            }
            finally
            {
                _moduleLock.Release();
            }
        }
        private ModuleInfo LoadModuleInternal(ModuleInfo module)
        {
            _moduleDefs.Add(module);

            foreach (var command in module.Commands)
                _map.AddCommand(command, this);

            foreach (var submodule in module.Submodules)
                LoadModuleInternal(submodule);

            return module;
        }

        public async Task<bool> RemoveModuleAsync(ModuleInfo module)
        {
            await _moduleLock.WaitAsync().ConfigureAwait(false);
            try
            {
                return RemoveModuleInternal(module);
            }
            finally
            {
                _moduleLock.Release();
            }
        }
        public async Task<bool> RemoveModuleAsync<T>()
        {
            await _moduleLock.WaitAsync().ConfigureAwait(false);
            try
            {
                ModuleInfo module;
                _typedModuleDefs.TryGetValue(typeof(T), out module);
                if (module == default(ModuleInfo))
                    return false;

                return RemoveModuleInternal(module);
            }
            finally
            {
                _moduleLock.Release();
            }
        }
        private bool RemoveModuleInternal(ModuleInfo module)
        {
            var defsRemove = module;
            if (!_moduleDefs.TryTake(out defsRemove))
                return false;

            foreach (var cmd in module.Commands)
                _map.RemoveCommand(cmd, this);

            foreach (var submodule in module.Submodules)
            {
                RemoveModuleInternal(submodule);
            }

            return true;
        }

        //Type Readers
        public void AddTypeReader<T>(TypeReader reader)
        {
            var readers = _typeReaders.GetOrAdd(typeof(T), x => new ConcurrentDictionary<Type, TypeReader>());
            readers[reader.GetType()] = reader;
        }
        public void AddTypeReader(Type type, TypeReader reader)
        {
            var readers = _typeReaders.GetOrAdd(type, x=> new ConcurrentDictionary<Type, TypeReader>());
            readers[reader.GetType()] = reader;
        }
        internal IDictionary<Type, TypeReader> GetTypeReaders(Type type)
        {
            ConcurrentDictionary<Type, TypeReader> definedTypeReaders;
            if (_typeReaders.TryGetValue(type, out definedTypeReaders))
                return definedTypeReaders;
            return null;
        }
        internal TypeReader GetDefaultTypeReader(Type type)
        {
            TypeReader reader;
            if (_defaultTypeReaders.TryGetValue(type, out reader))
                return reader;
            return null;
        }

        //Execution
        public SearchResult Search(CommandContext context, int argPos) => Search(context, context.Message.Content.Substring(argPos));
        public SearchResult Search(CommandContext context, string input)
        {
            string searchInput = _caseSensitive ? input : input.ToLowerInvariant();
            var matches = _map.GetCommands(searchInput, this).OrderByDescending(x => x.Priority).ToImmutableArray();
            
            if (matches.Length > 0)
                return SearchResult.FromSuccess(input, matches);
            else
                return SearchResult.FromError(CommandError.UnknownCommand, "Unknown command.");
        }

        public Task<IResult> ExecuteAsync(CommandContext context, int argPos, IDependencyMap dependencyMap = null, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception)
            => ExecuteAsync(context, context.Message.Content.Substring(argPos), dependencyMap, multiMatchHandling);
        public async Task<IResult> ExecuteAsync(CommandContext context, string input, IDependencyMap dependencyMap = null, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception)
        {
            dependencyMap = dependencyMap ?? DependencyMap.Empty;

            var searchResult = Search(context, input);
            if (!searchResult.IsSuccess)
                return searchResult;

            var commands = searchResult.Commands;
            for (int i = commands.Count - 1; i >= 0; i--)
            {
                var preconditionResult = await commands[i].CheckPreconditionsAsync(context, dependencyMap).ConfigureAwait(false);
                if (!preconditionResult.IsSuccess)
                {
                    if (commands.Count == 1)
                        return preconditionResult;
                    else
                        continue;
                }

                var parseResult = await commands[i].ParseAsync(context, searchResult, preconditionResult).ConfigureAwait(false);
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
