using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireOwnerAttribute : PreconditionAttribute
    {
        private IApplication application;

        public override async Task<PreconditionResult> CheckPermissions(CommandContext context, CommandInfo command, IDependencyMap map)
        {
            if (application == null)
                application = await context.Client.GetApplicationInfoAsync();
            if (context.User.Id == application.Owner.Id) return PreconditionResult.FromSuccess();
            return PreconditionResult.FromError("Command can only be run by the owner of the bot");
        }
    }
}
