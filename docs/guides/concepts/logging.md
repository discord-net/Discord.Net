---
title: Logging
---

Discord.Net's clients provide a [Log] event that all messages will be
disbatched over.

For more information about events in Discord.Net, see the [Events]
section.

[Log]: xref:Discord.Rest.BaseDiscordClient#Discord_Rest_BaseDiscordClient_Log
[Events]: events.md

### Usage

To receive log events, simply hook the discord client's log method
to a `Task` with a single parameter of type [LogMessage].

It is recommended that you use an established function instead of a
lambda for handling logs, because most addons accept a reference
to a logging function to write their own messages.

[LogMessage]: xref:Discord.LogMessage

### Usage in Commands

Discord.Net's [CommandService] also provides a log event, identical
in signature to other log events.

Data logged through this event is typically coupled with a
[CommandException], where information about the command's context
and error can be found and handled.

[CommandService]: xref:Discord.Commands.CommandService
[CommandException]: xref:Discord.Commands.CommandException

#### Samples

[!code-csharp[Logging Sample](samples/logging.cs)]

#### Tips

Due to the nature of Discord.Net's event system, all log event
handlers will be executed synchronously on the gateway thread. If your
log output will be dumped to a Web API (e.g. Sentry), you are advised
to wrap your output in a `Task.Run` so the gateway thread does not
become blocked while waiting for logging data to be written.