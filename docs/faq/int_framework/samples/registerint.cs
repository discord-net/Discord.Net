private async Task ReadyAsync()
{
    // pull your commands from some array, everyone has a different approach for this.
    var commands = _builders.ToArray();

    // write your list of commands globally in one go.
    await _client.Rest.BulkOverwriteGlobalCommands(commands);

    // write your array of commands to one guild in one go.
    // You can do a foreach (... in _client.Guilds) approach to write to all guilds.
    await _client.Rest.BulkOverwriteGuildCommands(commands, /* some guild ID */);

    foreach (var c in commands)
    {
        // Create a global command, repeating usage for multiple commands.
        await _client.Rest.CreateGlobalCommand(c);

        // Create a guild command, repeating usage for multiple commands.
        await _client.Rest.CreateGuildCommand(c, guildId);
    }
}
