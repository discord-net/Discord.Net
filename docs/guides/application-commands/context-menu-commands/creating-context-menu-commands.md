# Creating context menu commands.

There are two kinds of Context Menu Commands: User Commands and Message Commands.
Each of these have a Global and Guild variant.
Global menu commands are available for every guild that adds your app. An individual app's global commands are also available in DMs if that app has a bot that shares a mutual guild with the user.

Guild commands are specific to the guild you specify when making them. Guild commands are not available in DMs. Command names are unique per application within each scope (global and guild). That means:

- Your app cannot have two global commands with the same name
- Your app cannot have two guild commands within the same name on the same guild
- Your app can have a global and guild command with the same name
- Multiple apps can have commands with the same names

**Note**: Apps can have a maximum of 5 global context menu commands, and an additional 5 guild-specific context menu commands per guild.

If you don't have the code for a bot ready yet please follow [this guide](https://docs.stillu.cc/guides/getting_started/first-bot.html).

## SlashCommandBuilder

The slash command builder will help you create slash commands. The builder has these available fields and methods:

| Name                  | Type                             | Description                                                                                  |
| --------------------- | -------------------------------- | -------------------------------------------------------------------------------------------- |
| Name                  | string                           | The name of this context menu command.                                                       |
| Description           | string                           | A 0 length string. Left in place for possible future use.                                    |
| WithName              | Function                         | Sets the field name.                                                                         |
| Build                 | Function                         | Builds the builder into the appropriate `CommandCreationProperties` class used to make Menu commands |

**Note**: Context Menu command names can be upper and lowercase, and use spaces.

Let's use the user command builder to make a global and guild command.

```cs
// Let's hook the ready event for creating our commands in.
client.Ready += Client_Ready;

...

public async Task Client_Ready()
{
    // Let's build a guild command! We're going to need a guild id so lets just put that in a variable.
    ulong guildId = 848176216011046962;

    // Next, lets create our user and message command builder. This is like the embed builder but for context menu commands.
    var guildUserCommand = new UserCommandBuilder();
	var guildMessageCommand = new MessageCommandBuilder();

    // Note: Names have to be all lowercase and match the regular expression ^[\w -]{3,32}$
    guildUserCommand.WithName("Guild User Command");
	guildMessageCommand.WithName("Guild Message Command");

    // Descriptions are not used with User and Message commands
    //guildCommand.WithDescription("");

    // Let's do our global commands
    var globalCommand = new UserCommandBuilder();
    globalCommand.WithName("Global User Command");
	var globalMessageCommand = new MessageCommandBuilder();
	globalMessageCommand.WithName("Global Message Command");

    try
    {
        // Now that we have our builder, we can call the rest API to make our slash command.
        await client.Rest.CreateGuildUserCommand(guildUserCommand.Build(), guildId);
		await client.Rest.CreateGuildMessageCommand(guildMessageCommand.Build(), guildId);

        // With global commands we dont need the guild id.
        await client.Rest.CreateGlobalUserCommand(globalUserCommand.Build());
		await client.Rest.CreateGlobalMessageCommand(globalMessageCommand.Build());
    }
    catch(ApplicationCommandException exception)
    {
        // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
        var json = JsonConvert.SerializeObject(exception.Error, Formatting.Indented);

        // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
        Console.WriteLine(json);
    }
}

```
**Note**: Application commands only need to be created once. They do _not_ have to be 'created' on every startup or connection. The example simple shows creating them in the ready event as it's simpler than creating normal bot commands to register application commands.
