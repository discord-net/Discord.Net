---
uid: FAQ.Commands.General
title: General Questions about Commands
---

# Command-related Questions

In the following section, you will find commonly asked questions and
answered regarding general command usage when using @Discord.Commands.

## How can I restrict some of my commands so only specific users can execute them?

Based on how you want to implement the restrictions, you can use the
built-in [RequireUserPermission] precondition, which allows you to
restrict the command based on the user's current permissions in the
guild or channel (*e.g., `GuildPermission.Administrator`,
`ChannelPermission.ManageMessages`*).

If, however, you wish to restrict the commands based on the user's
role, you can either create your custom precondition or use
Joe4evr's [Preconditions Addons] that provides a few custom
preconditions that aren't provided in the stock library.
Its source can also be used as an example for creating your
custom preconditions.

[RequireUserPermission]: xref:Discord.Commands.RequireUserPermissionAttribute
[Preconditions Addons]: https://github.com/Joe4evr/Discord.Addons/tree/master/src/Discord.Addons.Preconditions

## Why am I getting an error about `Assembly.GetEntryAssembly`?

You may be confusing @Discord.Commands.CommandService.AddModulesAsync*
with @Discord.Commands.CommandService.AddModuleAsync*. The former
is used to add modules via the assembly, while the latter is used to
add a single module.

## What does [Remainder] do in the command signature?

The [RemainderAttribute] leaves the string unparsed, meaning you
do not have to add quotes around the text for the text to be
recognized as a single object. Please note that if your method has
multiple parameters, the remainder attribute can only be applied to
the last parameter.

[!code-csharp[Remainder](samples/Remainder.cs)]

[RemainderAttribute]: xref:Discord.Commands.RemainderAttribute

## Discord.Net keeps saying that a `MessageReceived` handler is blocking the gateway, what should I do?

By default, the library warns the user about any long-running event
handler that persists for **more than 3 seconds**. Any event
handlers that are run on the same thread as the gateway task, the task
in charge of keeping the connection alive, may block the processing of
heartbeat, and thus terminating the connection.

In this case, the library detects that a `MessageReceived`
event handler is blocking the gateway thread. This warning is
typically associated with the command handler as it listens for that
particular event. If the command handler is blocking the thread, then
this **might** mean that you have a long-running command.

> [!NOTE]
> In rare cases, runtime errors can also cause blockage, usually
> associated with Mono, which is not supported by this library.

To prevent a long-running command from blocking the gateway
thread, a flag called [RunMode] is explicitly designed to resolve
this issue.

There are 2 main `RunMode`s.

1. `RunMode.Sync`
2. `RunMode.Async`

`Sync` is the default behavior and makes the command to be run on the
same thread as the gateway one. `Async` will spin the task off to a
different thread from the gateway one.

> [!IMPORTANT]
> While specifying `RunMode.Async` allows the command to be spun off
> to a different thread, keep in mind that by doing so, there will be
> **potentially unwanted consequences**. Before applying this flag,
> please consider whether it is necessary to do so.
>
> Further details regarding `RunMode.Async` can be found below.

You can set the `RunMode` either by specifying it individually via
the `CommandAttribute` or by setting the global default with
the [DefaultRunMode] flag under `CommandServiceConfig`.

# [CommandAttribute](#tab/cmdattrib)

[!code-csharp[Command Attribute](samples/runmode-cmdattrib.cs)]

# [CommandServiceConfig](#tab/cmdconfig)

[!code-csharp[Command Service Config](samples/runmode-cmdconfig.cs)]

***

***

[RunMode]: xref:Discord.Commands.RunMode
[CommandAttribute]: xref:Discord.Commands.CommandAttribute
[DefaultRunMode]: xref:Discord.Commands.CommandServiceConfig.DefaultRunMode

## How does `RunMode.Async` work, and why is Discord.Net *not* using it by default?

`RunMode.Async` works by spawning a new `Task` with an unawaited
[Task.Run], essentially making the task that is used to invoke the
command task to be finished on a different thread. This design means
that [ExecuteAsync] will be forced to return a successful
[ExecuteResult] regardless of the actual execution result.

The following are the known caveats with `RunMode.Async`,

1. You can potentially introduce a race condition.
2. Unnecessary overhead caused by the [async state machine].
3. [ExecuteAsync] will immediately return [ExecuteResult] instead of
 other result types (this is particularly important for those who wish
 to utilize [RuntimeResult] in 2.0).
4. Exceptions are swallowed in the `ExecuteAsync` result.

However, there are ways to remedy some of these.

For #3, in Discord.Net 2.0, the library introduces a new event called
[CommandService.CommandExecuted], which is raised whenever the command is executed. 
This event will be raised regardless of
the `RunMode` type and will return the appropriate execution result
and the associated @Discord.Commands.CommandInfo if applicable.

For #4, exceptions are caught in [CommandService.Log] event under
[LogMessage.Exception] as [CommandException] and in the 
[CommandService.CommandExecuted] event under the [IResult] as 
[ExecuteResult.Exception].

[Task.Run]: https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.run
[async state machine]: https://www.red-gate.com/simple-talk/dotnet/net-tools/c-async-what-is-it-and-how-does-it-work/
[ExecuteAsync]: xref:Discord.Commands.CommandService.ExecuteAsync*
[ExecuteResult]: xref:Discord.Commands.ExecuteResult
[RuntimeResult]: xref:Discord.Commands.RuntimeResult
[CommandService.CommandExecuted]: xref:Discord.Commands.CommandService.CommandExecuted
[CommandService.Log]: xref:Discord.Commands.CommandService.Log
[LogMessage.Exception]: xref:Discord.LogMessage.Exception*
[ExecuteResult.Exception]: xref:Discord.Commands.ExecuteResult.Exception*
[CommandException]: xref:Discord.Commands.CommandException
[IResult]: xref:Discord.Commands.IResult