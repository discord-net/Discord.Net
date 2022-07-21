---
uid: Guides.IntFw.Intro
title: Introduction to the Interaction Service
---

# Getting Started

The Interaction Service provides an attribute based framework for creating Discord Interaction handlers.

To start using the Interaction Service, you need to create a service instance.
Optionally you can provide the [InteractionService] constructor with a
[InteractionServiceConfig] to change the services behaviour to suit your needs.

```csharp
...
// _client here is DiscordSocketClient.
// A different approach to passing in a restclient is also possible.
var _interactionService = new InteractionService(_client.Rest);

...
```

## Modules

Attribute based Interaction handlers must be defined within a command module class.
Command modules are responsible for executing the Interaction handlers and providing them with the necessary execution info and helper functions.

Command modules are transient objects.
A new module instance is created before a command execution starts then it will be disposed right after the method returns.

Every module class must:

- be public
- inherit [InteractionModuleBase]

Optionally you can override the included :

- OnModuleBuilding (executed after the module is built)
- BeforeExecute (executed before a command execution starts)
- AfterExecute (executed after a command execution concludes)

methods to configure the modules behaviour.

Every command module exposes a set of helper methods, namely:

- `RespondAsync()` => Respond to the interaction
- `FollowupAsync()` => Create a followup message for an interaction
- `ReplyAsync()` => Send a message to the origin channel of the interaction
- `DeleteOriginalResponseAsync()` => Delete the original interaction response

## Commands

Valid **Interaction Commands** must comply with the following requirements:

