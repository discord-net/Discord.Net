using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    /// <summary>
    /// Require that the command is invoked in a channel marked NSFW
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequireNsfwAttribute : PreconditionAttribute
    {
        public override string ErrorMessage { get; set; }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.Channel is ITextChannel text && text.IsNsfw)
                return Task.FromResult(PreconditionResult.FromSuccess());
            else
                return Task.FromResult(PreconditionResult.FromError(ErrorMessage ?? "This command may only be invoked in an NSFW channel."));
        }
    }
}
