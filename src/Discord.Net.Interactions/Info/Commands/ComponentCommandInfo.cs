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
        public override IReadOnlyCollection<ComponentCommandParameterInfo> Parameters { get; }

        /// <inheritdoc/>
        public override bool SupportsWildCards => true;

        internal ComponentCommandInfo(ComponentCommandBuilder builder, ModuleInfo module, InteractionService commandService) : base(builder, module, commandService)
        {
            Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
        }

        /// <inheritdoc/>
        public override async Task<IResult> ExecuteAsync(IInteractionContext context, IServiceProvider services)
            => await ExecuteAsync(context, services, null).ConfigureAwait(false);

        /// <summary>
        ///     Execute this command using dependency injection.
        /// </summary>
        /// <param name="context">Context that will be injected to the <see cref="InteractionModuleBase{T}"/>.</param>
        /// <param name="services">Services that will be used while initializing the <see cref="InteractionModuleBase{T}"/>.</param>
        /// <param name="additionalArgs">Provide additional string parameters to the method along with the auto generated parameters.</param>
        /// <returns>
        ///     A task representing the asynchronous command execution process.
        /// </returns>
        public async Task<IResult> ExecuteAsync(IInteractionContext context, IServiceProvider services, params string[] additionalArgs)
        {
            if (context.Interaction is not IComponentInteraction componentInteraction)
                return ExecuteResult.FromError(InteractionCommandError.ParseFailed, $"Provided {nameof(IInteractionContext)} doesn't belong to a Message Component Interaction");

            return await ExecuteAsync(context, Parameters, additionalArgs, componentInteraction.Data, services);
        }

        /// <inheritdoc/>
        public async Task<IResult> ExecuteAsync(IInteractionContext context, IEnumerable<CommandParameterInfo> paramList, IEnumerable<string> wildcardCaptures, IComponentInteractionData data,
            IServiceProvider services)
        {
            if (context.Interaction is not IComponentInteraction messageComponent)
                return ExecuteResult.FromError(InteractionCommandError.ParseFailed, $"Provided {nameof(IInteractionContext)} doesn't belong to a Component Command Interaction");

            try
            {
                var args = new object[paramCount];

                for (var i = 0; i < paramCount; i++)
                {
                    var parameter = Parameters.ElementAt(i);
                    bool isCapture = i < captureCount;

                    if (isCapture ^ parameter.IsRouteSegmentParameter)
                    {
                        var result = ExecuteResult.FromError(InteractionCommandError.BadArgs, $"Argument type and parameter type didn't match (Wild Card capture/Component value)");
                        await InvokeModuleEvent(context, result).ConfigureAwait(false);
                        return result;
                    }

                    TypeConverterResult readResult;

                    if (isCapture)
                        readResult = await parameter.TypeReader.ReadAsync(context, wildcardCaptures.ElementAt(i), services).ConfigureAwait(false);
                    else
                        readResult = await parameter.TypeConverter.ReadAsync(context, data, services).ConfigureAwait(false);

                    if (!readResult.IsSuccess)
                    {
                        await InvokeModuleEvent(context, readResult).ConfigureAwait(false);
                        return readResult;
                    }

                    args[i] = readResult.Value;
                }

                return await RunAsync(context, args, services).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var result = ExecuteResult.FromError(ex);
                await InvokeModuleEvent(context, result).ConfigureAwait(false);
                return result;
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
