---
uid: Guides.Commands.Intro
title: Introduction to the Command Service
---

# The Command Service

[Discord.Commands](xref:Discord.Commands) provides an attribute-based
command parser.

## Setup

To use Commands, you must create a [Command Service] and a command
Handler.

Included below is a very barebone command handler. You can extend your
command Handler as much as you like; however, the below is the bare
minimum.

The `CommandService` will optionally accept a [CommandServiceConfig],
which _does_ set a few default values for you. It is recommended to
look over the properties in [CommandServiceConfig] and their default
values.

[!code-csharp[Command Handler](samples/command_handler.cs)]

[Command Service]: xref:Discord.Commands.CommandService
[CommandServiceConfig]: xref:Discord.Commands.CommandServiceConfig

## With Attributes

Starting from 1.0, Commands can be defined ahead of time with 
attributes, or at runtime with builders.

For most bots, ahead-of-time Commands should be all you need, and this
is the recommended method of defining Commands.

### Modules

The first step to creating Commands is to create a _module_.

A Module is an organizational pattern that allows you to write your
Commands in different classes and have them automatically loaded.

Discord.Net's implementation of Modules is influenced heavily from
ASP.NET Core's Controller pattern. This means that the lifetime of a
module instance is only as long as the command is being invoked.

> [!WARNING]
> **Avoid using long-running code** in your modules wherever possible.
> You should **not** be implementing very much logic into your 
> modules, instead, outsource to a service for that.
>
> If you are unfamiliar with Inversion of Control, it is recommended 
> to read the MSDN article on [IoC] and [Dependency Injection].

>[!NOTE]
>[ModuleBase] is an _abstract_ class, meaning that you may extend it
>or override it as you see fit. Your module may inherit from any
>extension of ModuleBase.

To begin, create a new class somewhere in your project and inherit the
class from [ModuleBase]. This class **must** be `public`.

By now, your module should look like this:

[!code-csharp[Empty Module](samples/empty-module.cs)]

