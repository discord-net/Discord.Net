interactionService.SlashCommandExecuted += SlashCommandExecuted;

async Task SlashCommandExecuted(SlashCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
{
    if (!arg3.IsSuccess)
    {
        switch (arg3.Error)
        {
            case InteractionCommandError.UnmetPrecondition:
                await arg2.Interaction.RespondAsync($"Unmet Precondition: {arg3.ErrorReason}");
                break;
            case InteractionCommandError.UnknownCommand:
                await arg2.Interaction.RespondAsync("Unknown command");
                break;
            case InteractionCommandError.BadArgs:
                await arg2.Interaction.RespondAsync("Invalid number or arguments");
                break;
            case InteractionCommandError.Exception:
                await arg2.Interaction.RespondAsync($"Command exception: {arg3.ErrorReason}");
                break;
            case InteractionCommandError.Unsuccessful:
                await arg2.Interaction.RespondAsync("Command could not be executed");
                break;
            default:
                break;
        }
    }
}
