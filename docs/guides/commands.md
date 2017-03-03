# The Command Service

[Discord.Commands](xref:Discord.Commands) provides an Attribute-based
 Command Parser.

## Setup

To use Commands, you must create a [Commands Service] and a
Command Handler.

Included below is a very bare-bones Command Handler. You can extend
your Command Handler as much as you like, however the below is the
bare minimum.

The CommandService optionally will accept a [CommandServiceConfig],
which _does_ set a few default values for you. It is recommended to
look over the properties in [CommandServiceConfig], and their default
values.

[!code-csharp[Command Handler](samples/command_handler.cs)]

[Command Service]: xref:Discord.Commands.CommandService
[CommandServiceConfig]: xref:Discord.Commands.CommandServiceConfig

## With Attributes

In 1.0, Commands can be defined ahead of time, with attributes, or
at runtime, with builders.

For most bots, ahead-of-time commands should be all you need, and this
is the recommended method of defining commands.

### Modules

The first step to creating commands is to create a _module_.

Modules are an organizational pattern that allow you to write your
commands in different classes, and have them automatically loaded.

Discord.Net's implementation of Modules is influenced heavily from
ASP.Net Core's Controller pattern. This means that the lifetime of a
module instance is only as long as the command being invoked.

**Avoid using long-running code** in your modules whereever possible.
You should **not** be implementing very much logic into your modules;
outsource to a service for that.

If you are unfamiliar with Inversion of Control, it is recommended to
read the MSDN article on [IoC] and [Dependency Injection].

To begin, create a new class somewhere in your project, and
inherit the class from [ModuleBase]. This class **must** be `public`.

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

The next step to creating commands, is actually creating commands.

To create a command, add a method to your module of type `Task`.
Typically, you will want to mark this method as `async`, although it is
not required.

Adding parameters to a command is done by adding parameters to the
parent Task.

For example, to take an integer as an argument, add `int arg`. To take
a user as an argument, add `IUser user`. In 1.0, a command can accept
nearly any type of argument; a full list of types that are parsed by
default can be found in the below section on _Type Readers_.

Parameters, by default, are always required. To make a parameter
optional, give it a default value. To accept a comma-separated list,
set the parameter to `params Type[]`.

Should a parameter include spaces, it **must** be wrapped in quotes.
For example, for a command with a parameter `string food`, you would
execute it with `!favoritefood "Key Lime Pie"`.

If you would like a parameter to parse until the end of a command,
flag the parameter with the [RemainderAttribute]. This will allow a
user to invoke a command without wrapping a parameter in quotes.

Finally, flag your command with the [CommandAttribute]. (You must
specify a name for this command, except for when it is part of a
module group - see below).

[RemainderAttribute]: xref:Discord.Commands.RemainderAttribute
[CommandAttribute]: xref:Discord.Commands.CommandAttribute

### Command Overloads

You may add overloads of your commands, and the command parser will
automatically pick up on it.

If, for whatever reason, you have too commands which are ambiguous to
each other, you may use the @Discord.Commands.PriorityAttribute to
specify which should be tested before the other.

Priority's are sorted in ascending order; the higher priority will be
called first.

### CommandContext

Every command can access the execution context through the [Context]
property on [ModuleBase]. CommandContext allows you to access the
message, channel, guild, and user that the command was invoked from,
as well as the underlying discord client the command was invoked from.

Different types of Contexts may be specified using the generic variant
of [ModuleBase]. When using a [SocketCommandContext], for example,
the properties on this context will already be Socket entities. You
will not need to cast them.

To reply to messages, you may also invoke [ReplyAsync], instead of
accessing the channel through the [Context] and sending a message.

