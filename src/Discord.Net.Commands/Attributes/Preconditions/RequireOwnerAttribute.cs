using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    /// <summary>
    /// Require that the command is invoked by the owner of the bot.
    /// </summary>
    /// <remarks>This precondition will only work if the bot is a bot account.</remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireOwnerAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            var application = await context.Client.GetApplicationInfoAsync();
            if (context.User.Id == application.Owner.Id) return PreconditionResult.FromSuccess();
            return PreconditionResult.FromError("Command can only be run by the owner of the bot");
        }
    }
}
