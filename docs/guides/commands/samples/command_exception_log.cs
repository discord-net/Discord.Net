public async Task LogAsync(LogMessage logMessage)
{
    // This casting type requries C#7
    if (logMessage.Exception is CommandException cmdException)
    {
        // We can tell the user that something unexpected has happened
        await cmdException.Context.Channel.SendMessageAsync("Something went catastrophically wrong!");

        // We can also log this incident
        Console.WriteLine($"{cmdException.Context.User} failed to execute '{cmdException.Command.Name}' in {cmdException.Context.Channel}.");
        Console.WriteLine(cmdException.ToString());
    }
}