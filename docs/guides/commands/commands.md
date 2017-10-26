# The Command Service

[Discord.Commands](xref:Discord.Commands) provides an Attribute-based
command parser.

## Setup

To use Commands, you must create a [Command Service] and a Command
Handler.

Included below is a very barebone Command Handler. You can extend your
Command Handler as much as you like; however, the below is the bare
minimum.

The `CommandService` will optionally accept a [CommandServiceConfig],
which _does_ set a few default values for you. It is recommended to
look over the properties in [CommandServiceConfig] and their default
values.

[!code-csharp[Command Handler](samples/command_handler.cs)]

[Command Service]: xref:Discord.Commands.CommandService
[CommandServiceConfig]: xref:Discord.Commands.CommandServiceConfig

## With Attributes

In 1.0, Commands can be defined ahead of time with attributes, or at
runtime with builders.

For most bots, ahead-of-time Commands should be all you need, and this
is the recommended method of defining Commands.

### Modules

The first step to creating Commands is to create a _module_.

A Module is an organizational pattern that allows you to write your
Commands in different classes and have them automatically loaded.

Discord.Net's implementation of Modules is influenced heavily from
ASP.NET Core's Controller pattern. This means that the lifetime of a
module instance is only as long as the Command is being invoked.

**Avoid using long-running code** in your modules wherever possible.
You should **not** be implementing very much logic into your modules,
instead, outsource to a service for that.

If you are unfamiliar with Inversion of Control, it is recommended to
read the MSDN article on [IoC] and [Dependency Injection].

To begin, create a new class somewhere in your project and inherit the
class from [ModuleBase]. This class **must** be `public`.

>[!NOTE]
>[ModuleBase] is an _abstract_ class, meaning that you may extend it
>or override it as you see fit. Your module may inherit from any
>extension of ModuleBase.

By now, your module should look like this:

[!code-csharp[Empty Module](samples/empty-module.cs)]

