---
uid: FAQ.Commands
title: Questions about Commands
---

# Command-related Questions

## How can I restrict some of my commands so only certain users can execute them?

Based on how you want to implement the restrictions, you can use the
built-in [RequireUserPermission] precondition, which allows you to
restrict the command based on the user's current permissions in the
guild or channel (*e.g. `GuildPermission.Administrator`,
`ChannelPermission.ManageMessages` etc.*).

If, however, you wish to restrict the commands based on the user's
role, you can either create your own custom precondition or use
Joe4evr's [Preconditions Addons] that provides a few custom
preconditions that aren't provided in the stock library.
Its source can also be used as an example for creating your own
custom preconditions.

[RequireUserPermission]: xref:Discord.Commands.RequireUserPermissionAttribute
[Preconditions Addons]: https://github.com/Joe4evr/Discord.Addons/tree/master/src/Discord.Addons.Preconditions

## I'm getting an error about `Assembly#GetEntryAssembly`.

You may be confusing [CommandService#AddModulesAsync] with
[CommandService#AddModuleAsync]. The former is used to add modules
via the assembly, while the latter is used to add a single module.

[CommandService#AddModulesAsync]: xref:Discord.Commands.CommandService.AddModulesAsync*
[CommandService#AddModuleAsync]: xref:Discord.Commands.CommandService.AddModuleAsync*

## What does [Remainder] do in the command signature?

The [RemainderAttribute] leaves the string unparsed, meaning you
don't have to add quotes around the text for the text to be
recognized as a single object. Please note that if your method has
multiple parameters, the remainder attribute can only be applied to
the last parameter.

[!code-csharp[Remainder](samples/Remainder.cs)]

[RemainderAttribute]: xref:Discord.Commands.RemainderAttribute

## What is a service? Why does my module not hold any data after execution?

In Discord.Net, modules are created similarly to ASP.NET, meaning
that they have a transient nature. This means that they are spawned
every time when a request is received, and are killed from memory
when the execution finishes. This is why you cannot store persistent
data inside a module. To workaround this, consider using a service.

Service is often used to hold data externally, so that they will
persist throughout execution. Think of it like a chest that holds
whatever you throw at it that won't be affected by anything unless
you want it to. Note that you should also learn Microsoft's
implementation of [Dependency Injection] \([video]) before proceeding, as well
as how it works in [Discord.Net](xref:Guides.Commands.Intro#usage-in-modules).

A brief example of service and dependency injection can be seen below.

[!code-csharp[DI](samples/DI.cs)]

[Dependency Injection]: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
[video]: https://www.youtube.com/watch?v=QtDTfn8YxXg

## I have a long-running Task in my command, and Discord.Net keeps saying that a `MessageReceived` handler is blocking the gateway. What gives?

By default, all commands are executed on the same thread as the
gateway task, which is responsible for keeping the connection from
your client to Discord alive. By default, when you execute a command,
this blocks the gateway from communicating for as long as the command
task is being executed. The library will warn you about any long
running event handler (in this case, the command handler) that
persists for **more than 3 seconds**.

To resolve this, the library has designed a flag called [RunMode].

There are 2 main `RunMode`s.

1. `RunMode.Sync` (default)
2. `RunMode.Async`

You can set the `RunMode` either by specifying it individually via
the `CommandAttribute`, or by setting the global default with
the [DefaultRunMode] flag under `CommandServiceConfig`.

# [CommandAttribute](#tab/cmdattrib)

[!code-csharp[Command Attribute](samples/runmode-cmdattrib.cs)]

# [CommandServiceConfig](#tab/cmdconfig)

[!code-csharp[Command Service Config](samples/runmode-cmdconfig.cs)]

***

***

> [!IMPORTANT]
> While specifying `RunMode.Async` allows the command to be spun off
> to a different thread instead of the gateway thread,
> keep in mind that there will be **potential consequences**
> by doing so. Before applying this flag, please
> consider whether it is necessary to do so.
>
> Further details regarding `RunMode.Async` can be found below.

[RunMode]: xref:Discord.Commands.RunMode
[CommandAttribute]: xref:Discord.Commands.CommandAttribute
[DefaultRunMode]: xref:Discord.Commands.CommandServiceConfig.DefaultRunMode

## How does `RunMode.Async` work, and why is Discord.Net *not* using it by default?

`RunMode.Async` works by spawning a new `Task` with an unawaited
[Task.Run], essentially making `ExecuteAsyncInternalAsync`, the task
that is used to invoke the command task, to be finished on a
different thread. This means that [ExecuteAsync] will be forced to
return a successful [ExecuteResult] regardless of the execution.

The following are the known caveats with `RunMode.Async`,

1. You can potentially introduce race condition.
2. Unnecessary overhead caused by [async state machine].
3. [ExecuteAsync] will immediately return [ExecuteResult] instead of
 other result types (this is particularly important for those who wish
 to utilize [RuntimeResult] in 2.0).
4. Exceptions are swallowed.

However, there are ways to remedy some of these.

For #3, in Discord.Net 2.0, the library introduces a new event called
[CommandExecuted], which is raised whenever the command is
**successfully executed**. This event will be raised regardless of
the `RunMode` type and will return the appropriate execution result.

For #4, exceptions are caught in [CommandService#Log] event under
[LogMessage.Exception] as [CommandException].

[Task.Run]: https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.run
[async state machine]: https://www.red-gate.com/simple-talk/dotnet/net-tools/c-async-what-is-it-and-how-does-it-work/
[ExecuteAsync]: xref:Discord.Commands.CommandService.ExecuteAsync*
[ExecuteResult]: xref:Discord.Commands.ExecuteResult
[RuntimeResult]: xref:Discord.Commands.RuntimeResult
[CommandExecuted]: xref:Discord.Commands.CommandService.CommandExecuted
[CommandService#Log]: xref:Discord.Commands.CommandService.Log
[LogMessage.Exception]: xref:Discord.LogMessage.Exception*
[CommandException]: xref:Discord.Commands.CommandException