---
uid: Guides.SlashCommands.Creating
title: Creating Slash Commands
---

# Creating your first slash commands.

There are two kinds of Slash Commands: global commands and guild commands.
Global commands are available for every guild that adds your app. An individual app's global commands are also available in DMs if that app has a bot that shares a mutual guild with the user.

Guild commands are specific to the guild you specify when making them. Guild commands are not available in DMs. Command names are unique per application within each scope (global and guild). That means:

- Your app cannot have two global commands with the same name
- Your app cannot have two guild commands within the same name on the same guild
- Your app can have a global and guild command with the same name
- Multiple apps can have commands with the same names

**Note**: Apps can have a maximum of 100 global commands, and an additional 100 guild-specific commands per guild.

**Note**: Global commands will take up to 1 hour to create, delete or modify on guilds. If you need to update a command quickly for testing you can create it as a guild command.

If you don't have the code for a bot ready yet please follow [this guide](https://docs.stillu.cc/guides/getting_started/first-bot.html).

## SlashCommandBuilder

The slash command builder will help you create slash commands. The builder has these available fields and methods:

| Name                  | Type                             | Description                                                                                  |
| --------------------- | -------------------------------- | -------------------------------------------------------------------------------------------- |
| MaxNameLength         | const int                        | The maximum length of a name for a slash command allowed by Discord.                         |
| MaxDescriptionLength  | const int                        | The maximum length of a commands description allowed by Discord.                             |
| MaxOptionsCount       | const int                        | The maximum count of command options allowed by Discord                                      |
| Name                  | string                           | The name of this slash command.                                                              |
| Description           | string                           | A 1-100 length description of this slash command                                             |
| Options               | List\<SlashCommandOptionBuilder> | The options for this command.                                                                |
| DefaultPermission     | bool                             | Whether the command is enabled by default when the app is added to a guild.                  |
| WithName              | Function                         | Sets the field name.                                                                         |
| WithDescription       | Function                         | Sets the description of the current command.                                                 |
| WithDefaultPermission | Function                         | Sets the default permission of the current command.                                          |
| AddOption             | Function                         | Adds an option to the current slash command.                                                 |
| Build                 | Function                         | Builds the builder into a `SlashCommandCreationProperties` class used to make slash commands |

> [!NOTE]
> Slash command names must be all lowercase!

## Creating a Slash Command

Let's use the slash command builder to make a global and guild command.

```cs
// Let's hook the ready event for creating our commands in.
client.Ready += Client_Ready;

...

public async Task Client_Ready()
{
    // Let's build a guild command! We're going to need a guild so lets just put that in a variable.
    var guild = client.GetGuild(guildId);

    // Next, lets create our slash command builder. This is like the embed builder but for slash commands.
    var guildCommand = new SlashCommandBuilder();

    // Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
    guildCommand.WithName("first-command");

    // Descriptions can have a max length of 100.
    guildCommand.WithDescription("This is my first guild slash command!");

    // Let's do our global command
    var globalCommand = new SlashCommandBuilder();
    globalCommand.WithName("first-global-command");
    globalCommand.WithDescription("This is my first global slash command");

    try
    {
        // Now that we have our builder, we can call the CreateApplicationCommandAsync method to make our slash command.
        await guild.CreateApplicationCommandAsync(guildCommand.Build());

        // With global commands we don't need the guild.
        await client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
        // Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
        // For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
    }
    catch(ApplicationCommandException exception)
    {
        // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
        var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

        // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
        Console.WriteLine(json);
    }
}

```

> [!NOTE]
> Slash commands only need to be created once. They do _not_ have to be 'created' on every startup or connection. The example simple shows creating them in the ready event as it's simpler than creating normal bot commands to register slash commands. The global commands take up to an hour to register every time the CreateGlobalApplicationCommandAsync() is called for a given command.
