using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractionFramework.Attributes
{
    public class RequireOwnerAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckRequirementsAsync (IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            switch (context.Client.TokenType)
            {
                case TokenType.Bot:
                    var application = await context.Client.GetApplicationInfoAsync().ConfigureAwait(false);
                    if (context.User.Id != application.Owner.Id)
                        return PreconditionResult.FromError(ErrorMessage ?? "Command can only be run by the owner of the bot.");
                    return PreconditionResult.FromSuccess();
                default:
                    return PreconditionResult.FromError($"{nameof(RequireOwnerAttribute)} is not supported by this {nameof(TokenType)}.");
            }
        }
    }
}
