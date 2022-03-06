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
        internal IReadOnlyDictionary<string, SlashCommandParameterInfo> _flattenedParameterDictionary { get; }

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

        /// <summary>
        ///     Gets the flattened collection of command parameters and complex parameter fields.
        /// </summary>
        public IReadOnlyCollection<SlashCommandParameterInfo> FlattenedParameters { get; }

        internal SlashCommandInfo (Builders.SlashCommandBuilder builder, ModuleInfo module, InteractionService commandService) : base(builder, module, commandService)
        {
            Description = builder.Description;
            DefaultPermission = builder.DefaultPermission;
            Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
            FlattenedParameters = FlattenParameters(Parameters).ToImmutableArray();

            for (var i = 0; i < FlattenedParameters.Count - 1; i++)
                if (!FlattenedParameters.ElementAt(i).IsRequired && FlattenedParameters.ElementAt(i + 1).IsRequired)
                    throw new InvalidOperationException("Optional parameters must appear after all required parameters, ComplexParameters with optional parameters must be located at the end.");

            _flattenedParameterDictionary = FlattenedParameters?.ToDictionary(x => x.Name, x => x).ToImmutableDictionary();
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
                var slashCommandParameterInfos = paramList.ToList();
                var args = new object[slashCommandParameterInfos.Count];

                for (var i = 0; i < slashCommandParameterInfos.Count; i++)
                {
                    var parameter = slashCommandParameterInfos[i];
                    var result = await ParseArgument(parameter, context, argList, services).ConfigureAwait(false);

                    if (!result.IsSuccess)
                        return await InvokeEventAndReturn(context, result).ConfigureAwait(false);

                    if (result is not ParseResult parseResult)
                        return ExecuteResult.FromError(InteractionCommandError.BadArgs, "Command parameter parsing failed for an unknown reason.");

                    args[i] = parseResult.Value;
                }
                return await RunAsync(context, args, services).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                return await InvokeEventAndReturn(context, ExecuteResult.FromError(ex)).ConfigureAwait(false);
            }
        }

        private async Task<IResult> ParseArgument(SlashCommandParameterInfo parameterInfo, IInteractionContext context, List<IApplicationCommandInteractionDataOption> argList,
             IServiceProvider services)
        {
            if (parameterInfo.IsComplexParameter)
            {
                var ctorArgs = new object[parameterInfo.ComplexParameterFields.Count];

                for (var i = 0; i < ctorArgs.Length; i++)
                {
                    var result = await ParseArgument(parameterInfo.ComplexParameterFields.ElementAt(i), context, argList, services).ConfigureAwait(false);

                    if (!result.IsSuccess)
                        return result;

                    if (result is not ParseResult parseResult)
                        return ExecuteResult.FromError(InteractionCommandError.BadArgs, "Complex command parsing failed for an unknown reason.");

                    ctorArgs[i] = parseResult.Value;
                }

                return ParseResult.FromSuccess(parameterInfo._complexParameterInitializer(ctorArgs));
            }

            var arg = argList?.Find(x => string.Equals(x.Name, parameterInfo.Name, StringComparison.OrdinalIgnoreCase));

            if (arg == default)
                return parameterInfo.IsRequired ? ExecuteResult.FromError(InteractionCommandError.BadArgs, "Command was invoked with too few parameters") :
                    ParseResult.FromSuccess(parameterInfo.DefaultValue);

            var typeConverter = parameterInfo.TypeConverter;
            var readResult = await typeConverter.ReadAsync(context, arg, services).ConfigureAwait(false);
            if (!readResult.IsSuccess)
                return readResult;

            return ParseResult.FromSuccess(readResult.Value);
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

        private static IEnumerable<SlashCommandParameterInfo> FlattenParameters(IEnumerable<SlashCommandParameterInfo> parameters)
        {
            foreach (var parameter in parameters)
                if (!parameter.IsComplexParameter)
                    yield return parameter;
                else
                    foreach(var complexParameterField in parameter.ComplexParameterFields)
                        yield return complexParameterField;
        }
    }
}
