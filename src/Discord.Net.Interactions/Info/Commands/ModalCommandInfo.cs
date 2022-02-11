using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the info class of an attribute based method for handling Modal Interaction events.
    /// </summary>
    public class ModalCommandInfo : CommandInfo<ModalCommandParameterInfo>
    {
        /// <summary>
        ///     Gets the <see cref="ModalInfo"/> class for this commands <see cref="IModal"/> parameter.
        /// </summary>
        public ModalInfo Modal { get; }

        /// <inheritdoc/>
        public override bool SupportsWildCards => true;

        /// <inheritdoc/>
        public override IReadOnlyCollection<ModalCommandParameterInfo> Parameters { get; }

        internal ModalCommandInfo(Builders.ModalCommandBuilder builder, ModuleInfo module, InteractionService commandService) : base(builder, module, commandService)
        {
            Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
            Modal = Parameters.Last().Modal;
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
            if (context.Interaction is not IModalInteraction modalInteraction)
                return ExecuteResult.FromError(InteractionCommandError.ParseFailed, $"Provided {nameof(IInteractionContext)} doesn't belong to a Modal Interaction.");

            try
            {
                var args = new object[Parameters.Count];
                var captureCount = additionalArgs.Length;

                for(var i = 0; i < Parameters.Count; i++)
                {
                    var parameter = Parameters.ElementAt(i);

                    if(i < captureCount)
                    {
                        var readResult = await parameter.TypeReader.ReadAsync(context, additionalArgs[i], services).ConfigureAwait(false);

                        if(!readResult.IsSuccess)
                        {
                            await InvokeModuleEvent(context, readResult).ConfigureAwait(false);
                            return readResult;
                        }
                        args[i] = readResult.Value;
                    }
                    else
                    {
                        var modalResult = await Modal.CreateModalAsync(context, services, Module.CommandService._exitOnMissingModalField).ConfigureAwait(false);

                        if (!modalResult.IsSuccess)
                        {
                            await InvokeModuleEvent(context, modalResult).ConfigureAwait(false);
                            return modalResult;
                        }

                        if (modalResult is ParseResult parseResult)
                            args[i] = parseResult.Value;
                        else
                            return ExecuteResult.FromError(InteractionCommandError.BadArgs, "Command parameter parsing failed for an unknown reason.");
                    }
                }
                return await RunAsync(context, args.ToArray(), services);
            }
            catch (Exception ex)
            {
                var result = ExecuteResult.FromError(ex);
                await InvokeModuleEvent(context, result).ConfigureAwait(false);
                return result;
            }
        }

        /// <inheritdoc/>
        protected override Task InvokeModuleEvent(IInteractionContext context, IResult result)
            => CommandService._modalCommandExecutedEvent.InvokeAsync(this, context, result);

        /// <inheritdoc/>
        protected override string GetLogString(IInteractionContext context)
        {
            if (context.Guild != null)
                return $"Modal Command: \"{base.ToString()}\" for {context.User} in {context.Guild}/{context.Channel}";
            else
                return $"Modal Command: \"{base.ToString()}\" for {context.User} in {context.Channel}";
        }
    }
}