[IoC]: https://msdn.microsoft.com/en-us/library/ff921087.aspx
[Dependency Injection]: https://msdn.microsoft.com/en-us/library/ff921152.aspx
[ModuleBase]: xref:Discord.Commands.ModuleBase`1

### Adding Commands

The next step to creating commands is actually creating the commands.

To create a command, add a method to your module of type `Task` or 
`Task<RuntimeResult>` depending on your use.
Typically, you will want to mark this method as `async`, although it
is not required.

Adding parameters to a command is done by adding parameters to the
parent Task. For example, to take an integer as an argument from 
the user, add `int arg`; to take a user as an argument from the 
user, add `IUser user`. Starting from 1.0, a command can accept 
nearly any type of argument;  a full list of types that are parsed 
by default can be found in the below 
section on [Type Readers](#type-readers).

Parameters, by default, are always required. To make a parameter
optional, give it a default value. To accept a comma-separated list,
set the parameter to `params Type[]`.

Should a parameter include spaces, the parameter **must** be 
wrapped in quotes. For example, for a command with a parameter 
`string food`, you would execute it with 
`!favoritefood "Key Lime Pie"`. If you would like a parameter to 
parse until the end of a command, flag the parameter with the 
[RemainderAttribute]. This will allow a user to invoke a command 
without wrapping a parameter in quotes.

Finally, flag your command with the [CommandAttribute] (you must
specify a name for this command, except for when it is part of a
[Module Group](#module-groups).

[RemainderAttribute]: xref:Discord.Commands.RemainderAttribute
[CommandAttribute]: xref:Discord.Commands.CommandAttribute

### Command Overloads

You may add overloads to your Commands, and the Command parser will
automatically pick up on it.

If for whatever reason, you have two Commands which are ambiguous to
each other, you may use the @Discord.Commands.PriorityAttribute to
specify which should be tested before the other.

The `Priority` attributes are sorted in ascending order; the higher
priority will be called first.

### Command Context

Every command can access the execution context through the [Context]
property on [ModuleBase]. `ICommandContext` allows you to access the
message, channel, guild, user, and the underlying Discord client 
that the command was invoked from.

Different types of Contexts may be specified using the generic variant
of [ModuleBase]. When using a [SocketCommandContext], for example, the
properties on this context will already be Socket entities, so you
will not need to cast them.

To reply to messages, you may also invoke [ReplyAsync], instead of
accessing the channel through the [Context] and sending a message.

> [!WARNING]
>Contexts should **NOT** be mixed! You cannot have one module that
>uses `CommandContext` and another that uses `SocketCommandContext`.

[Context]: xref:Discord.Commands.ModuleBase`1.Context
[SocketCommandContext]: xref:Discord.Commands.SocketCommandContext
[ReplyAsync]: xref:Discord.Commands.ModuleBase`1.ReplyAsync*

### Example Module

At this point, your module should look comparable to this example:
[!code-csharp[Example Module](samples/module.cs)]

#### Loading Modules Automatically

The Command Service can automatically discover all classes in an
Assembly that inherit [ModuleBase] and load them. Invoke 
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

Modules are constructed using Dependency Injection. Any parameters
that are placed in the Module's constructor must be injected into an
@System.IServiceProvider first. 

> [!TIP]
> Alternatively, you may accept an
> `IServiceProvider` as an argument and extract services yourself,
> although this is discouraged.

### Module Properties

Modules with `public` settable properties will have the dependencies
injected after the construction of the Module.

### Module Groups

Module Groups allow you to create a module where Commands are
prefixed. To create a group, flag a module with the
@Discord.Commands.GroupAttribute.

Module groups also allow you to create **nameless Commands**, where
the [CommandAttribute] is configured with no name. In this case, the
command will inherit the name of the group it belongs to.

### Submodules

Submodules are Modules that reside within another one. Typically,
submodules are used to create nested groups (although not required to
create nested groups).

[!code-csharp[Groups and Submodules](samples/groups.cs)]

## With Builders

**TODO**

## Dependency Injection

The Command Service is bundled with a very barebone Dependency
Injection service for your convenience. It is recommended that you use
DI when writing your modules.

### Setup

First, you need to create an @System.IServiceProvider.

Next, add the dependencies to the service collection that you wish 
to use in the Modules.

Finally, pass the service collection into `AddModulesAsync`.

[!code-csharp[IServiceProvider Setup](samples/dependency_map_setup.cs)]

### Usage in Modules

In the constructor of your Module, any parameters will be filled in by
the @System.IServiceProvider that you've passed.

Any publicly settable properties will also be filled in the same
manner.

>[!NOTE]
> Annotating a property with a [DontInjectAttribute] attribute will 
> prevent the property from being injected.

>[!NOTE]
>If you accept `CommandService` or `IServiceProvider` as a parameter
in your constructor or as an injectable property, these entries will
be filled by the `CommandService` that the Module is loaded from and
the `ServiceProvider` that is passed into it respectively.

[!code-csharp[ServiceProvider in Modules](samples/dependency_module.cs)]

[DontInjectAttribute]: xref:Discord.Commands.DontInjectAttribute

# Preconditions

Precondition serve as a permissions system for your Commands. Keep in
mind, however, that they are not limited to _just_ permissions and can
be as complex as you want them to be.

>[!NOTE]
>There are two types of Preconditions.
[PreconditionAttribute] can be applied to Modules, Groups, or Commands;
[ParameterPreconditionAttribute] can be applied to Parameters.

[PreconditionAttribute]: xref:Discord.Commands.PreconditionAttribute
[ParameterPreconditionAttribute]: xref:Discord.Commands.ParameterPreconditionAttribute

## Bundled Preconditions

Commands ship with four bundled Preconditions; you may view their
usages on their respective API pages.

- @Discord.Commands.RequireContextAttribute
- @Discord.Commands.RequireOwnerAttribute
- @Discord.Commands.RequireBotPermissionAttribute
- @Discord.Commands.RequireUserPermissionAttribute
- @Discord.Commands.RequireNsfwAttribute

## Custom Preconditions

To write your own Precondition, create a new class that inherits from
either [PreconditionAttribute] or [ParameterPreconditionAttribute]
depending on your use.

In order for your Precondition to function, you will need to override
the [CheckPermissionsAsync] method.

Your IDE should provide an option to fill this in for you.

If the context meets the required parameters, return 
[PreconditionResult.FromSuccess], otherwise return 
[PreconditionResult.FromError] and include an error message if 
necessary.

[!code-csharp[Custom Precondition](samples/require_owner.cs)]

[CheckPermissionsAsync]: xref:Discord.Commands.PreconditionAttribute.CheckPermissionsAsync*
[PreconditionResult.FromSuccess]: xref:Discord.Commands.PreconditionResult.FromSuccess*
[PreconditionResult.FromError]: xref:Discord.Commands.PreconditionResult.FromError*

# Type Readers

Type Readers allow you to parse different types of arguments in
your commands.

By default, the following Types are supported arguments:

- bool
- char
- sbyte/byte
- ushort/short
- uint/int
- ulong/long
- float, double, decimal
- string
- DateTime/DateTimeOffset/TimeSpan
- IMessage/IUserMessage
- IChannel/IGuildChannel/ITextChannel/IVoiceChannel/IGroupChannel
- IUser/IGuildUser/IGroupUser
- IRole

### Creating a Type Readers

To create a `TypeReader`, create a new class that imports @Discord and
@Discord.Commands and ensure the class inherits from
@Discord.Commands.TypeReader.

Next, satisfy the `TypeReader` class by overriding the [ReadAsync] method.

>[!NOTE]
>In many cases, Visual Studio can fill this in for you, using the
>"Implement Abstract Class" IntelliSense hint.

Inside this task, add whatever logic you need to parse the input
string.

If you are able to successfully parse the input, return 
[TypeReaderResult.FromSuccess] with the parsed input, otherwise return
[TypeReaderResult.FromError] and include an error message if 
necessary.

[TypeReaderResult]: xref:Discord.Commands.TypeReaderResult
[TypeReaderResult.FromSuccess]: xref:Discord.Commands.TypeReaderResult.FromSuccess*
[TypeReaderResult.FromError]: xref:Discord.Commands.TypeReaderResult.FromError*
[ReadAsync]: xref:Discord.Commands.TypeReader.ReadAsync*

#### Sample

[!code-csharp[TypeReaders](samples/typereader.cs)]

### Installing TypeReaders

TypeReaders are not automatically discovered by the Command Service
and must be explicitly added.

To install a TypeReader, invoke [CommandService.AddTypeReader].

[CommandService.AddTypeReader]: xref:Discord.Commands.CommandService.AddTypeReader*
