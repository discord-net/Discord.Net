# The Command Service

[Discord.Commands](xref:Discord.Commands) provides an Attribute-based Command Parser.

### Setup

To use Commands, you must create a [Commands Service](xref:Discord.Commands.CommandService) and a Command Handler.

Included below is a very bare-bones Command Handler. You can extend your Command Handler as much as you like, however the below is the bare minimum.

[!code-csharp[Barebones Command Handler](samples/command_handler.cs)]

## Modules

Modules serve as a host for commands you create. 

To create a module, create a class that you will place commands in. Flag this class with the `[Module]` attribute. You may optionally pass in a string to the `Module` attribute to set a prefix for all of the commands inside the module.

### Example Module

[!code-csharp[Modules](samples/module.cs)]

#### Loading Modules Automatically

The Command Service can automatically discover all classes in an Assembly that are flagged with the `Module` attribute, and load them.

To have a module opt-out of auto-loading, pass `autoload: false` in the Module attribute.

Invoke [CommandService.LoadAssembly](xref:Discord.Commands.CommandService#Discord_Commands_CommandService_LoadAssembly) to discover modules and install them.

#### Loading Modules Manually

To manually load a module, invoke [CommandService.Load](xref:Discord.Commands.CommandService#Discord_Commands_CommandService_Load), and pass in an instance of your module.

### Module Constructors

When automatically loading modules, you are limited in your constructor. Using a constructor that accepts _no arguments_, or a constructor that accepts a @Discord.Commands.CommandService will always work.

Alternatively, you can use an @Discord.Commands.IDependencyMap, as shown below.

## Dependency Injection

The Commands Service includes a very basic implementation of Dependency Injection that allows you to have completely custom constructors, within certain limitations.

### Setup

First, you need to create an @Discord.Commands.IDependencyMap . The library includes @Discord.Commands.DependencyMap to help with this, however you may create your own IDependencyMap if you wish. 

Next, add the dependencies your modules will use to the map. 

Finally, pass the map into the `LoadAssembly` method. Your modules will automatically be loaded with this dependency map.

[!code-csharp[DependencyMap Setup](samples/dependency_map_setup.cs)]

### Usage in Modules

In the constructor of your module, any parameters will be filled in by the @Discord.Commands.IDependencyMap you pass into `LoadAssembly`.

>[!NOTE]
>If you accept `CommandService` or `IDependencyMap`  as a parameter in your constructor, these parameters will be filled by the CommandService the module was loaded from, and the DependencyMap passed into it, respectively. 

[!code-csharp[DependencyMap in Modules](samples/dependency_module.cs)]

## Type Readers

Type Readers allow you to parse different types of arguments in your commands.

By default, the following Types are supported arguments:

- string
- sbyte/byte
- ushort/short
- uint/int
- ulong/long
- float, double, decimal
- DateTime/DateTimeOffset
- IUser/IGuildUser
- IChannel/IGuildChannel/ITextChannel/IVoiceChannel/IGroupChannel
- IRole
- IMessage

### Creating a Type Readers

To create a TypeReader, create a new class that imports @Discord and @Discord.Commands . Ensure your class inherits from @Discord.Commands.TypeReader

Next, satisfy the `TypeReader` class by overriding `Task<TypeReaderResult> Read(IMessage context, string input)`.

>[!NOTE]
>In many cases, Visual Stuido can fill this in for you, using the "Implement Abstract Class" IntelliSense hint.

Inside this task, add whatever logic you need to parse the input string.

Finally, return a `TypeReaderResult`. If you were able to successfully parse the input, return `TypeReaderResult.FromSuccess(parsedInput)`. Otherwise, return `TypeReaderResult.FromError`.

#### Sample

[!code-csharp[TypeReaders](samples/typereader.cs)]

### Installing TypeReaders

TypeReaders are not automatically discovered by the Command Service, and must be explicitly added. To install a TypeReader, invoke [CommandService.AddTypeReader](xref:Discord.Commands.CommandService#Discord_Commands_CommandService_AddTypeReader__1_Discord_Commands_TypeReader_).