using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the info class of an attribute based method for command type <see cref="ApplicationCommandType.Slash"/>.
    /// </summary>
    public class SlashCommandInfo : CommandInfo<SlashCommandParameterInfo>, IApplicationCommandInfo
    {
        /// <summary>
        ///     Gets the command description that will be displayed on Discord.
        /// </summary>
        public string Description { get; }

        /// <inheritdoc/>
        public ApplicationCommandType CommandType { get; } = ApplicationCommandType.Slash;

        /// <inheritdoc/>
        public bool DefaultPermission { get; }

        /// <inheritdoc/>
        public override IReadOnlyCollection<SlashCommandParameterInfo> Parameters { get; }

        /// <inheritdoc/>
        public override bool SupportsWildCards => false;

        internal SlashCommandInfo (Builders.SlashCommandBuilder builder, ModuleInfo module, InteractionService commandService) : base(builder, module, commandService)
        {
            Description = builder.Description;
            DefaultPermission = builder.DefaultPermission;
            Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
        }

        /// <inheritdoc/>
        public override async Task<IResult> ExecuteAsync (IInteractionContext context, IServiceProvider services)
        {
            if(context.Interaction is not ISlashCommandInteraction slashCommand)
                return ExecuteResult.FromError(InteractionCommandError.ParseFailed, $"Provided {nameof(IInteractionContext)} doesn't belong to a Slash Command Interaction");

            var options = slashCommand.Data.Options;

            while (options != null && options.Any(x => x.Type == ApplicationCommandOptionType.SubCommand || x.Type == ApplicationCommandOptionType.SubCommandGroup))
                options = options.ElementAt(0)?.Options;

            return await ExecuteAsync(context, Parameters, options?.ToList(), services);
        }

        private async Task<IResult> ExecuteAsync (IInteractionContext context, IEnumerable<SlashCommandParameterInfo> paramList,
            List<IApplicationCommandInteractionDataOption> argList, IServiceProvider services)
        {
            try
            {
                if (paramList?.Count() < argList?.Count())
                    return ExecuteResult.FromError(InteractionCommandError.BadArgs ,"Command was invoked with too many parameters");

                var args = new object[paramList.Count()];

                for (var i = 0; i < paramList.Count(); i++)
                {
                    var parameter = paramList.ElementAt(i);

                    var arg = argList?.Find(x => string.Equals(x.Name, parameter.Name, StringComparison.OrdinalIgnoreCase));

                    if (arg == default)
                    {
                        if (parameter.IsRequired)
                            return ExecuteResult.FromError(InteractionCommandError.BadArgs, "Command was invoked with too few parameters");
                        else
                            args[i] = parameter.DefaultValue;
                    }
                    else
                    {
                        var typeConverter = parameter.TypeConverter;

                        var readResult = await typeConverter.ReadAsync(context, arg, services).ConfigureAwait(false);

                        if (!readResult.IsSuccess)
                        {
                            await InvokeModuleEvent(context, readResult).ConfigureAwait(false);
                            return readResult;
                        }

                        args[i] = readResult.Value;
                    }
                }

                return await RunAsync(context, args, services).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return ExecuteResult.FromError(ex);
            }
        }

        protected override Task InvokeModuleEvent (IInteractionContext context, IResult result)
            => CommandService._slashCommandExecutedEvent.InvokeAsync(this, context, result);

        protected override string GetLogString (IInteractionContext context)
        {
            if (context.Guild != null)
                return $"Slash Command: \"{base.ToString()}\" for {context.User} in {context.Guild}/{context.Channel}";
            else
                return $"Slash Command: \"{base.ToString()}\" for {context.User} in {context.Channel}";
        }
    }
}
