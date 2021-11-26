---
uid: Guides.InteractionsFramework.Intro
title: Introduction to the Interaction Framework
---

# Getting Started

Interaction Service provides an attribute based framework for creating Discord Interaction handlers.

To start using the Interaction Service, you need to create a service instance. Optionally you can provide the `InterctionService` constructor with a `InteractionServiceConfig` to change the services behaviour to suit your needs.

```csharp
...

var commands = new InteractionService(discord);

...
```

## Modules

Attribute based Interaction handlers must be defined within a command module class. Command modules are responsible for executing the Interaction handlers and providing them with the necessary execution info and helper functions.

Command modules are transient objects. A new module instance is created before a command execution starts then it will be disposed right after the method returns.

Every module class must:

- be public
- inherit `InteractionModuleBase`

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
> a `TypeConverter` that is capable of parsing type in question must be registered to the `InteractionService` instance. 

You should avoid using long running code in your command module. Depending on your setup, long running code may block the Gateway thread of your bot, interrupting its connection to Discord.

### Slash Commands

Slash Commands are created using the `[SlashCommandAttribute]`. Every Slash Command must declare a name and a description. You can check Discords **Application Command Naming Guidelines** [here](https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-naming).

```csharp
[SlashCommand("echo", "Echo an input")]
public async Task Echo(string input)
{
    await RespondAsync(input);
}
```

#### Parameters

Slash Commands can have up to 25 method parameters. You must name your parameters in accordance with [Discords Naming Guidelines](https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-naming). Interaction Service also features a pascal casing seperator for formatting parameter names with pascal casing into Discord compliant parameter names('parameterName' => 'parameter-name'). By default, your methods can feature the following parameter types:

