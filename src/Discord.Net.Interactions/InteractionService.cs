using Discord.Interactions.Builders;
using Discord.Logging;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Provides the framework for building and registering Discord Application Commands.
    /// </summary>
    public class InteractionService : IDisposable
    {
        /// <summary>
        ///     Occurs when a Slash Command related information is recieved.
        /// </summary>
        public event Func<LogMessage, Task> Log { add { _logEvent.Add(value); } remove { _logEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new ();

        /// <summary>
        ///     Occurs when a Slash Command is executed.
        /// </summary>
        public event Func<SlashCommandInfo, IInteractionContext, IResult, Task> SlashCommandExecuted { add { _slashCommandExecutedEvent.Add(value); } remove { _slashCommandExecutedEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<SlashCommandInfo, IInteractionContext, IResult, Task>> _slashCommandExecutedEvent = new ();

        /// <summary>
        ///     Occurs when a Context Command is executed.
        /// </summary>
        public event Func<ContextCommandInfo, IInteractionContext, IResult, Task> ContextCommandExecuted { add { _contextCommandExecutedEvent.Add(value); } remove { _contextCommandExecutedEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<ContextCommandInfo, IInteractionContext, IResult, Task>> _contextCommandExecutedEvent = new ();

        /// <summary>
        ///     Occurs when a Message Component command is executed.
        /// </summary>
        public event Func<ComponentCommandInfo, IInteractionContext, IResult, Task> ComponentCommandExecuted { add { _componentCommandExecutedEvent.Add(value); } remove { _componentCommandExecutedEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<ComponentCommandInfo, IInteractionContext, IResult, Task>> _componentCommandExecutedEvent = new ();

        /// <summary>
        ///     Occurs when a Autocomplete command is executed.
        /// </summary>
        public event Func<AutocompleteCommandInfo, IInteractionContext, IResult, Task> AutocompleteCommandExecuted { add { _autocompleteCommandExecutedEvent.Add(value); } remove { _autocompleteCommandExecutedEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<AutocompleteCommandInfo, IInteractionContext, IResult, Task>> _autocompleteCommandExecutedEvent = new();

        /// <summary>
        ///     Occurs when a AutocompleteHandler is executed.
        /// </summary>
        public event Func<IAutocompleteHandler, IInteractionContext, IResult, Task> AutocompleteHandlerExecuted { add { _autocompleteHandlerExecutedEvent.Add(value); } remove { _autocompleteHandlerExecutedEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<IAutocompleteHandler, IInteractionContext, IResult, Task>> _autocompleteHandlerExecutedEvent = new();

        private readonly ConcurrentDictionary<Type, ModuleInfo> _typedModuleDefs;
        private readonly CommandMap<SlashCommandInfo> _slashCommandMap;
        private readonly ConcurrentDictionary<ApplicationCommandType, CommandMap<ContextCommandInfo>> _contextCommandMaps;
        private readonly CommandMap<ComponentCommandInfo> _componentCommandMap;
        private readonly CommandMap<AutocompleteCommandInfo> _autocompleteCommandMap;
        private readonly HashSet<ModuleInfo> _moduleDefs;
        private readonly ConcurrentDictionary<Type, TypeConverter> _typeConverters;
        private readonly ConcurrentDictionary<Type, Type> _genericTypeConverters;
        private readonly ConcurrentDictionary<Type, IAutocompleteHandler> _autocompleteHandlers = new();
        private readonly SemaphoreSlim _lock;
        internal readonly Logger _cmdLogger;
        internal readonly LogManager _logManager;
        internal readonly Func<DiscordRestClient> _getRestClient;

        internal readonly bool _throwOnError, _useCompiledLambda, _enableAutocompleteHandlers;
        internal readonly string _wildCardExp;
        internal readonly RunMode _runMode;
        internal readonly RestResponseCallback _restResponseCallback;

        /// <summary>
        ///     Rest client to be used to register application commands.
        /// </summary>
        public DiscordRestClient RestClient { get => _getRestClient(); }

        /// <summary>
        ///     Represents all modules loaded within <see cref="InteractionService"/>.
        /// </summary>
        public IReadOnlyList<ModuleInfo> Modules => _moduleDefs.ToList();

        /// <summary>
        ///     Represents all Slash Commands loaded within <see cref="InteractionService"/>.
        /// </summary>
        public IReadOnlyList<SlashCommandInfo> SlashCommands => _moduleDefs.SelectMany(x => x.SlashCommands).ToList();

        /// <summary>
        ///     Represents all Context Commands loaded within <see cref="InteractionService"/>.
        /// </summary>
        public IReadOnlyList<ContextCommandInfo> ContextCommands => _moduleDefs.SelectMany(x => x.ContextCommands).ToList();

        /// <summary>
        ///     Represents all Component Commands loaded within <see cref="InteractionService"/>.
        /// </summary>
        public IReadOnlyCollection<ComponentCommandInfo> ComponentCommands => _moduleDefs.SelectMany(x => x.ComponentCommands).ToList();

        /// <summary>
        ///     Initialize a <see cref="InteractionService"/> with provided configurations.
        /// </summary>
        /// <param name="discord">The discord client.</param>
        /// <param name="config">The configuration class.</param>
        public InteractionService (DiscordSocketClient discord, InteractionServiceConfig config = null)
            : this(() => discord.Rest, config ?? new InteractionServiceConfig()) { }

        /// <summary>
        ///     Initialize a <see cref="InteractionService"/> with provided configurations.
        /// </summary>
        /// <param name="discord">The discord client.</param>
        /// <param name="config">The configuration class.</param>
        public InteractionService (DiscordShardedClient discord, InteractionServiceConfig config = null)
            : this(() => discord.Rest, config ?? new InteractionServiceConfig()) { }

        /// <summary>
        ///     Initialize a <see cref="InteractionService"/> with provided configurations.
        /// </summary>
        /// <param name="discord">The discord client.</param>
        /// <param name="config">The configuration class.</param>
        public InteractionService (BaseSocketClient discord, InteractionServiceConfig config = null)
            :this(() => discord.Rest, config ?? new InteractionServiceConfig()) { }

        /// <summary>
        ///     Initialize a <see cref="InteractionService"/> with provided configurations.
        /// </summary>
        /// <param name="discord">The discord client.</param>
        /// <param name="config">The configuration class.</param>
        public InteractionService (DiscordRestClient discord, InteractionServiceConfig config = null)
            :this(() => discord, config ?? new InteractionServiceConfig()) { }

        private InteractionService (Func<DiscordRestClient> getRestClient, InteractionServiceConfig config = null)
        {
            config ??= new InteractionServiceConfig();

            _lock = new SemaphoreSlim(1, 1);
            _typedModuleDefs = new ConcurrentDictionary<Type, ModuleInfo>();
            _moduleDefs = new HashSet<ModuleInfo>();

            _logManager = new LogManager(config.LogLevel);
            _logManager.Message += async msg => await _logEvent.InvokeAsync(msg).ConfigureAwait(false);
            _cmdLogger = _logManager.CreateLogger("App Commands");

            _slashCommandMap = new CommandMap<SlashCommandInfo>(this);
            _contextCommandMaps = new ConcurrentDictionary<ApplicationCommandType, CommandMap<ContextCommandInfo>>();
            _componentCommandMap = new CommandMap<ComponentCommandInfo>(this, config.InteractionCustomIdDelimiters);
            _autocompleteCommandMap = new CommandMap<AutocompleteCommandInfo>(this);

            _getRestClient = getRestClient;

            _runMode = config.DefaultRunMode;
            if (_runMode == RunMode.Default)
                throw new InvalidOperationException($"RunMode cannot be set to {RunMode.Default}");

            _throwOnError = config.ThrowOnError;
            _wildCardExp = config.WildCardExpression;
            _useCompiledLambda = config.UseCompiledLambda;
            _enableAutocompleteHandlers = config.EnableAutocompleteHandlers;
            _restResponseCallback = config.RestResponseCallback;

            _genericTypeConverters = new ConcurrentDictionary<Type, Type>
            {
                [typeof(IChannel)] = typeof(DefaultChannelConverter<>),
                [typeof(IRole)] = typeof(DefaultRoleConverter<>),
                [typeof(IUser)] = typeof(DefaultUserConverter<>),
                [typeof(IMentionable)] = typeof(DefaultMentionableConverter<>),
                [typeof(IConvertible)] = typeof(DefaultValueConverter<>),
                [typeof(Enum)] = typeof(EnumConverter<>)
            };

            _typeConverters = new ConcurrentDictionary<Type, TypeConverter>
            {
                [typeof(TimeSpan)] = new TimeSpanConverter()
            };
        }

        /// <summary>
        ///     Create and loads a <see cref="ModuleInfo"/> using a builder factory.
        /// </summary>
        /// <param name="name">Name of the module.</param>
        /// <param name="services">The <see cref="IServiceProvider"/> for your dependency injection solution if using one; otherwise, pass <c>null</c>.</param>
        /// <param name="buildFunc">Module builder factory.</param>
        /// <returns>
        ///     A task representing the operation for adding modules. The task result contains the built module instance.
        /// </returns>
        public async Task<ModuleInfo> CreateModuleAsync(string name, IServiceProvider services, Action<ModuleBuilder> buildFunc)
        {
            services ??= EmptyServiceProvider.Instance;

            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                var builder = new ModuleBuilder(this, name);
                buildFunc(builder);

                var moduleInfo = builder.Build(this, services);
                LoadModuleInternal(moduleInfo);

                return moduleInfo;
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        ///     Discover and load command modules from an <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly"><see cref="Assembly"/> the command modules are defined in.</param>
        /// <param name="services">The <see cref="IServiceProvider"/> for your dependency injection solution if using one; otherwise, pass <c>null</c>.</param>
        /// <returns>
        ///     A task representing the operation for adding modules. The task result contains a collection of the modules added.
        /// </returns>
        public async Task<IEnumerable<ModuleInfo>> AddModulesAsync (Assembly assembly, IServiceProvider services)
        {
            services ??= EmptyServiceProvider.Instance;

            await _lock.WaitAsync().ConfigureAwait(false);

            try
            {
                var types = await ModuleClassBuilder.SearchAsync(assembly, this);
                var moduleDefs = await ModuleClassBuilder.BuildAsync(types, this, services);

                foreach (var info in moduleDefs)
                {
                    _typedModuleDefs[info.Key] = info.Value;
                    LoadModuleInternal(info.Value);
                }
                return moduleDefs.Values;
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        ///     Add a command module from a <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">Type of the module.</typeparam>
        /// <param name="services">The <see cref="IServiceProvider" /> for your dependency injection solution if using one; otherwise, pass <c>null</c> .</param>
        /// <returns>
        ///     A task representing the operation for adding the module. The task result contains the built module.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Thrown if this module has already been added.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the <typeparamref name="T"/> is not a valid module definition.
        /// </exception>
        public Task<ModuleInfo> AddModuleAsync<T> (IServiceProvider services) where T : class =>
            AddModuleAsync(typeof(T), services);

        /// <summary>
        ///     Add a command module from a <see cref="Type"/>.
        /// </summary>
        /// <param name="type">Type of the module.</param>
        /// <param name="services">The <see cref="IServiceProvider" /> for your dependency injection solution if using one; otherwise, pass <c>null</c> .</param>
        /// <returns>
        ///     A task representing the operation for adding the module. The task result contains the built module.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Thrown if this module has already been added.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the <paramref name="type"/> is not a valid module definition.
        /// </exception>
        public async Task<ModuleInfo> AddModuleAsync (Type type, IServiceProvider services)
        {
            if (!typeof(IInteractionModuleBase).IsAssignableFrom(type))
                throw new ArgumentException("Type parameter must be a type of Slash Module", "T");

            services ??= EmptyServiceProvider.Instance;

            await _lock.WaitAsync().ConfigureAwait(false);

            try
            {
                var typeInfo = type.GetTypeInfo();

                if (_typedModuleDefs.ContainsKey(typeInfo))
                    throw new ArgumentException("Module definition for this type already exists.");

                var moduleDef = ( await ModuleClassBuilder.BuildAsync(new List<TypeInfo> { typeInfo }, this, services).ConfigureAwait(false) ).FirstOrDefault();

                if (moduleDef.Value == default)
                    throw new InvalidOperationException($"Could not build the module {typeInfo.FullName}, did you pass an invalid type?");

                if (!_typedModuleDefs.TryAdd(type, moduleDef.Value))
                    throw new ArgumentException("Module definition for this type already exists.");

                _typedModuleDefs[moduleDef.Key] = moduleDef.Value;
                LoadModuleInternal(moduleDef.Value);

                return moduleDef.Value;
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        ///     Register Application Commands from <see cref="ContextCommands"/> and <see cref="SlashCommands"/> to a guild. 
        /// </summary>
        /// <param name="guildId">Id of the target guild.</param>
        /// <param name="deleteMissing">If <see langword="false"/>, this operation will not delete the commands that are missing from <see cref="InteractionService"/>.</param>
        /// <returns>
        ///     A task representing the command registration process. The task result contains the active application commands of the target guild.
        /// </returns>
        public async Task<IReadOnlyCollection<RestGuildCommand>> RegisterCommandsToGuildAsync (ulong guildId, bool deleteMissing = true)
        {
            EnsureClientReady();

            var topLevelModules = _moduleDefs.Where(x => !x.IsSubModule);
            var props = topLevelModules.SelectMany(x => x.ToApplicationCommandProps()).ToList();

            if (!deleteMissing)
            {

                var existing = await RestClient.GetGuildApplicationCommands(guildId).ConfigureAwait(false);
                var missing = existing.Where(x => !props.Any(y => y.Name.IsSpecified && y.Name.Value == x.Name));
                props.AddRange(missing.Select(x => x.ToApplicationCommandProps()));
            }

            return await RestClient.BulkOverwriteGuildCommands(props.ToArray(), guildId).ConfigureAwait(false);
        }

        /// <summary>
        ///     Register Application Commands from <see cref="ContextCommands"/> and <see cref="SlashCommands"/> to Discord on in global scope.
        /// </summary>
        /// <param name="deleteMissing">If <see langword="false"/>, this operation will not delete the commands that are missing from <see cref="InteractionService"/>.</param>
        /// <returns>
        ///    A task representing the command registration process. The task result contains the active global application commands of bot.
        /// </returns>
        public async Task<IReadOnlyCollection<RestGlobalCommand>> RegisterCommandsGloballyAsync (bool deleteMissing = true)
        {
            EnsureClientReady();

            var topLevelModules = _moduleDefs.Where(x => !x.IsSubModule);
            var props = topLevelModules.SelectMany(x => x.ToApplicationCommandProps()).ToList();

            if (!deleteMissing)
            {
                var existing = await RestClient.GetGlobalApplicationCommands().ConfigureAwait(false);
                var missing = existing.Where(x => !props.Any(y => y.Name.IsSpecified && y.Name.Value == x.Name));
                props.AddRange(missing.Select(x => x.ToApplicationCommandProps()));
            }

            return await RestClient.BulkOverwriteGlobalCommands(props.ToArray()).ConfigureAwait(false);
        }

        /// <summary>
        ///     Register Application Commands from <paramref name="commands"/> to a guild.
        /// </summary>
        /// <remarks>
        ///     Commands will be registered as standalone commands, if you want the <see cref="GroupAttribute"/> to take effect,
        ///     use <see cref="AddModulesToGuildAsync(IGuild, ModuleInfo[])"/>. Registering a commands without group names might cause the command traversal to fail.
        /// </remarks>
        /// <param name="guild">The target guild.</param>
        /// <param name="commands">Commands to be registered to Discord.</param>
        /// <returns>
        ///     A task representing the command registration process. The task result contains the active application commands of the target guild.
        /// </returns>
        public async Task<IReadOnlyCollection<RestGuildCommand>> AddCommandsToGuildAsync(IGuild guild, bool deleteMissing = false, params ICommandInfo[] commands)
        {
            EnsureClientReady();

            if (guild is null)
                throw new ArgumentNullException(nameof(guild));

            var props = new List<ApplicationCommandProperties>();

            foreach (var command in commands)
            {
                switch (command)
                {
                    case SlashCommandInfo slashCommand:
                        props.Add(slashCommand.ToApplicationCommandProps());
                        break;
                    case ContextCommandInfo contextCommand:
                        props.Add(contextCommand.ToApplicationCommandProps());
                        break;
                    default:
                        throw new InvalidOperationException($"Command type {command.GetType().FullName} isn't supported yet");
                }
            }

            if (!deleteMissing)
            {
                var existing = await RestClient.GetGuildApplicationCommands(guild.Id).ConfigureAwait(false);
                var missing = existing.Where(x => !props.Any(y => y.Name.IsSpecified && y.Name.Value == x.Name));
                props.AddRange(missing.Select(x => x.ToApplicationCommandProps()));
            }

            return await RestClient.BulkOverwriteGuildCommands(props.ToArray(), guild.Id).ConfigureAwait(false);
        }

        /// <summary>
        ///     Register Application Commands from modules provided in <paramref name="modules"/> to a guild. 
        /// </summary>
        /// <param name="guild">The target guild.</param>
        /// <param name="modules">Modules to be registered to Discord.</param>
        /// <returns>
        ///     A task representing the command registration process. The task result contains the active application commands of the target guild.
        /// </returns>
        public async Task<IReadOnlyCollection<RestGuildCommand>> AddModulesToGuildAsync(IGuild guild, bool deleteMissing = false, params ModuleInfo[] modules)
        {
            EnsureClientReady();

            if (guild is null)
                throw new ArgumentNullException(nameof(guild));

            var props = modules.SelectMany(x => x.ToApplicationCommandProps(true)).ToList();

            if (!deleteMissing)
            {
                var existing = await RestClient.GetGuildApplicationCommands(guild.Id).ConfigureAwait(false);
                var missing = existing.Where(x => !props.Any(y => y.Name.IsSpecified && y.Name.Value == x.Name));
                props.AddRange(missing.Select(x => x.ToApplicationCommandProps()));
            }

            return await RestClient.BulkOverwriteGuildCommands(props.ToArray(), guild.Id).ConfigureAwait(false);
        }

        /// <summary>
        ///     Register Application Commands from modules provided in <paramref name="modules"/> as global commands. 
        /// </summary>
        /// <param name="modules">Modules to be registered to Discord.</param>
        /// <returns>
        ///     A task representing the command registration process. The task result contains the active application commands of the target guild.
        /// </returns>
        public async Task<IReadOnlyCollection<RestGlobalCommand>> AddModulesGloballyAsync(bool deleteMissing = false, params ModuleInfo[] modules)
        {
            EnsureClientReady();

            var props = modules.SelectMany(x => x.ToApplicationCommandProps(true)).ToList();

            if (!deleteMissing)
            {
                var existing = await RestClient.GetGlobalApplicationCommands().ConfigureAwait(false);
                var missing = existing.Where(x => !props.Any(y => y.Name.IsSpecified && y.Name.Value == x.Name));
                props.AddRange(missing.Select(x => x.ToApplicationCommandProps()));
            }

            return await RestClient.BulkOverwriteGlobalCommands(props.ToArray()).ConfigureAwait(false);
        }

        /// <summary>
        ///     Register Application Commands from <paramref name="commands"/> as global commands.
        /// </summary>
        /// <remarks>
        ///     Commands will be registered as standalone commands, if you want the <see cref="GroupAttribute"/> to take effect,
        ///     use <see cref="AddModulesToGuildAsync(IGuild, ModuleInfo[])"/>. Registering a commands without group names might cause the command traversal to fail.
        /// </remarks>
        /// <param name="commands">Commands to be registered to Discord.</param>
        /// <returns>
        ///     A task representing the command registration process. The task result contains the active application commands of the target guild.
        /// </returns>
        public async Task<IReadOnlyCollection<RestGlobalCommand>> AddCommandsGloballyAsync(bool deleteMissing = false, params IApplicationCommandInfo[] commands)
        {
            EnsureClientReady();

            var props = new List<ApplicationCommandProperties>();

            foreach (var command in commands)
            {
                switch (command)
                {
                    case SlashCommandInfo slashCommand:
                        props.Add(slashCommand.ToApplicationCommandProps());
                        break;
                    case ContextCommandInfo contextCommand:
                        props.Add(contextCommand.ToApplicationCommandProps());
                        break;
                    default:
                        throw new InvalidOperationException($"Command type {command.GetType().FullName} isn't supported yet");
                }
            }

            if (!deleteMissing)
            {
                var existing = await RestClient.GetGlobalApplicationCommands().ConfigureAwait(false);
                var missing = existing.Where(x => !props.Any(y => y.Name.IsSpecified && y.Name.Value == x.Name));
                props.AddRange(missing.Select(x => x.ToApplicationCommandProps()));
            }

            return await RestClient.BulkOverwriteGlobalCommands(props.ToArray()).ConfigureAwait(false);
        }

        private void LoadModuleInternal (ModuleInfo module)
        {
            _moduleDefs.Add(module);

            foreach (var command in module.SlashCommands)
                _slashCommandMap.AddCommand(command, command.IgnoreGroupNames);

            foreach (var command in module.ContextCommands)
                _contextCommandMaps.GetOrAdd(command.CommandType, new CommandMap<ContextCommandInfo>(this)).AddCommand(command, command.IgnoreGroupNames);

            foreach (var interaction in module.ComponentCommands)
                _componentCommandMap.AddCommand(interaction, interaction.IgnoreGroupNames);

            foreach (var command in module.AutocompleteCommands)
                _autocompleteCommandMap.AddCommand(command.GetCommandKeywords(), command);

            foreach (var subModule in module.SubModules)
                LoadModuleInternal(subModule);
        }

        /// <summary>
        ///     Remove a command module.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the module.</typeparam>
        /// <returns>
        ///     A task that represents the asynchronous removal operation. The task result contains a value that
        ///     indicates whether the module is successfully removed.
        /// </returns>
        public Task<bool> RemoveModuleAsync<T> ( ) =>
            RemoveModuleAsync(typeof(T));

        /// <summary>
        ///     Remove a command module.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the module.</param>
        /// <returns>
        ///     A task that represents the asynchronous removal operation. The task result contains a value that
        ///     indicates whether the module is successfully removed.
        /// </returns>
        public async Task<bool> RemoveModuleAsync (Type type)
        {
            await _lock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (!_typedModuleDefs.TryRemove(type, out var module))
                    return false;

                return RemoveModuleInternal(module);
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        ///     Remove a command module.
        /// </summary>
        /// <param name="module">The <see cref="ModuleInfo" /> to be removed from the service.</param>
        /// <returns>
        ///     A task that represents the asynchronous removal operation. The task result contains a value that
        ///     indicates whether the <paramref name="module"/> is successfully removed.
        /// </returns>
        public async Task<bool> RemoveModuleAsync(ModuleInfo module)
        {
            await _lock.WaitAsync().ConfigureAwait(false);

            try
            {
                var typeModulePair = _typedModuleDefs.FirstOrDefault(x => x.Value.Equals(module));

                if (!typeModulePair.Equals(default(KeyValuePair<Type, ModuleInfo>)))
                    _typedModuleDefs.TryRemove(typeModulePair.Key, out var _);

                return RemoveModuleInternal(module);
            }
            finally
            {
                _lock.Release();
            }
        }

        private bool RemoveModuleInternal (ModuleInfo moduleInfo)
        {
            if (!_moduleDefs.Remove(moduleInfo))
                return false;

            foreach (var command in moduleInfo.SlashCommands)
            {
                _slashCommandMap.RemoveCommand(command);
            }

            return true;
        }

        /// <summary>
        ///     Execute a Command from a given <see cref="IInteractionContext"/>.
        /// </summary>
        /// <param name="context">Name context of the command.</param>
        /// <param name="services">The service to be used in the command's dependency injection.</param>
        /// <returns>
        ///     A task representing the command execution process. The task result contains the result of the execution.
        /// </returns>
        public async Task<IResult> ExecuteCommandAsync (IInteractionContext context, IServiceProvider services)
        {
            var interaction = context.Interaction;

            return interaction switch
            {
                ISlashCommandInteraction slashCommand => await ExecuteSlashCommandAsync(context, slashCommand, services).ConfigureAwait(false),
                IComponentInteraction messageComponent => await ExecuteComponentCommandAsync(context, messageComponent.Data.CustomId, services).ConfigureAwait(false),
                IUserCommandInteraction userCommand => await ExecuteContextCommandAsync(context, userCommand.Data.Name, ApplicationCommandType.User, services).ConfigureAwait(false),
                IMessageCommandInteraction messageCommand => await ExecuteContextCommandAsync(context, messageCommand.Data.Name, ApplicationCommandType.Message, services).ConfigureAwait(false),
                IAutocompleteInteraction autocomplete => await ExecuteAutocompleteAsync(context, autocomplete, services).ConfigureAwait(false),
                _ => throw new InvalidOperationException($"{interaction.Type} interaction type cannot be executed by the Interaction service"),
            };
        }

        private async Task<IResult> ExecuteSlashCommandAsync (IInteractionContext context, ISlashCommandInteraction interaction, IServiceProvider services)
        {
            var keywords = interaction.Data.GetCommandKeywords();

            var result = _slashCommandMap.GetCommand(keywords);

            if (!result.IsSuccess)
            {
                await _cmdLogger.DebugAsync($"Unknown slash command, skipping execution ({string.Join(" ", keywords).ToUpper()})");

                await _slashCommandExecutedEvent.InvokeAsync(null, context, result).ConfigureAwait(false);
                return result;
            }
            return await result.Command.ExecuteAsync(context, services).ConfigureAwait(false);
        }

        private async Task<IResult> ExecuteContextCommandAsync (IInteractionContext context, string input, ApplicationCommandType commandType, IServiceProvider services)
        {
            if (!_contextCommandMaps.TryGetValue(commandType, out var map))
                return SearchResult<ContextCommandInfo>.FromError(input, InteractionCommandError.UnknownCommand, $"No {commandType} command found.");

            var result = map.GetCommand(input);

            if (!result.IsSuccess)
            {
                await _cmdLogger.DebugAsync($"Unknown context command, skipping execution ({result.Text.ToUpper()})");

                await _contextCommandExecutedEvent.InvokeAsync(null, context, result).ConfigureAwait(false);
                return result;
            }
            return await result.Command.ExecuteAsync(context, services).ConfigureAwait(false);
        }

        private async Task<IResult> ExecuteComponentCommandAsync (IInteractionContext context, string input, IServiceProvider services)
        {
            var result = _componentCommandMap.GetCommand(input);

            if (!result.IsSuccess)
            {
                await _cmdLogger.DebugAsync($"Unknown custom interaction id, skipping execution ({input.ToUpper()})");

                await _componentCommandExecutedEvent.InvokeAsync(null, context, result).ConfigureAwait(false);
                return result;
            }
            return await result.Command.ExecuteAsync(context, services, result.RegexCaptureGroups).ConfigureAwait(false);
        }

        private async Task<IResult> ExecuteAutocompleteAsync (IInteractionContext context, IAutocompleteInteraction interaction, IServiceProvider services )
        {
            var keywords = interaction.Data.GetCommandKeywords();

            if(_enableAutocompleteHandlers)
            {
                var autocompleteHandlerResult = _slashCommandMap.GetCommand(keywords);

                if(autocompleteHandlerResult.IsSuccess)
                {
                    var parameter = autocompleteHandlerResult.Command.Parameters.FirstOrDefault(x => string.Equals(x.Name, interaction.Data.Current.Name, StringComparison.Ordinal));

                    if(parameter?.AutocompleteHandler is not null)
                        return await parameter.AutocompleteHandler.ExecuteAsync(context, interaction, parameter, services).ConfigureAwait(false);
                }
            }

            keywords.Add(interaction.Data.Current.Name);

            var commandResult = _autocompleteCommandMap.GetCommand(keywords);

            if(!commandResult.IsSuccess)
            {
                await _cmdLogger.DebugAsync($"Unknown command name, skipping autocomplete process ({interaction.Data.CommandName.ToUpper()})");

                await _autocompleteCommandExecutedEvent.InvokeAsync(null, context, commandResult).ConfigureAwait(false);
                return commandResult;
            }

            return await commandResult.Command.ExecuteAsync(context, services).ConfigureAwait(false);
        }

        internal TypeConverter GetTypeConverter (Type type, IServiceProvider services = null)
        {
            if (_typeConverters.TryGetValue(type, out var specific))
                return specific;

            else if (_genericTypeConverters.Any(x => x.Key.IsAssignableFrom(type)))
            {
                services ??= EmptyServiceProvider.Instance;

                var converterType = GetMostSpecificTypeConverter(type);
                var converter = ReflectionUtils<TypeConverter>.CreateObject(converterType.MakeGenericType(type).GetTypeInfo(), this, services);
                _typeConverters[type] = converter;
                return converter;
            }

            else if (_typeConverters.Any(x => x.Value.CanConvertTo(type)))
                return _typeConverters.First(x => x.Value.CanConvertTo(type)).Value;

            throw new ArgumentException($"No type {nameof(TypeConverter)} is defined for this {type.FullName}", "type");
        }

        /// <summary>
        ///     Add a concrete type <see cref="TypeConverter"/>.
        /// </summary>
        /// <typeparam name="T">Primary target <see cref="Type"/> of the <see cref="TypeConverter"/>.</typeparam>
        /// <param name="converter">The <see cref="TypeConverter"/> instance.</param>
        public void AddTypeConverter<T> (TypeConverter converter) =>
            AddTypeConverter(typeof(T), converter);

        /// <summary>
        ///     Add a concrete type <see cref="TypeConverter"/>.
        /// </summary>
        /// <param name="type">Primary target <see cref="Type"/> of the <see cref="TypeConverter"/>.</param>
        /// <param name="converter">The <see cref="TypeConverter"/> instance.</param>
        public void AddTypeConverter (Type type, TypeConverter converter)
        {
            if (!converter.CanConvertTo(type))
                throw new ArgumentException($"This {converter.GetType().FullName} cannot read {type.FullName} and cannot be registered as its {nameof(TypeConverter)}");

            _typeConverters[type] = converter;
        }

        /// <summary>
        ///     Add a generic type <see cref="TypeConverter{T}"/>.
        /// </summary>
        /// <typeparam name="T">Generic Type constraint of the <see cref="Type"/> of the <see cref="TypeConverter{T}"/>.</typeparam>
        /// <param name="converterType">Type of the <see cref="TypeConverter{T}"/>.</param>

        public void AddGenericTypeConverter<T> (Type converterType) =>
            AddGenericTypeConverter(typeof(T), converterType);

        /// <summary>
        ///     Add a generic type <see cref="TypeConverter{T}"/>.
        /// </summary>
        /// <param name="targetType">Generic Type constraint of the <see cref="Type"/> of the <see cref="TypeConverter{T}"/>.</param>
        /// <param name="converterType">Type of the <see cref="TypeConverter{T}"/>.</param>
        public void AddGenericTypeConverter (Type targetType, Type converterType)
        {
            if (!converterType.IsGenericTypeDefinition)
                throw new ArgumentException($"{converterType.FullName} is not generic.");

            var genericArguments = converterType.GetGenericArguments();

            if (genericArguments.Count() > 1)
                throw new InvalidOperationException($"Valid generic {converterType.FullName}s cannot have more than 1 generic type parameter");

            var constraints = genericArguments.SelectMany(x => x.GetGenericParameterConstraints());

            if (!constraints.Any(x => x.IsAssignableFrom(targetType)))
                throw new InvalidOperationException($"This generic class does not support type {targetType.FullName}");

            _genericTypeConverters[targetType] = converterType;
        }

        internal IAutocompleteHandler GetAutocompleteHandler(Type autocompleteHandlerType, IServiceProvider services = null)
        {
            services ??= EmptyServiceProvider.Instance;

            if (!_enableAutocompleteHandlers)
                throw new InvalidOperationException($"{nameof(IAutocompleteHandler)}s are not enabled. To use this feature set {nameof(InteractionServiceConfig.EnableAutocompleteHandlers)} to TRUE");

            if (_autocompleteHandlers.TryGetValue(autocompleteHandlerType, out var autocompleteHandler))
                return autocompleteHandler;
            else
            {
                autocompleteHandler = ReflectionUtils<IAutocompleteHandler>.CreateObject(autocompleteHandlerType.GetTypeInfo(), this, services);
                _autocompleteHandlers[autocompleteHandlerType] = autocompleteHandler;
                return autocompleteHandler;
            }
        }

        /// <summary>
        ///     Modify the command permissions of the matching Discord Slash Command.
        /// </summary>
        /// <param name="module">Module representing the top level Slash Command.</param>
        /// <param name="guild">Target guild.</param>
        /// <param name="permissions">New permission values.</param>
        /// <returns>
        ///     The active command permissions after the modification.
        /// </returns>
        public async Task<GuildApplicationCommandPermission> ModifySlashCommandPermissionsAsync (ModuleInfo module, IGuild guild,
            params ApplicationCommandPermission[] permissions)
        {
            if (!module.IsSlashGroup)
                throw new InvalidOperationException($"This module does not have a {nameof(GroupAttribute)} and does not represent an Application Command");

            if (!module.IsTopLevelGroup)
                throw new InvalidOperationException("This module is not a top level application command. You cannot change its permissions");

            if (guild is null)
                throw new ArgumentNullException("guild");

            var commands = await RestClient.GetGuildApplicationCommands(guild.Id).ConfigureAwait(false);
            var appCommand = commands.First(x => x.Name == module.SlashGroupName);

            return await appCommand.ModifyCommandPermissions(permissions).ConfigureAwait(false);
        }

        /// <summary>
        ///     Modify the command permissions of the matching Discord Slash Command.
        /// </summary>
        /// <param name="command">The Slash Command.</param>
        /// <param name="guild">Target guild.</param>
        /// <param name="permissions">New permission values.</param>
        /// <returns>
        ///     The active command permissions after the modification.
        /// </returns>
        public async Task<GuildApplicationCommandPermission> ModifySlashCommandPermissionsAsync (SlashCommandInfo command, IGuild guild,
            params ApplicationCommandPermission[] permissions) =>
            await ModifyApplicationCommandPermissionsAsync(command, guild, permissions).ConfigureAwait(false);

        /// <summary>
        ///     Modify the command permissions of the matching Discord Slash Command.
        /// </summary>
        /// <param name="command">The Context Command.</param>
        /// <param name="guild">Target guild.</param>
        /// <param name="permissions">New permission values.</param>
        /// <returns>
        ///     The active command permissions after the modification.
        /// </returns>
        public async Task<GuildApplicationCommandPermission> ModifyContextCommandPermissionsAsync (ContextCommandInfo command, IGuild guild,
            params ApplicationCommandPermission[] permissions) =>
            await ModifyApplicationCommandPermissionsAsync(command, guild, permissions).ConfigureAwait(false);

        private async Task<GuildApplicationCommandPermission> ModifyApplicationCommandPermissionsAsync<T> (T command, IGuild guild,
            params ApplicationCommandPermission[] permissions) where T : class, IApplicationCommandInfo, ICommandInfo
        {
            if (!command.IsTopLevelCommand)
                throw new InvalidOperationException("This command is not a top level application command. You cannot change its permissions");

            if (guild is null)
                throw new ArgumentNullException("guild");

            var commands = await RestClient.GetGuildApplicationCommands(guild.Id).ConfigureAwait(false);
            var appCommand = commands.First(x => x.Name == ( command as IApplicationCommandInfo ).Name);

            return await appCommand.ModifyCommandPermissions(permissions).ConfigureAwait(false);
        }

        /// <summary>
        ///     Gets a <see cref="SlashCommandInfo"/>.
        /// </summary>
        /// <typeparam name="TModule">Declaring module type of this command, must be a type of <see cref="InteractionModuleBase{T}"/>.</typeparam>
        /// <param name="methodName">Method name of the handler, use of <see langword="nameof"/> is recommended.</param>
        /// <returns>
        ///     <see cref="SlashCommandInfo"/> instance for this command.
        /// </returns>
        /// <exception cref="InvalidOperationException">Module or Slash Command couldn't be found.</exception>
        public SlashCommandInfo GetSlashCommandInfo<TModule> (string methodName) where TModule : class
        {
            var module = GetModuleInfo<TModule>();

            return module.SlashCommands.First(x => x.MethodName == methodName);
        }

        /// <summary>
        ///     Gets a <see cref="ContextCommandInfo"/>.
        /// </summary>
        /// <typeparam name="TModule">Declaring module type of this command, must be a type of <see cref="InteractionModuleBase{T}"/>.</typeparam>
        /// <param name="methodName">Method name of the handler, use of <see langword="nameof"/> is recommended.</param>
        /// <returns>
        ///     <see cref="ContextCommandInfo"/> instance for this command.
        /// </returns>
        /// <exception cref="InvalidOperationException">Module or Context Command couldn't be found.</exception>
        public ContextCommandInfo GetContextCommandInfo<TModule> (string methodName) where TModule : class
        {
            var module = GetModuleInfo<TModule>();

            return module.ContextCommands.First(x => x.MethodName == methodName);
        }

        /// <summary>
        ///     Gets a <see cref="ComponentCommandInfo"/>.
        /// </summary>
        /// <typeparam name="TModule">Declaring module type of this command, must be a type of <see cref="InteractionModuleBase{T}"/>.</typeparam>
        /// <param name="methodName">Method name of the handler, use of <see langword="nameof"/> is recommended.</param>
        /// <returns>
        ///     <see cref="ComponentCommandInfo"/> instance for this command.
        /// </returns>
        /// <exception cref="InvalidOperationException">Module or Component Command couldn't be found.</exception>
        public ComponentCommandInfo GetComponentCommandInfo<TModule> (string methodName) where TModule : class
        {
            var module = GetModuleInfo<TModule>();

            return module.ComponentCommands.First(x => x.MethodName == methodName);
        }

        /// <summary>
        ///     Gets a built <see cref="ModuleInfo"/>.
        /// </summary>
        /// <typeparam name="TModule">Type of the module, must be a type of <see cref="InteractionModuleBase{T}"/>.</typeparam>
        /// <returns>
        ///     <see cref="ModuleInfo"/> instance for this module.
        /// </returns>
        public ModuleInfo GetModuleInfo<TModule> ( ) where TModule : class
        {
            if (!typeof(IInteractionModuleBase).IsAssignableFrom(typeof(TModule)))
                throw new ArgumentException("Type parameter must be a type of Slash Module", "TModule");

            var module = _typedModuleDefs[typeof(TModule)];

            if (module is null)
                throw new InvalidOperationException($"{typeof(TModule).FullName} is not loaded to the Slash Command Service");

            return module;
        }

        /// <inheritdoc/>
        public void Dispose ( )
        {
            _lock.Dispose();
        }

        private Type GetMostSpecificTypeConverter (Type type)
        {
            if (_genericTypeConverters.TryGetValue(type, out var matching))
                return matching;

            var typeInterfaces = type.GetInterfaces();
            var candidates = _genericTypeConverters.Where(x => x.Key.IsAssignableFrom(type))
                .OrderByDescending(x => typeInterfaces.Count(y => y.IsAssignableFrom(x.Key)));

            return candidates.First().Value;
        }

        private void EnsureClientReady()
        {
            if (RestClient?.CurrentUser is null || RestClient?.CurrentUser?.Id == 0)
                throw new InvalidOperationException($"Provided client is not ready to execute this operation, invoke this operation after a `Client Ready` event");
        }
    }
}
