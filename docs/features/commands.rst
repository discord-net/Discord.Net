Commands
========

The `Discord.Net.Commands`_ package DiscordBotClient extends DiscordClient with support for commands.

.. _Discord.Net.Commands: https://www.nuget.org/packages/Discord.Net.Commands

In order to create a command, you have to add a CommandService to the client.
CommandService takes a CommandServiceConfig object as its parameter. Inside CommandServiceConfig you can setup your CommandChar(s) and HelpMode.

Example (CommandService creation)

.. literalinclude:: /samples/command_service_creation.cs
   :language:csharp6
   :tab-width 2

After command service has been added, you can use the CommandService to create commands.

Example (Command creation)
----------------

.. literalinclude:: /samples/command.cs
   :language: csharp6
   :tab-width: 2
   
If you have, or plan to have multiple commands that could be fit in a certain "group", you should use Command Groups. Command Groups can have same prefix for a group of commands, followed by a prefix for a certain command.
   
Example (CommandGroup creation)
----------------

.. literalinclude:: /samples/command_group.cs
   :language: csharp6
   :tab-width: 2

In this example, to call a greet bye command, we would type "~greet bye" in chat.
