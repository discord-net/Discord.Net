using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    public static class CommandServiceExtensions
    {
        public static async Task<IReadOnlyCollection<CommandInfo>> GetExecutableCommandsAsync(this ICollection<CommandInfo> commands, ICommandContext context, IServiceProvider provider)
        {
            var executableCommands = new List<CommandInfo>();

            var tasks = commands.Select(async c =>
            {
                var result = await c.CheckPreconditionsAsync(context, provider).ConfigureAwait(false);
                return new { Command = c, PreconditionResult = result };
            });

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            foreach (var result in results)
            {
                if (result.PreconditionResult.IsSuccess)
                    executableCommands.Add(result.Command);
            }

            return executableCommands;
        }
        public static Task<IReadOnlyCollection<CommandInfo>> GetExecutableCommandsAsync(this CommandService commandService, ICommandContext context, IServiceProvider provider)
            => GetExecutableCommandsAsync(commandService.Commands.ToArray(), context, provider);
        public static async Task<IReadOnlyCollection<CommandInfo>> GetExecutableCommandsAsync(this ModuleInfo module, ICommandContext context, IServiceProvider provider)
        {
            var executableCommands = new List<CommandInfo>();

            executableCommands.AddRange(await module.Commands.ToArray().GetExecutableCommandsAsync(context, provider).ConfigureAwait(false));

            var tasks = module.Submodules.Select(async s => await s.GetExecutableCommandsAsync(context, provider).ConfigureAwait(false));
            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            executableCommands.AddRange(results.SelectMany(c => c));

            return executableCommands;
        }
    }
}
