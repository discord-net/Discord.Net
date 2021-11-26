---
uid: Guides.Commands.Intro
title: Introduction to Command Service
---

# The Command Service

[Discord.Commands](xref:Discord.Commands) provides an attribute-based
command parser.

## Get Started

To use commands, you must create a [Command Service] and a command
handler.

Included below is a barebone command handler. You can extend your
command handler as much as you like; however, the below is the bare
minimum.

> [!NOTE]
> The `CommandService` will optionally accept a [CommandServiceConfig],
> which *does* set a few default values for you. It is recommended to
> look over the properties in [CommandServiceConfig] and their default
> values.

[!code-csharp[Command Handler](samples/intro/command_handler.cs)]

[Command Service]: xref:Discord.Commands.CommandService
[CommandServiceConfig]: xref:Discord.Commands.CommandServiceConfig

## With Attributes

Starting from 1.0, commands can be defined ahead of time with
attributes, or at runtime with builders.

For most bots, ahead-of-time commands should be all you need, and this
is the recommended method of defining commands.

### Modules

The first step to creating commands is to create a _module_.

A module is an organizational pattern that allows you to write your
commands in different classes and have them automatically loaded.

Discord.Net's implementation of "modules" is influenced heavily by the
ASP.NET Core's Controller pattern. This means that the lifetime of a
module instance is only as long as the command is being invoked.

Before we create a module, it is **crucial** for you to remember that
in order to create a module and have it automatically discovered,
your module must:

* Be public
* Inherit [ModuleBase]

By now, your module should look like this:

[!code-csharp[Empty Module](samples/intro/empty-module.cs)]

> [!NOTE]
> [ModuleBase] is an `abstract` class, meaning that you may extend it
> or override it as you see fit. Your module may inherit from any
> extension of ModuleBase.

[IoC]: https://msdn.microsoft.com/en-us/library/ff921087.aspx
[Dependency Injection]: https://msdn.microsoft.com/en-us/library/ff921152.aspx
[ModuleBase]: xref:Discord.Commands.ModuleBase`1

### Adding/Creating Commands

> [!WARNING]
> **Avoid using long-running code** in your modules wherever possible.
> Long-running code, by default, within a command module
> can cause gateway thread to be blocked; therefore, interrupting
> the bot's connection to Discord.
>
> You may read more about it in @FAQ.Commands.General .

The next step to creating commands is actually creating the commands.

For a command to be valid, it **must** have a return type of `Task`
or `Task<RuntimeResult>`. Typically, you might want to mark this
method as `async`, although it is not required.

Then, flag your command with the [CommandAttribute]. Note that you must
specify a name for this command, except for when it is part of a
[Module Group](#module-groups).

### Command Parameters

Adding parameters to a command is done by adding parameters to the
parent `Task`.

For example:

* To take an integer as an argument from the user, add `int num`.
* To take a user as an argument from the user, add `IUser user`.
* ...etc.

Starting from 1.0, a command can accept nearly any type of argument;
a full list of types that are parsed by default can
be found in @Guides.Commands.TypeReaders.

[CommandAttribute]: xref:Discord.Commands.CommandAttribute

#### Optional Parameters

Parameters, by default, are always required. To make a parameter
optional, give it a default value (i.e., `int num = 0`).

#### Parameters with Spaces

To accept a space-separated list, set the parameter to `params Type[]`.

Should a parameter include spaces, the parameter **must** be
wrapped in quotes. For example, for a command with a parameter
`string food`, you would execute it with
`!favoritefood "Key Lime Pie"`.

If you would like a parameter to parse until the end of a command,
flag the parameter with the [RemainderAttribute]. This will
allow a user to invoke a command without wrapping a
parameter in quotes.

[RemainderAttribute]: xref:Discord.Commands.RemainderAttribute

### Command Overloads

You may add overloads to your commands, and the command parser will
automatically pick up on it.

If, for whatever reason, you have two commands which are ambiguous to
each other, you may use the @Discord.Commands.PriorityAttribute to
specify which should be tested before the other.

The `Priority` attributes are sorted in descending order; the higher
priority will be called first.

### Command Context

Every command can access the execution context through the [Context]
property on [ModuleBase]. `ICommandContext` allows you to access the
message, channel, guild, user, and the underlying Discord client
that the command was invoked from.

Different types of `Context` may be specified using the generic variant
of [ModuleBase]. When using a [SocketCommandContext], for example, the
properties on this context will already be Socket entities, so you
will not need to cast them.

To reply to messages, you may also invoke [ReplyAsync], instead of
accessing the channel through the [Context] and sending a message.

> [!WARNING]
> Contexts should **NOT** be mixed! You cannot have one module that
> uses `CommandContext` and another that uses `SocketCommandContext`.

[Context]: xref:Discord.Commands.ModuleBase`1.Context
[SocketCommandContext]: xref:Discord.Commands.SocketCommandContext
[ReplyAsync]: xref:Discord.Commands.ModuleBase`1.ReplyAsync*

> [!TIP]
> At this point, your module should look comparable to this example:
> [!code-csharp[Example Module](samples/intro/module.cs)]

#### Loading Modules Automatically

The Command Service can automatically discover all classes in an
`Assembly` that inherit [ModuleBase] and load them. Invoke
[CommandService.AddModulesAsync] to discover modules and
install them.

To opt a module out of auto-loading, flag it with
[DontAutoLoadAttribute].

[DontAutoLoadAttribute]: xref:Discord.Commands.DontAutoLoadAttribute
[CommandService.AddModulesAsync]: xref:Discord.Commands.CommandService.AddModulesAsync*

#### Loading Modules Manually

To manually load a module, invoke [CommandService.AddModuleAsync] by
passing in the generic type of your module and optionally, a
service provider.

[CommandService.AddModuleAsync]: xref:Discord.Commands.CommandService.AddModuleAsync*

### Module Constructors

Modules are constructed using @Guides.Commands.DI. Any parameters
that are placed in the Module's constructor must be injected into an
@System.IServiceProvider first.

> [!TIP]
> Alternatively, you may accept an
> `IServiceProvider` as an argument and extract services yourself,
> although this is discouraged.

### Module Properties

Modules with `public` settable properties will have the dependencies
injected after the construction of the module. See @Guides.Commands.DI
to learn more.

### Module Groups

Module Groups allow you to create a module where commands are
prefixed. To create a group, flag a module with the
@Discord.Commands.GroupAttribute.

Module Groups also allow you to create **nameless Commands**, where
the [CommandAttribute] is configured with no name. In this case, the
command will inherit the name of the group it belongs to.

### Submodules

Submodules are "modules" that reside within another one. Typically,
submodules are used to create nested groups (although not required to
create nested groups).

[!code-csharp[Groups and Submodules](samples/intro/groups.cs)]
