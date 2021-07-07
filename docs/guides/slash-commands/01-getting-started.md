# Getting started with slash commands.

Welcome! This guide will show you how to use slash commands. If you have extra questions that aren't covered here you can come to our [Discord](https://discord.com/invite/dvSfUTet3K) server and ask around there.

## What is a slash command?

Slash Commands _(synonymous with application commands)_ are made up of a name, description, and a block of options, which you can think of like arguments to a function. The name and description help users find your command among many others, and the options validate user input as they fill out your command.

Your global commands are available in every guild that adds your application. You can also make commands for a specific guild; they're only available in that guild.

An Interaction is the message that your application receives when a user uses a command. It includes the values that the user submitted, as well as some metadata about this particular instance of the command being used: the guild_id, channel_id, member and other fields. You can find all the values in our data models.

## Authorizing your bot for slash commands

There is a new special OAuth2 scope for applications called `applications.commands`. In order to make Slash Commands work within a guild, the guild must authorize your application with the `applications.commands` scope. The bot scope is not enough.

Head over to your discord applications OAuth2 screen and make sure to select the `application.commands` scope.

![OAuth2 scoping](images/oauth.png)

From there you can then use the link to add your bot to a server.

**Note**: In order for users in your guild to use your slash commands, they need to have the "Use Slash Command" permission on the guild.
