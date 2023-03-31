using Discord.Interactions.Builders;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the info class of an attribute based method for handling Component Interaction events.
    /// </summary>
    public class ComponentCommandInfo : CommandInfo<ComponentCommandParameterInfo>
    {
        /// <inheritdoc/>
        public override IReadOnlyList<ComponentCommandParameterInfo> Parameters { get; }

        /// <inheritdoc/>
        public override bool SupportsWildCards => true;

        internal ComponentCommandInfo(ComponentCommandBuilder builder, ModuleInfo module, InteractionService commandService) : base(builder, module, commandService)
        {
            Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
        }

        /// <inheritdoc/>
        public override async Task<IResult> ExecuteAsync(IInteractionContext context, IServiceProvider services)
        {
            if (context.Interaction is not IComponentInteraction)
                return ExecuteResult.FromError(InteractionCommandError.ParseFailed, $"Provided {nameof(IInteractionContext)} doesn't belong to a Message Component Interaction");

            return await base.ExecuteAsync(context, services).ConfigureAwait(false);
        }

        protected override async Task<IResult> ParseArgumentsAsync(IInteractionContext context, IServiceProvider services)
        {
            var captures = (context as IRouteMatchContainer)?.SegmentMatches?.ToList();
            var captureCount = captures?.Count() ?? 0;

            try
            {
                var data = (context.Interaction as IComponentInteraction).Data;
                var args = new object[Parameters.Count];

                for (var i = 0; i < Parameters.Count; i++)
                {
                    var parameter = Parameters[i];
                    var isCapture = i < captureCount;

                    if (isCapture ^ parameter.IsRouteSegmentParameter)
                        return await InvokeEventAndReturn(context, ExecuteResult.FromError(InteractionCommandError.BadArgs, "Argument type and parameter type didn't match (Wild Card capture/Component value)")).ConfigureAwait(false);

                    var readResult = isCapture ? await parameter.TypeReader.ReadAsync(context, captures[i].Value, services).ConfigureAwait(false) :
                        await parameter.TypeConverter.ReadAsync(context, data, services).ConfigureAwait(false);

                    if (!readResult.IsSuccess)
                        return await InvokeEventAndReturn(context, readResult).ConfigureAwait(false);

                    args[i] = readResult.Value;
                }

                return ParseResult.FromSuccess(args);
            }
            catch (Exception ex)
            {
                return await InvokeEventAndReturn(context, ExecuteResult.FromError(ex)).ConfigureAwait(false);
            }
        }

        protected override Task InvokeModuleEvent(IInteractionContext context, IResult result)
            => CommandService._componentCommandExecutedEvent.InvokeAsync(this, context, result);

        protected override string GetLogString(IInteractionContext context)
        {
            if (context.Guild != null)
                return $"Component Interaction: \"{base.ToString()}\" for {context.User} in {context.Guild}/{context.Channel}";
            else
                return $"Component Interaction: \"{base.ToString()}\" for {context.User} in {context.Channel}";
        }
    }
}
