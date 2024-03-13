using System;

namespace Discord.Interactions
{
    public class InteractionException : Exception
    {
        public ICommandInfo CommandInfo { get; }
        public IInteractionContext InteractionContext { get; }

        public InteractionException(ICommandInfo commandInfo, IInteractionContext context, Exception exception)
            : base($"Error occurred executing {commandInfo}.", exception)
        {
            CommandInfo = commandInfo;
            InteractionContext = context;
        }
    }
}
