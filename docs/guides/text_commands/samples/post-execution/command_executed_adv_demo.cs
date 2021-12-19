public async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
{
    switch(result)
    {
        case MyCustomResult customResult:
            // do something extra with it
            break;
        default:
            if (!string.IsNullOrEmpty(result.ErrorReason))
                await context.Channel.SendMessageAsync(result.ErrorReason);
            break;
    }
}