If you have too many global commands then you might want to consider doing a bulk overwrite.
```cs
public async Task Client_Ready() {
    List<ApplicationCommandProperties> applicationCommandProperties = new();
    try {
        // Simple help slash command.
        SlashCommandBuilder globalCommandHelp = new SlashCommandBuilder();
        globalCommandHelp.WithName("help");
        globalCommandHelp.WithDescription("Shows information about the bot.");
        applicationCommandProperties.Add(globalCommandHelp.Build());
        
        // Slash command with name as its parameter.
        SlashCommandOptionBuilder slashCommandOptionBuilder = new();
        slashCommandOptionBuilder.WithName("name");
        slashCommandOptionBuilder.WithType(ApplicationCommandOptionType.String);
        slashCommandOptionBuilder.WithDescription("Add a family");
        slashCommandOptionBuilder.WithRequired(true); // Only add this if you want it to be required

        SlashCommandBuilder globalCommandAddFamily = new SlashCommandBuilder(); 
        globalCommandAddFamily.WithName("add-family");
        globalCommandAddFamily.WithDescription("Add a family");
        applicationCommandProperties.Add(globalCommandAddFamily.Build());
        
        await _client.BulkOverwriteGlobalApplicationCommandsAsync(applicationCommandProperties.ToArray());
    } catch (ApplicationCommandException exception) {
        var json = JsonConvert.SerializeObject(exception.Error, Formatting.Indented);
        Console.WriteLine(json);
    }
    Console.WriteLine("Client Ready: Finished");
}
```