[Context]: xref:Discord.Commands.ModuleBase`1#Discord_Commands_ModuleBase_1_Context
[SocketCommandContext]: xref:Discord.Commands.SocketCommandContext

>![WARNING]
>Contexts should **NOT** be mixed! You cannot have one module that
>uses CommandContext, and another that uses SocketCommandContext.

### Example Module

At this point, your module should look comparable to this example:
[!code-csharp[Example Module](samples/module.cs)]

#### Loading Modules Automatically

The Command Service can automatically discover all classes in an
Assembly that inherit [ModuleBase], and load them.

To opt a module out of auto-loading, flag it with
[DontAutoLoadAttribute]

Invoke [CommandService.AddModulesAsync] to discover modules and
install them.

[DontAutoLoadAttribute]: xref:Discord.Commands.DontAutoLoadAttribute
[CommandService.AddModulesAsync]: xref:Discord_Commands_CommandService#Discord_Commands_CommandService_AddModulesAsync_Assembly_

#### Loading Modules Manually

To manually load a module, invoke [CommandService.AddModuleAsync],
by passing in the generic type of your module, and optionally
a dependency map.

[CommandService.AddModuleAsync]: xref:Discord.Commands.CommandService#Discord_Commands_CommandService_AddModuleAsync__1

### Module Constructors

Modules are constructed using Dependency Injection. Any parameters
that are placed in the constructor must be injected into an
@Discord.Commands.IDependencyMap. Alternatively, you may accept an
IDependencyMap as an argument and extract services yourself.

### Module Properties

Modules with public settable properties will have them injected after module
construction.

### Module Groups

Module Groups allow you to create a module where commands are prefixed.
To create a group, flag a module with the
@Discord.Commands.GroupAttribute

Module groups also allow you to create **nameless commands**, where the
[CommandAttribute] is configured with no name. In this case, the
command will inherit the name of the group it belongs to.

### Submodules

Submodules are modules that reside within another module. Typically,
submodules are used to create nested groups (although not required to
create nested groups).

[!code-csharp[Groups and Submodules](samples/groups.cs)]

## With Builders

**TODO**

## Dependency Injection

The commands service is bundled with a very barebones Dependency
Injection service for your convienence. It is recommended that
you use DI when writing your modules.

### Setup

First, you need to create an @Discord.Commands.IDependencyMap.
The library includes @Discord.Commands.DependencyMap to help with
this, however you may create your own IDependencyMap if you wish.

Next, add the dependencies your modules will use to the map.

Finally, pass the map into the `LoadAssembly` method.
Your modules will automatically be loaded with this dependency map.

[!code-csharp[DependencyMap Setup](samples/dependency_map_setup.cs)]

### Usage in Modules

In the constructor of your module, any parameters will be filled in by
the @Discord.Commands.IDependencyMap you pass into `LoadAssembly`.

Any publicly settable properties will also be filled in the same manner.

>[!NOTE]
> Annotating a property with the [DontInject] attribute will prevent it from
being injected.

>[!NOTE]
>If you accept `CommandService` or `IDependencyMap`  as a parameter in
your constructor or as an injectable property, these entries will be filled
by the CommandService the module was loaded from, and the DependencyMap passed
into it, respectively.

[!code-csharp[DependencyMap in Modules](samples/dependency_module.cs)]

# Preconditions

Preconditions serve as a permissions system for your commands. Keep in
mind, however, that they are not limited to _just_ permissions, and
can be as complex as you want them to be.

>[!NOTE]
>Preconditions can be applied to Modules, Groups, or Commands.

## Bundled Preconditions

Commands ships with four bundled preconditions; you may view their
usages on their API page.

- @Discord.Commands.RequireContextAttribute
- @Discord.Commands.RequireOwnerAttribute
- @Discord.Commands.RequireBotPermissionAttribute
- @Discord.Commands.RequireUserPermissionAttribute

## Custom Preconditions

To write your own preconditions, create a new class that inherits from
 @Discord.Commands.PreconditionAttribute

In order for your precondition to function, you will need to override
[CheckPermissions].

Your IDE should provide an option to fill this in for you.

Return [PreconditionResult.FromSuccess] if the context met the
required parameters, otherwise return [PreconditionResult.FromError],
optionally including an error message.

[!code-csharp[Custom Precondition](samples/require_owner.cs)]

[CheckPermissions]: xref:Discord.Commands.PreconditionAttribute#Discord_Commands_PreconditionAttribute_CheckPermissions_Discord_Commands_CommandContext_Discord_Commands_CommandInfo_Discord_Commands_IDependencyMap_
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

To create a TypeReader, create a new class that imports @Discord and
@Discord.Commands. Ensure your class inherits from @Discord.Commands.TypeReader

Next, satisfy the `TypeReader` class by overriding [Read].

>[!NOTE]
>In many cases, Visual Studio can fill this in for you, using the
>"Implement Abstract Class" IntelliSense hint.

Inside this task, add whatever logic you need to parse the input string.

Finally, return a `TypeReaderResult`. If you were able to successfully
parse the input, return `TypeReaderResult.FromSuccess(parsedInput)`.
Otherwise, return `TypeReaderResult.FromError`.

[Read]: xref:Discord.Commands.TypeReader#Discord_Commands_TypeReader_Read_Discord_Commands_CommandContext_System_String_

#### Sample

[!code-csharp[TypeReaders](samples/typereader.cs)]

### Installing TypeReaders

TypeReaders are not automatically discovered by the Command Service,
and must be explicitly added. To install a TypeReader, invoke [CommandService.AddTypeReader](xref:Discord.Commands.CommandService#Discord_Commands_CommandService_AddTypeReader__1_Discord_Commands_TypeReader_).
