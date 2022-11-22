using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the info class of an attribute based method for command type <see cref="ApplicationCommandType.User"/>.
    /// </summary>
    public class UserCommandInfo : ContextCommandInfo
    {
        internal UserCommandInfo(Builders.ContextCommandBuilder builder, ModuleInfo module, InteractionService commandService)
            : base(builder, module, commandService) { }

        /// <inheritdoc/>
        public override async Task<IResult> ExecuteAsync(IInteractionContext context, IServiceProvider services)
        {
            if (context.Interaction is not IUserCommandInteraction userCommand)
                return ExecuteResult.FromError(InteractionCommandError.ParseFailed, $"Provided {nameof(IInteractionContext)} doesn't belong to a Message Command Interation");

            return await base.ExecuteAsync(context, services).ConfigureAwait(false);
        }

        protected override Task<IResult> ParseArgumentsAsync(IInteractionContext context, IServiceProvider services)
        {
            try
            {
                object[] args = new object[1] { (context.Interaction as IUserCommandInteraction).Data.User };

                return Task.FromResult(ParseResult.FromSuccess(args) as IResult);
            }
            catch (Exception ex)
            {
                return Task.FromResult(ParseResult.FromError(ex) as IResult);
            }
        }

        /// <inheritdoc/>
        protected override string GetLogString(IInteractionContext context)
        {
            if (context.Guild != null)
                return $"User Command: \"{base.ToString()}\" for {context.User} in {context.Guild}/{context.Channel}";
            else
                return $"User Command: \"{base.ToString()}\" for {context.User} in {context.Channel}";
        }
    }
}