- Implementations of `IUser`
- Implementations of `IChannel`*
- Implementations of `IRole`
- Implementations of `IMentionable`
- `string`
- `float`, `double`, `decimal`
- `bool`
- `char`
- `sbyte`, `byte`
- `int16`, `int32`, `int64`
- `uint16`, `uint32`, `uint64`
- `enum` (Values are registered as multiple choice options and are enforced by Discord. Use `[HideAttribute]' on enum values to prevent them from getting registered.)
- `DateTime`
- `TimeSpan`

---

**You can use more specialized implementations of `IChannel` to restrict the allowed channel types for a channel type option.*
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

##### Optional Parameters

Parameters with default values (ie. `int count = 0`) will be displayed as optional parameters on Discord Client.

##### Parameter Summary

By using the `[SummaryAttribute]` you can customize the displayed name and description of a parameter

```csharp
[Summary(description: "this is a parameter description")] string input
```

##### Parameter Choices

`[ChoiceAttribute]` can be used to add choices to a parameter.

```csharp
[SlashCommand("blep", "Send a random adorable animal photo")]
public async Task Blep([Choice("Dog","dog"), Choice("Cat", "cat"), Choice("Penguin", "penguin")] string animal)
{
    ...
}
```

In most cases, instead of relying on this attribute, you should use an `Enum` to create multiple choice parameters. Ex.

```csharp
public enum Animal
{
    Cat,
    Dog,
    Penguin
}

[SlashCommand("blep", "Send a random adorable animal photo")]
public async Task Blep(Animal animal)
{
    ...
}
```

This Slash Command will be displayed exactly the same as the previous example.

##### Channel Types

Channel types for an `IChannel` parameter can also be restricted using the `[ChannelTypesAttribute]`.

```csharp
[SlashCommand("name", "Description")]
public async Task Command([ChannelTypes(ChannelType.Stage, ChannelType.Text)]IChannel channel)
{
    ...
}
```

In this case, user can only input Stage Channels and Text Channels to this parameter.

##### Autocomplete

You can enable Autocomple Interactions for a Slash Command parameter using the `[AutocompleteAttribute]`. To handle the Autocomplete Interactions raised by this parameter you can either create [Autocomplete Commands](#autocomplete-commands) or you can opt to use the [Autocompleters Pattern](./autocompleters)

##### Min/Max Value

You can specify the permitted max/min value for a number type parameter using the `[MaxValueAttribute]` and `[MinValueAttribute]`.

### User Commands

A valid User Command must have the following structure:

```csharp
[UserCommand("Say Hello")]
public async Task SayHello(IUser user)
{
    ...
}
```

User commands can only have one parameter and its type must be an implementation of `IUser`.

### Message Commands

A valid Message Command must have the following structure:

```csharp
[MessageCommand("Bookmark")]
public async Task Bookmark(IUser user)
{
    ...
}
```

Message commands can only have one parameter and its type must be an implementation of `IMessage`.

### Component Interaction Commands

Component Interaction Commands are used to handle interactions that originate from **Discord Message Component**s. This pattern is particularly useful if you will be reusing a set a **Custom ID**s.

```csharp
[ComponentInteraction("custom_id")]
public async Task RoleSelection()
{
    ...
}
```

Component Interaction Commands support wild card matching, by default `*` character can be used to create a wild card pattern. Interaction Service will use lazy matching to capture the words corresponding to the wild card character. And the captured words will be passed on to the command method in the same order they were captured.

*Ex.*

If Interaction Service recieves a component interaction with **player:play,rickroll** custom id, `op` will be *play* and `name` will be *rickroll*

```csharp
[ComponentInteraction("player:*,*")]
public async Task Play(string op, string name)
{
    ...
}
```

You may use as many wild card characters as you want.

#### Select Menus

Unlike button interactions, select menu interactions also contain the values of the selected menu items. In this case, you should structure your method to accept a string array.

```csharp
[ComponentInteraction("role_selection")]
public async Task RoleSelection(string[] selectedRoles)
{
    ...
}
```

 Wild card pattern can also be used to match select menu custom ids but remember that the array containing the select menu values should be the last parameter.

```csharp
[ComponentInteraction("role_selection_*")]
public async Task RoleSelection(string id, string[] selectedRoles)
{
    ...
}
```

### Autocomplete Commands

Autocomplete commands must be parameterless methods. A valid Autocomplete command must  have the following structure:

```csharp
[AutocompleteCommand("command_name", "parameter_name")]
public async Task Autocomplete()
{
    IEnumerable<AutocompleteResult> results;

    ...

    await (Context.Interaction as SocketAutocompleteInteraction).RespondAsync(results);
}
```

Alternatively, you can use the *Autocompleters* to simplify this workflow.

## Interaction Context

Every command module provides its commands with an execution context. This context property includes general information about the underlying interaction that triggered the command execution. The base command context.

You can design your modules to work with different implementation types of `IInteractionContext`. To achieve this, make sure your module classes inherit from the generic variant of the `InteractionModuleBase`.

> Context type must be consistent throughout the project, or you will run into issues during runtime.

Interaction Service ships with 4 different kinds of `InteractionContext`s:

1. InteractionContext: A bare-bones execution context consisting of only implementation netural interfaces
2. SocketInteractionContext: An execution context for use with `DiscordSocketClient`. Socket entities are exposed in this context without the need of casting them.
3. ShardedInteractionContext: `DiscordShardedClient` variant of the `SocketInteractionContext`
4. RestInteractionContext: An execution context designed to be used with a `DiscordRestClient` and webhook based interactions pattern

You can create custom Interaction Contexts by implementing the `IInteracitonContext` interface.

One problem with using the concrete type InteractionContexts is that you cannot access the information that is specific to different interaction types without casting. Concrete type interaction contexts are great for creating shared interaction modules but you can also use the generic variants of the built-in interaction contexts to create interaction specific interaction modules.

Ex.
Message component interactions have access to a special method called `UpdateAsync()` to update the body of the method the interaction originated from. Normally this wouldn't be accessable without casting the `Context.Interaction`.

```csharp
discordClient.ButtonExecuted += async (interaction) => 
{
    var ctx = new SocketInteractionContext<SocketMessageComponent>(discordClient, interaction);
    await interactionService.ExecuteAsync(ctx, serviceProvider);
};

public class MessageComponentModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    [ComponentInteraction("custom_id")]
    public async Command()
    {
        Context.Interaction.UpdateAsync(...);
    }
}
```

## Loading Modules

Interaction Service can automatically discover and load modules that inherit `InteractionModuleBase` from an `Assembly`. Call `InteractionService.AddModulesAsync()` to use this functionality.

You can also manually add Interaction modules using the `InteractionService.AddModuleAsync()` method by providing the module type you want to load.

## Resolving Module Dependencies

Module dependencies are resolved using the Constructor Injection and Property Injection patterns. Meaning, the constructor parameters and public settable properties of a module will be assigned using the `IServiceProvider`. For more information on dependency injection, check out [Dependency Injection](.\dependency-injection)

## Module Groups

Module groups allow you to create sub-commands and sub-commands groups. By nesting commands inside a module that is tagged with `[GroupAttribute]` you can create prefixed commands.

Although creating nested module stuctures are allowed, you are not permitted to use more than 2 `[GroupAttribute]`s in module hierarchy.

## Executing Commands

Any of the following socket events can be used to execute commands:

- InteractionCreated
- ButtonExecuted
- SelectMenuExecuted
- AutocompleteExecuted
- UserCommandExecuted
- MessageCommandExecuted

Commands can be either executed on the gateway thread or on a seperate thread from the thread pool. This behaviour can be configured by changing the *RunMode* property of `InteractionServiceConfig` or by setting the *runMode* parameter of a command attribute.

You can also configure the way `InteractionService` executes the commands. By default, commands are executed using `ConstructorInfo.Invoke()` to create module instances and `MethodInfo.Invoke()` method for executing the method bodies. By setting, `InteractionServiceConfig.UseCompiledLambda` to `true`, you can make `InteractionService` create module instances and execute commands using *Compiled Lambda* expressions. This cuts down on command execution time but it might add some memory overhead.

Time it takes to create a module instance and execute a `Task.Delay(0)` method using the Reflection methods compared to Compiled Lambda expressions:

|           Method |      Mean |    Error |   StdDev |
|----------------- |----------:|---------:|---------:|
| ReflectionInvoke | 225.93 ns | 4.522 ns | 7.040 ns |
|   CompiledLambda |  48.79 ns | 0.981 ns | 1.276 ns |

## Registering Commands to Discord

Application commands loaded to the Interaciton Service can be registered to Discord using a number of different methods. In most cases `RegisterCommandsGloballyAsync()` and `RegisterCommandsToGuildAsync()` are the methods to use. Command registration methods can only be used after the gateway client is ready or the rest client is logged in.

In debug environment, since Global commands can take up to 1 hour to register/update, you should register your commands to a test guild for your changes to take effect immediately. You can use the preprocessor directives to create a simple logic for registering commands:

```csharp
#if DEBUG
    await interactionService.RegisterCommandsToGuildAsync(<test_guild_id>);
#else
    await interactionService.RegisterCommandsGloballyAsync();
#endif
```
