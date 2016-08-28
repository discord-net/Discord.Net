// Defining the Precondition

// Specify that this precondition can target a Class (Module/Group) or Method (Command)
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
// Inherit from PreconditionAttribute
public class RequireOwnerAttribute : PreconditionAttribute
{
    public readonly ulong OwnerId = 66078337084162048;

    // Override the CheckPermissions method
    public override Task<PreconditionResult> CheckPermissions(IUserMessage context, Command executingCommand, object moduleInstance)
    {
        // If the author of the message is '66078337084162048', return success; otherwise fail. 
        return Task.FromResult(context.Author.Id == OwnerId ? PreconditionResult.FromSuccess() : PreconditionResult.FromError("You must be the owner of the bot."));
    }
}