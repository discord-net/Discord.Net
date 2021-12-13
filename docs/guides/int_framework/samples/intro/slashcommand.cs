[SlashCommand("echo", "Echo an input")]
public async Task Echo(string input)
{
    await RespondAsync(input);
}
