using Discord.Interactions.Builders;
using Discord.WebSocket;
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
                var paramCount = paramList.Count();
                var captureCount = wildcardCaptures?.Count() ?? 0;
                var args = new object[paramCount];

                for(var i = 0; i < paramCount; i++)
                {
                    var parameter = Parameters.ElementAt(i);

                    if (i < captureCount)
                    {
                        if(parameter.IsRouteSegmentParameter)
                        {
                            var readResult = await parameter.TypeReader.ReadAsync(context, wildcardCaptures.ElementAt(i), services).ConfigureAwait(false);

                            if(!readResult.IsSuccess)
                            {
                                await InvokeModuleEvent(context, readResult).ConfigureAwait(false);
                                return readResult;
                            }

                            args[i] = readResult.Value;
                        }
                        else
                        {
                            var result = ExecuteResult.FromError(InteractionCommandError.BadArgs, $"Expected Wild Card Capture, got component value");
                            await InvokeModuleEvent(context, result).ConfigureAwait(false);
                            return result;
                        }    
                    }
                    else
                    {
                        if(!parameter.IsRouteSegmentParameter)
                        {
                            var readResult = await parameter.TypeConverter.ReadAsync(context, data, services).ConfigureAwait(false);

                            if (!readResult.IsSuccess)
                            {
                                await InvokeModuleEvent(context, readResult).ConfigureAwait(false);
                                return readResult;
                            }

                            args[i] = readResult.Value;
                        }
                        else
                        {
                            var result = ExecuteResult.FromError(InteractionCommandError.BadArgs, $"Expected component value, got Wild Card capture.");
                            await InvokeModuleEvent(context, result).ConfigureAwait(false);
                            return result;
                        }
                    }
                }

                return await RunAsync(context, args, services).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return ExecuteResult.FromError(ex);
            }
        }

        private static object[] GenerateArgs(IEnumerable<CommandParameterInfo> paramList, IEnumerable<string> argList)
        {
            var result = new object[paramList.Count()];

            for (var i = 0; i < paramList.Count(); i++)
            {
                var parameter = paramList.ElementAt(i);

                if (argList?.ElementAt(i) is null)
                {
                    if (!parameter.IsRequired)
                        result[i] = parameter.DefaultValue;
                    else
                        throw new InvalidOperationException($"Component Interaction handler is executed with too few args.");
                }
                else if (parameter.IsParameterArray)
                {
                    string[] paramArray = new string[argList.Count() - i];
                    argList.ToArray().CopyTo(paramArray, i);
                    result[i] = paramArray;
                }
                else
                    result[i] = argList?.ElementAt(i);
            }

            return result;
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