[IoC]: https://msdn.microsoft.com/en-us/library/ff921087.aspx
[Dependency Injection]: https://msdn.microsoft.com/en-us/library/ff921152.aspx
[ModuleBase]: xref:Discord.Commands.ModuleBase`1

### Adding Commands

The next step to creating Commands is actually creating the Commands.

To create a Command, add a method to your module of type `Task`.
Typically, you will want to mark this method as `async`, although it
is not required.

Adding parameters to a Command is done by adding parameters to the
parent Task.

For example, to take an integer as an argument from the user, add `int
arg`; to take a user as an argument from the user, add `IUser user`.
In 1.0, a Command can accept nearly any type of argument; a full list
of types that are parsed by default can be found in the below section
on _Type Readers_.

Parameters, by default, are always required. To make a parameter
optional, give it a default value. To accept a comma-separated list,
set the parameter to `params Type[]`.

Should a parameter include spaces, it **must** be wrapped in quotes.
For example, for a Command with a parameter `string food`, you would
execute it with `!favoritefood "Key Lime Pie"`.

If you would like a parameter to parse until the end of a Command,
flag the parameter with the [RemainderAttribute]. This will allow a
user to invoke a Command without wrapping a parameter in quotes.

Finally, flag your Command with the [CommandAttribute] (you must
specify a name for this Command, except for when it is part of a
Module Group - see below).

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

Every Command can access the execution context through the [Context]
property on [ModuleBase]. `ICommandContext` allows you to access the
message, channel, guild, and user that the Command was invoked from,
as well as the underlying Discord client that the Command was invoked
from.

Different types of Contexts may be specified using the generic variant
of [ModuleBase]. When using a [SocketCommandContext], for example, the
properties on this context will already be Socket entities, so you
will not need to cast them.

To reply to messages, you may also invoke [ReplyAsync], instead of
accessing the channel through the [Context] and sending a message.

> [!WARNING]
>Contexts should **NOT** be mixed! You cannot have one module that
>uses `CommandContext` and another that uses `SocketCommandContext`.

[Context]: xref:Discord.Commands.ModuleBase`1#Discord_Commands_ModuleBase_1_Context
[SocketCommandContext]: xref:Discord.Commands.SocketCommandContext
[ReplyAsync]: xref:Discord.Commands.ModuleBase`1#Discord_Commands_ModuleBase_1_ReplyAsync_System_String_System_Boolean_Discord_Embed_Discord_RequestOptions_

### Example Module

At this point, your module should look comparable to this example:
[!code-csharp[Example Module](samples/module.cs)]

#### Loading Modules Automatically

The Command Service can automatically discover all classes in an
Assembly that inherit [ModuleBase] and load them.

To opt a module out of auto-loading, flag it with
[DontAutoLoadAttribute].

Invoke [CommandService.AddModulesAsync] to discover modules and
install them.

[DontAutoLoadAttribute]: xref:Discord.Commands.DontAutoLoadAttribute
[CommandService.AddModulesAsync]: xref:Discord.Commands.CommandService#Discord_Commands_CommandService_AddModulesAsync_Assembly_

#### Loading Modules Manually

To manually load a module, invoke [CommandService.AddModuleAsync] by
passing in the generic type of your module and optionally, a
dependency map.

[CommandService.AddModuleAsync]: xref:Discord.Commands.CommandService#Discord_Commands_CommandService_AddModuleAsync__1

### Module Constructors

Modules are constructed using Dependency Injection. Any parameters
that are placed in the Module's constructor must be injected into an
@System.IServiceProvider first. Alternatively, you may accept an
`IServiceProvider` as an argument and extract services yourself.

### Module Properties

Modules with `public` settable properties will have the dependencies
injected after the construction of the Module.

### Module Groups

Module Groups allow you to create a module where Commands are
prefixed. To create a group, flag a module with the
@Discord.Commands.GroupAttribute.

Module groups also allow you to create **nameless Commands**, where
the [CommandAttribute] is configured with no name. In this case, the
Command will inherit the name of the group it belongs to.

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

First, you need to create an @System.IServiceProvider; you may create
your own one if you wish.

Next, add the dependencies that your modules will use to the map.

Finally, pass the map into the `LoadAssembly` method. Your modules
will be automatically loaded with this dependency map.

[!code-csharp[IServiceProvider Setup](samples/dependency_map_setup.cs)]

### Usage in Modules

In the constructor of your Module, any parameters will be filled in by
the @System.IServiceProvider that you've passed into `LoadAssembly`.

Any publicly settable properties will also be filled in the same
manner.

>[!NOTE]
> Annotating a property with a [DontInjectAttribute] attribute will prevent the
property from being injected.

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

## Custom Preconditions

To write your own Precondition, create a new class that inherits from
either [PreconditionAttribute] or [ParameterPreconditionAttribute]
depending on your use.

In order for your Precondition to function, you will need to override
the [CheckPermissions] method.

Your IDE should provide an option to fill this in for you.

If the context meets the required parameters, return 
[PreconditionResult.FromSuccess], otherwise return 
[PreconditionResult.FromError] and include an error message if 
necessary.

[!code-csharp[Custom Precondition](samples/require_owner.cs)]

[CheckPermissions]: xref:Discord.Commands.PreconditionAttribute#Discord_Commands_PreconditionAttribute_CheckPermissions_Discord_Commands_ICommandContext_Discord_Commands_CommandInfo_IServiceProvider_
[PreconditionResult.FromSuccess]: xref:Discord.Commands.PreconditionResult#Discord_Commands_PreconditionResult_FromSuccess
[PreconditionResult.FromError]: xref:Discord.Commands.PreconditionResult#Discord_Commands_PreconditionResult_FromError_System_String_

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

Next, satisfy the `TypeReader` class by overriding the [Read] method.

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
[TypeReaderResult.FromSuccess]: xref:Discord.Commands.TypeReaderResult#Discord_Commands_TypeReaderResult_FromSuccess_Discord_Commands_TypeReaderValue_
[TypeReaderResult.FromError]: xref:Discord.Commands.TypeReaderResult#Discord_Commands_TypeReaderResult_FromError_Discord_Commands_CommandError_System_String_
[Read]: xref:Discord.Commands.TypeReader#Discord_Commands_TypeReader_Read_Discord_Commands_ICommandContext_System_String_IServiceProvider_

#### Sample

[!code-csharp[TypeReaders](samples/typereader.cs)]

### Installing TypeReaders

TypeReaders are not automatically discovered by the Command Service
and must be explicitly added.

To install a TypeReader, invoke [CommandService.AddTypeReader].

[CommandService.AddTypeReader]: xref:Discord.Commands.CommandService#Discord_Commands_CommandService_AddTypeReader__1_Discord_Commands_TypeReader_