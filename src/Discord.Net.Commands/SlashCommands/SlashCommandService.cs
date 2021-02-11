using Discord.Commands;
using Discord.Logging;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    public class SlashCommandService
    {
        // This semaphore is used to prevent race conditions.
        private readonly SemaphoreSlim _moduleLock;
        // This contains a dictionary of all definde SlashCommands, based on it's name
        public Dictionary<string, SlashCommandInfo> commandDefs;
        // This contains a list of all slash command modules defined by their user in their assembly.
        public Dictionary<Type, SlashModuleInfo> moduleDefs;

        // This is such a complicated method to log stuff...
        public event Func<LogMessage, Task> Log { add { _logEvent.Add(value); } remove { _logEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new AsyncEvent<Func<LogMessage, Task>>();
        internal Logger _logger;
        internal LogManager _logManager;

        public SlashCommandService() // TODO: possible config?
        {
            // max one thread
            _moduleLock = new SemaphoreSlim(1, 1);
            
            _logManager = new LogManager(LogSeverity.Info);
            _logManager.Message += async msg => await _logEvent.InvokeAsync(msg).ConfigureAwait(false);
            _logger = new Logger(_logManager, "SlshCommand");
        }

        public void AddAssembly()
        {

        }

        /// <summary>
        /// Execute a slash command.
        /// </summary>
        /// <param name="interaction">Interaction data recieved from discord.</param>
        /// <returns></returns>
        public async Task<IResult> ExecuteAsync(SocketInteraction interaction)
        {
            // First, get the info about this command, if it exists
            SlashCommandInfo commandInfo;
            if (commandDefs.TryGetValue(interaction.Data.Name, out commandInfo))
            {
                // TODO: implement everything that has to do with parameters :)

                // Then, set the context in which the command will be executed
                commandInfo.Module.userCommandModule.SetContext(interaction);
                // Then run the command (with no parameters)
                return await commandInfo.ExecuteAsync(new object[] { }).ConfigureAwait(false);
            }
            else
            {
                return SearchResult.FromError(CommandError.UnknownCommand, $"There is no registered slash command with the name {interaction.Data.Name}");
            }
        }
        
        public async Task AddModulesAsync(Assembly assembly, IServiceProvider services)
        {
            // First take a hold of the module lock, as to make sure we aren't editing stuff while we do our business
            await _moduleLock.WaitAsync().ConfigureAwait(false);

            try
            {
                // Get all of the modules that were properly defined by the user.
                IReadOnlyList<TypeInfo> types = await SlashCommandServiceHelper.GetValidModuleClasses(assembly, this).ConfigureAwait(false);
                // Then, based on that, make an instance out of each of them, and get the resulting SlashModuleInfo s
                moduleDefs = await SlashCommandServiceHelper.InstantiateModules(types, this).ConfigureAwait(false);
                // After that, internally register all of the commands into SlashCommandInfo
                commandDefs = await SlashCommandServiceHelper.PrepareAsync(types,moduleDefs,this).ConfigureAwait(false);
                // TODO: And finally, register the commands with discord.
                await SlashCommandServiceHelper.RegisterCommands(commandDefs, this, services).ConfigureAwait(false);
            }
            finally
            {
                _moduleLock.Release();
            }
        }
    }
}