|                               | return type                  | max parameter count | allowed parameter types       | attribute                |
|-------------------------------|------------------------------|---------------------|-------------------------------|--------------------------|
|[Slash Command](#slash-commands)| `Task`/`Task<RuntimeResult>` | 25                  | any*                           | `[SlashCommand]`         |
|[User Command](#user-commands)  | `Task`/`Task<RuntimeResult>` | 1                   | Implementations of `IUser`    | `[UserCommand]`          |
|[Message Command](#message-commands)| `Task`/`Task<RuntimeResult>` | 1                   | Implementations of `IMessage` | `[MessageCommand]`       |
|[Component Interaction Command](#component-interaction-commands)| `Task`/`Task<RuntimeResult>` | inf                 | `string` or `string[]`        | `[ComponentInteraction]` |
|[Autocomplete Command](#autocomplete-commands)| `Task`/`Task<RuntimeResult>` | -             | -                   | `[AutocompleteCommand]`|

> [!NOTE]
> a `TypeConverter` that is capable of parsing type in question must be registered to the [InteractionService] instance.
> You should avoid using long running code in your command module.
> Depending on your setup, long running code may block the Gateway thread of your bot, interrupting its connection to Discord.

## Slash Commands

Slash Commands are created using the [SlashCommandAttribute].
Every Slash Command must declare a name and a description.
You can check Discords **Application Command Naming Guidelines**
[here](https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-naming).

[!code-csharp[Slash Command](samples/intro/slashcommand.cs)]

### Parameters

Slash Commands can have up to 25 method parameters. You must name your parameters in accordance with
[Discords Naming Guidelines](https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-naming).
[InteractionService] also features a pascal casing seperator for formatting parameter names with
pascal casing into Discord compliant parameter names('parameterName' => 'parameter-name').
By default, your methods can feature the following parameter types:

- Implementations of [IUser]
- Implementations of [IChannel]
- Implementations of [IRole]
- Implementations of [IMentionable]
- Implementations of [IAttachment]
- `string`
- `float`, `double`, `decimal`
- `bool`
- `char`
- `sbyte`, `byte`
- `int16`, `int32`, `int64`
- `uint16`, `uint32`, `uint64`
- `enum` (Values are registered as multiple choice options and are enforced by Discord. Use `[HideAttribute]` on enum values to prevent them from getting registered.)
- `DateTime`
- `TimeSpan`

---

**You can use more specialized implementations of [IChannel] to restrict the allowed channel types for a channel type option.*
| interface           | Channel Type                  |
|---------------------|-------------------------------|
| `IStageChannel`     | Stage Channels                |
| `IVoiceChannel`     | Voice Channels                |
| `IDMChannel`        | DM Channels                   |
| `IGroupChannel`     | Group Channels                |
| `ICategory Channel` | Category Channels             |
| `INewsChannel`      | News Channels                 |
| `IThreadChannel`    | Public, Private, News Threads |
| `ITextChannel`      | Text Channels                 |

---

#### Optional Parameters

Parameters with default values (ie. `int count = 0`) will be displayed as optional parameters on Discord Client.

#### Parameter Summary

By using the [SummaryAttribute] you can customize the displayed name and description of a parameter

[!code-csharp[Summary Attribute](samples/intro/summaryattribute.cs)]

#### Parameter Choices

[ChoiceAttribute] can be used to add choices to a parameter.

[!code-csharp[Choice Attribute](samples/intro/groupattribute.cs)]

This Slash Command will be displayed exactly the same as the previous example.

#### Channel Types

Channel types for an [IChannel] parameter can also be restricted using the [ChannelTypesAttribute].

[!code-csharp[Channel Attribute](samples/intro/channelattribute.cs)]

In this case, user can only input Stage Channels and Text Channels to this parameter.

#### Min/Max Value

You can specify the permitted max/min value for a number type parameter using the [MaxValueAttribute] and [MinValueAttribute].

#### Complex Parameters

This allows users to create slash command options using an object's constructor allowing complex objects to be created which cannot be infered from only one input value.
Constructor methods support every attribute type that can be used with the regular slash commands ([Autocomplete], [Summary] etc. ).
Preferred constructor of a Type can be specified either by passing a `Type[]` to the `[ComplexParameterAttribute]` or tagging a type constructor with the `[ComplexParameterCtorAttribute]`. If nothing is specified, the InteractionService defaults to the only public constructor of the type.
TypeConverter pattern is used to parse the constructor methods objects.

[!code-csharp[Complex Parameter](samples/intro/complexparams.cs)]

Interaction service complex parameter constructors are prioritized in the following order:

1. Constructor matching the signature provided in the `[ComplexParameter(Type[])]` overload.
2. Constuctor tagged with `[ComplexParameterCtor]`.
3. Type's only public constuctor.

#### DM Permissions

You can use the [EnabledInDmAttribute] to configure whether a globally-scoped top level command should be enabled in Dms or not. Only works on top level commands.

#### Default Member Permissions

[DefaultMemberPermissionsAttribute] can be used when creating a command to set the permissions a user must have to use the command. Permission overwrites can be configured from the Integrations page of Guild Settings. [DefaultMemberPermissionsAttribute] cumulatively propagates down the class hierarchy until it reaches a top level command. This attribute can be only used on top level commands and will not work on commands that are nested in command groups.

## User Commands

A valid User Command must have the following structure:

[!code-csharp[User Command](samples/intro/usercommand.cs)]

> [!WARNING]
> User commands can only have one parameter and its type must be an implementation of [IUser].

## Message Commands

A valid Message Command must have the following structure:

[!code-csharp[Message Command](samples/intro/messagecommand.cs)]

> [!WARNING]
> Message commands can only have one parameter and its type must be an implementation of [IMessage].

## Component Interaction Commands

Component Interaction Commands are used to handle interactions that originate from **Discord Message Component**s.
This pattern is particularly useful if you will be reusing a set a **Custom ID**s.

Component Interaction Commands support wild card matching,
by default `*` character can be used to create a wild card pattern.
Interaction Service will use lazy matching to capture the words corresponding to the wild card character.
And the captured words will be passed on to the command method in the same order they were captured.

[!code-csharp[Button](samples/intro/button.cs)]

You may use as many wild card characters as you want.

> [!NOTE]
> If Interaction Service recieves a component interaction with **player:play,rickroll** custom id,
> `op` will be *play* and `name` will be *rickroll*

## Select Menus

Unlike button interactions, select menu interactions also contain the values of the selected menu items.
In this case, you should structure your method to accept a string array.

[!code-csharp[Dropdown](samples/intro/dropdown.cs)]

> [!NOTE]
> Wildcards may also be used to match a select menu ID,
> though keep in mind that the array containing the select menu values should be the last parameter.

## Autocomplete Commands

Autocomplete commands must be parameterless methods. A valid Autocomplete command must have the following structure:

[!code-csharp[Autocomplete Command](samples/intro/autocomplete.cs)]

Alternatively, you can use the [AutocompleteHandlers] to simplify this workflow.

## Modals

Modal commands last parameter must be an implementation of `IModal`.
A Modal implementation would look like this:

[!code-csharp[Modal Command](samples/intro/modal.cs)]

> [!NOTE]
> If you are using Modals in the interaction service it is **highly 
> recommended** that you enable `PreCompiledLambdas` in your config
> to prevent performance issues. 

## Interaction Context

Every command module provides its commands with an execution context.
This context property includes general information about the underlying interaction that triggered the command execution.
The base command context.

You can design your modules to work with different implementation types of [IInteractionContext].
To achieve this, make sure your module classes inherit from the generic variant of the [InteractionModuleBase].

> [!NOTE]
> Context type must be consistent throughout the project, or you will run into issues during runtime.

The [InteractionService] ships with 4 different kinds of [InteractionContext]:

1. [InteractionContext]]: A bare-bones execution context consisting of only implementation neutral interfaces
2. [SocketInteractionContext]: An execution context for use with [DiscordSocketClient]. Socket entities are exposed in this context without the need of casting them.
3. [ShardedInteractionContext]: [DiscordShardedClient] variant of the [SocketInteractionContext]
4. [RestInteractionContext]: An execution context designed to be used with a [DiscordRestClient] and webhook based interactions pattern

You can create custom Interaction Contexts by implementing the [IInteractionContext] interface.

One problem with using the concrete type InteractionContexts is that you cannot access the information that is specific to different interaction types without casting. Concrete type interaction contexts are great for creating shared interaction modules but you can also use the generic variants of the built-in interaction contexts to create interaction specific interaction modules.

> [!INFO]
> Message component interactions have access to a special method called `UpdateAsync()` to update the body of the method the interaction originated from.
> Normally this wouldn't be accessable without casting the `Context.Interaction`.

[!code-csharp[Context Example](samples/intro/context.cs)]

## Loading Modules

[InteractionService] can automatically discover and load modules that inherit [InteractionModuleBase] from an `Assembly`.
Call `InteractionService.AddModulesAsync()` to use this functionality.

> [!NOTE]
> You can also manually add Interaction modules using the `InteractionService.AddModuleAsync()`
> method by providing the module type you want to load.

## Resolving Module Dependencies

Module dependencies are resolved using the Constructor Injection and Property Injection patterns.
Meaning, the constructor parameters and public settable properties of a module will be assigned using the `IServiceProvider`.
For more information on dependency injection, read the [DependencyInjection] guides.

> [!NOTE]
> On every command execution, module dependencies are resolved using a new service scope which allows you to utilize scoped service instances, just like in Asp.Net.
> Including the precondition checks, every module method is executed using the same service scope and service scopes are disposed right after the `AfterExecute` method returns.

## Module Groups

Module groups allow you to create sub-commands and sub-commands groups.
By nesting commands inside a module that is tagged with [GroupAttribute] you can create prefixed commands.

> [!WARNING]
> Although creating nested module stuctures are allowed,
> you are not permitted to use more than 2 [GroupAttribute]'s in module hierarchy.

[!code-csharp[Command Group Example](samples/intro/groupmodule.cs)]

## Executing Commands

Any of the following socket events can be used to execute commands:

- [InteractionCreated]
- [ButtonExecuted]
- [SelectMenuExecuted]
- [AutocompleteExecuted]
- [UserCommandExecuted]
- [MessageCommandExecuted]

Commands can be either executed on the gateway thread or on a seperate thread from the thread pool. This behaviour can be configured by changing the *RunMode* property of `InteractionServiceConfig` or by setting the *runMode* parameter of a command attribute.

You can also configure the way [InteractionService] executes the commands.
By default, commands are executed using `ConstructorInfo.Invoke()` to create module instances and
`MethodInfo.Invoke()` method for executing the method bodies.
By setting, `InteractionServiceConfig.UseCompiledLambda` to `true`, you can make [InteractionService] create module instances and execute commands using
*Compiled Lambda* expressions. This cuts down on command execution time but it might add some memory overhead.

Time it takes to create a module instance and execute a `Task.Delay(0)` method using the Reflection methods compared to Compiled Lambda expressions:

|           Method |      Mean |    Error |   StdDev |
|----------------- |----------:|---------:|---------:|
| ReflectionInvoke | 225.93 ns | 4.522 ns | 7.040 ns |
|   CompiledLambda |  48.79 ns | 0.981 ns | 1.276 ns |

## Registering Commands to Discord

Application commands loaded to the Interaction Service can be registered to Discord using a number of different methods.
In most cases `RegisterCommandsGloballyAsync()` and `RegisterCommandsToGuildAsync()` are the methods to use.
Command registration methods can only be used after the gateway client is ready or the rest client is logged in.

[!code-csharp[Registering Commands Example](samples/intro/registering.cs)]

Methods like `AddModulesToGuildAsync()`, `AddCommandsToGuildAsync()`, `AddModulesGloballyAsync()` and `AddCommandsGloballyAsync()`
can be used to register cherry picked modules or commands to global/guild scopes.

> [!NOTE]
> In debug environment, since Global commands can take up to 1 hour to register/update,
> it is adviced to register your commands to a test guild for your changes to take effect immediately.
> You can use preprocessor directives to create a simple logic for registering commands as seen above

## Interaction Utility

Interaction Service ships with a static `InteractionUtiliy`
class which contains some helper methods to asynchronously waiting for Discord Interactions.
For instance, `WaitForInteractionAsync()` method allows you to wait for an Interaction for a given amount of time.
This method returns the first encountered Interaction that satisfies the provided predicate.

> [!WARNING]
> If you are running the Interaction Service on `RunMode.Sync` you should avoid using this method in your commands,
> as it will block the gateway thread and interrupt your bots connection.

## Webhook Based Interactions

Instead of using the gateway to recieve Discord Interactions, Discord allows you to recieve Interaction events over Webhooks.
Interaction Service also supports this Interaction type but to be able to
respond to the Interactions within your command modules you need to perform the following:

- Make your modules inherit `RestInteractionModuleBase`
- Set the `ResponseCallback` property of `InteractionServiceConfig` so that the `ResponseCallback`
delegate can be used to create HTTP responses from a deserialized json object string.
- Use the interaction endpoints of the module base instead of the interaction object (ie. `RespondAsync()`, `FollowupAsync()`...).

[AutocompleteHandlers]: xref:Guides.IntFw.AutoCompletion
[DependencyInjection]: xref:Guides.TextCommands.DI

[GroupAttribute]: xref:Discord.Interactions.GroupAttribute
[InteractionService]: xref:Discord.Interactions.InteractionService
[InteractionServiceConfig]: xref:Discord.Interactions.InteractionServiceConfig
[InteractionModuleBase]: xref:Discord.Interactions.InteractionModuleBase
[SlashCommandAttribute]: xref:Discord.Interactions.SlashCommandAttribute
[InteractionCreated]: xref:Discord.WebSocket.BaseSocketClient
[ButtonExecuted]: xref:Discord.WebSocket.BaseSocketClient
[SelectMenuExecuted]: xref:Discord.WebSocket.BaseSocketClient
[AutocompleteExecuted]: xref:Discord.WebSocket.BaseSocketClient
[UserCommandExecuted]: xref:Discord.WebSocket.BaseSocketClient
[MessageCommandExecuted]: xref:Discord.WebSocket.BaseSocketClient
[DiscordSocketClient]: xref:Discord.WebSocket.DiscordSocketClient
[DiscordRestClient]: xref:Discord.Rest.DiscordRestClient
[SocketInteractionContext]: xref:Discord.Interactions.SocketInteractionContext
[ShardedInteractionContext]: xref:Discord.Interactions.ShardedInteractionContext
[InteractionContext]: xref:Discord.Interactions.InteractionContext
[IInteractionContect]: xref:Discord.Interactions.IInteractionContext
[RestInteractionContext]: xref:Discord.Rest.RestInteractionContext
[SummaryAttribute]: xref:Discord.Interactions.SummaryAttribute
[ChoiceAttribute]: xref:Discord.Interactions.ChoiceAttribute
[ChannelTypesAttribute]: xref:Discord.Interactions.ChannelTypesAttribute
[MaxValueAttribute]: xref:Discord.Interactions.MaxValueAttribute
[MinValueAttribute]: xref:Discord.Interactions.MinValueAttribute

[IChannel]: xref:Discord.IChannel
[IRole]: xref:Discord.IRole
[IUser]: xref:Discord.IUser
[IMessage]: xref:Discord.IMessage
[IMentionable]: xref:Discord.IMentionable
