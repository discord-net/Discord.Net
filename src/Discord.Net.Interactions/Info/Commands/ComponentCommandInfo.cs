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
    public class ComponentCommandInfo : CommandInfo<CommandParameterInfo>
    {
        /// <inheritdoc/>
        public override IReadOnlyCollection<CommandParameterInfo> Parameters { get; }

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
        ///     A task representing the asyncronous command execution process.
        /// </returns>
        public async Task<IResult> ExecuteAsync(IInteractionContext context, IServiceProvider services, params string[] additionalArgs)
        {
            if (context.Interaction is not IComponentInteraction componentInteraction)
                return ExecuteResult.FromError(InteractionCommandError.ParseFailed, $"Provided {nameof(IInteractionContext)} doesn't belong to a Message Component Interaction");

            var args = new List<string>();

            if (additionalArgs is not null)
                args.AddRange(additionalArgs);

            if (componentInteraction.Data?.Values is not null)
                args.AddRange(componentInteraction.Data.Values);

            return await ExecuteAsync(context, Parameters, args, services);
        }

        /// <inheritdoc/>
        public async Task<IResult> ExecuteAsync(IInteractionContext context, IEnumerable<CommandParameterInfo> paramList, IEnumerable<string> values,
            IServiceProvider services)
        {
            if (context.Interaction is not SocketMessageComponent messageComponent)
                return ExecuteResult.FromError(InteractionCommandError.ParseFailed, $"Provided {nameof(IInteractionContext)} doesn't belong to a Component Command Interaction");

            try
            {
                var strCount = Parameters.Count(x => x.ParameterType == typeof(string));

                if (strCount > values?.Count())
                    return ExecuteResult.FromError(InteractionCommandError.BadArgs, "Command was invoked with too few parameters");

                var componentValues = messageComponent.Data?.Values;

                var args = new object[Parameters.Count];

                if (componentValues is not null)
                {
                    if (Parameters.Last().ParameterType == typeof(string[]))
                        args[args.Length - 1] = componentValues.ToArray();
                    else
                        return ExecuteResult.FromError(InteractionCommandError.BadArgs, $"Select Menu Interaction handlers must accept a {typeof(string[]).FullName} as its last parameter");
                }

                for (var i = 0; i < strCount; i++)
                    args[i] = values.ElementAt(i);

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

                if (argList?.ElementAt(i) == null)
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
