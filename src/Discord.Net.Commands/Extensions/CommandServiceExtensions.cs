using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Commands
{
    public static class CommandServiceExtensions
    {
        public static async Task<IReadOnlyCollection<CommandInfo>> GetExecutableCommandsAsync(this IEnumerable<CommandInfo> commands, ICommandContext context, IServiceProvider provider)
        {
            var executableCommands = new List<CommandInfo>();
            
            foreach (var command in commands)
            {
                var result = await command.CheckPreconditionsAsync(context, provider).ConfigureAwait(false);
                if (result.IsSuccess)
                    executableCommands.Add(command);
            }

            return executableCommands;
        }
        public static Task<IReadOnlyCollection<CommandInfo>> GetExecutableCommandsAsync(this CommandService commandService, ICommandContext context, IServiceProvider provider)
            => GetExecutableCommandsAsync(commandService.Commands, context, provider);
        public static async Task<IReadOnlyCollection<CommandInfo>> GetExecutableCommandsAsync(this ModuleInfo module, ICommandContext context, IServiceProvider provider)
        {
            var executableCommands = new List<CommandInfo>();

            executableCommands.AddRange(await module.Commands.GetExecutableCommandsAsync(context, provider).ConfigureAwait(false));

            foreach (var subModule in module.Submodules)
                executableCommands.AddRange(await subModule.GetExecutableCommandsAsync(context, provider).ConfigureAwait(false));
            
            return executableCommands;
        }
    }
}
