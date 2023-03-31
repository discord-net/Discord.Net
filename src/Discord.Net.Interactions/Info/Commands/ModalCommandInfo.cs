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
        public override IReadOnlyList<ModalCommandParameterInfo> Parameters { get; }

        internal ModalCommandInfo(Builders.ModalCommandBuilder builder, ModuleInfo module, InteractionService commandService) : base(builder, module, commandService)
        {
            Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
            Modal = Parameters.Last().Modal;
        }

        /// <inheritdoc/>
        public override async Task<IResult> ExecuteAsync(IInteractionContext context, IServiceProvider services)
        {
            if (context.Interaction is not IModalInteraction modalInteraction)
                return ExecuteResult.FromError(InteractionCommandError.ParseFailed, $"Provided {nameof(IInteractionContext)} doesn't belong to a Modal Interaction.");

            return await base.ExecuteAsync(context, services).ConfigureAwait(false);
        }

        protected override async Task<IResult> ParseArgumentsAsync(IInteractionContext context, IServiceProvider services)
        {
            var captures = (context as IRouteMatchContainer)?.SegmentMatches?.ToList();
            var captureCount = captures?.Count() ?? 0;

            try
            {
                var args = new object[Parameters.Count];

                for (var i = 0; i < Parameters.Count; i++)
                {
                    var parameter = Parameters.ElementAt(i);

                    if (i < captureCount)
                    {
                        var readResult = await parameter.TypeReader.ReadAsync(context, captures[i].Value, services).ConfigureAwait(false);
                        if (!readResult.IsSuccess)
                            return await InvokeEventAndReturn(context, readResult).ConfigureAwait(false);

                        args[i] = readResult.Value;
                    }
                    else
                    {
                        var modalResult = await Modal.CreateModalAsync(context, services, Module.CommandService._exitOnMissingModalField).ConfigureAwait(false);
                        if (!modalResult.IsSuccess)
                            return await InvokeEventAndReturn(context, modalResult).ConfigureAwait(false);

                        if (modalResult is not TypeConverterResult converterResult)
                            return await InvokeEventAndReturn(context, ExecuteResult.FromError(InteractionCommandError.BadArgs, "Command parameter parsing failed for an unknown reason."));

                        args[i] = converterResult.Value;
                    }
                }

                return ParseResult.FromSuccess(args);
            }
            catch (Exception ex)
            {
                return await InvokeEventAndReturn(context, ExecuteResult.FromError(ex)).ConfigureAwait(false);
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
