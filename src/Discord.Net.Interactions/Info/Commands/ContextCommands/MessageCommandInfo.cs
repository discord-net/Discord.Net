using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the info class of an attribute based method for command type <see cref="ApplicationCommandType.Message"/>.
    /// </summary>
    public class MessageCommandInfo : ContextCommandInfo
    {
        internal MessageCommandInfo(Builders.ContextCommandBuilder builder, ModuleInfo module, InteractionService commandService)
            : base(builder, module, commandService) { }

        /// <inheritdoc/>
        public override async Task<IResult> ExecuteAsync(IInteractionContext context, IServiceProvider services)
        {
            if (context.Interaction is not IMessageCommandInteraction)
                return ExecuteResult.FromError(InteractionCommandError.ParseFailed, $"Provided {nameof(IInteractionContext)} doesn't belong to a Message Command Interation");

            return await ExecuteAsync(context, services).ConfigureAwait(false);
        }

        protected override async ValueTask<IResult> ParseArgumentsAsync(IInteractionContext context, IServiceProvider services)
        {
            try
            {
                object[] args = new object[1] { (context.Interaction as IMessageCommandInteraction).Data.Message };

                return ParseResult.FromSuccess(args);
            }
            catch (Exception ex)
            {
                return ParseResult.FromError(ex);
            }
        }

        /// <inheritdoc/>
        protected override string GetLogString(IInteractionContext context)
        {
            if (context.Guild != null)
                return $"Message Command: \"{base.ToString()}\" for {context.User} in {context.Guild}/{context.Channel}";
            else
                return $"Message Command: \"{base.ToString()}\" for {context.User} in {context.Channel}";
        }
    }
}
