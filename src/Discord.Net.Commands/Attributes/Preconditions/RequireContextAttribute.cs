using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [Flags]
    public enum ContextType
    {
        Guild = 1, // 01
        DM = 2 // 10
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireContextAttribute : PreconditionAttribute
    {
        public ContextType Context { get; set; }

        public RequireContextAttribute(ContextType context)
        {
            Context = context;
        }

        public override Task<PreconditionResult> CheckPermissions(IMessage context, Command executingCommand, object moduleInstance)
        {
            var validContext = false;

            if (Context.HasFlag(ContextType.Guild))
                validContext = validContext || context.Channel is IGuildChannel;

            if (Context.HasFlag(ContextType.DM))
                validContext = validContext || context.Channel is IDMChannel;

            if (validContext)
                return Task.FromResult(PreconditionResult.FromSuccess());
            else
                return Task.FromResult(PreconditionResult.FromError($"Invalid context for command; accepted contexts: {Context}"));
        }
    }
}
