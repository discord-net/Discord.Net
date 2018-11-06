using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    /// <summary>
    ///     Requires the command to be invoked in a channel marked NSFW.
    /// </summary>
    /// <remarks>
    ///     The precondition will restrict the access of the command or module to be accessed within a guild channel
    ///     that has been marked as mature or NSFW. If the channel is not of type <see cref="ITextChannel"/> or the
    ///     channel is not marked as NSFW, the precondition will fail with an erroneous <see cref="PreconditionResult"/>.
    /// </remarks>
    /// <example>
    ///     The following example restricts the command <c>too-cool</c> to an NSFW-enabled channel only.
    ///     <code language="cs">
    ///     public class DankModule : ModuleBase
    ///     {
    ///         [Command("cool")]
    ///         public Task CoolAsync()
    ///             => ReplyAsync("I'm cool for everyone.");
    ///
    ///         [RequireNsfw]
    ///         [Command("too-cool")]
    ///         public Task TooCoolAsync()
    ///             => ReplyAsync("You can only see this if you're cool enough.");
    ///     }
    ///     </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequireNsfwAttribute : PreconditionAttribute
    {
        /// <inheritdoc />
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.Channel is ITextChannel text && text.IsNsfw)
                return Task.FromResult(PreconditionResult.FromSuccess());
            else
                return Task.FromResult(PreconditionResult.FromError("This command may only be invoked in an NSFW channel."));
        }
    }
}
