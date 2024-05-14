---
uid: FAQ.Interactions.Framework
title: Interaction Framework
---

# The Interaction Framework

Common misconceptions and questions about the Interaction Framework.

## How can I restrict some of my commands so only specific users can execute them?

Based on how you want to implement the restrictions, you can use the
built-in `RequireUserPermission` precondition, which allows you to
restrict the command based on the user's current permissions in the
guild or channel (*e.g., `GuildPermission.Administrator`,
`ChannelPermission.ManageMessages`*).

[RequireUserPermission]: xref:Discord.Commands.RequireUserPermissionAttribute

> [!NOTE]
> There are many more preconditions to use, including being able to make some yourself.
> Examples on self-made preconditions can be found
> [here](https://github.com/discord-net/Discord.Net/blob/dev/samples/InteractionFramework/Attributes/RequireOwnerAttribute.cs)

## Why do preconditions not hide my commands?

In the current permission design by Discord,
it is not very straight forward to limit vision of slash/context commands to users.
If you want to hide commands, you should take a look at the commands' `DefaultPermissions` parameter.

## Module dependencies aren't getting populated by Property Injection?

Make sure the properties are publicly accessible and publicly settable.

[!code-csharp[Property Injection](samples/propertyinjection.cs)]

## `InteractionService.ExecuteAsync()` always returns a successful result, how do i access the failed command execution results?

If you are using `RunMode.Async` you need to setup your post-execution pipeline around
`..Executed` events exposed by the Interaction Service.

## How do I check if the executing user has * permission?

Refer to the [documentation about preconditions]

[documentation about preconditions]: xref:Guides.IntFw.Preconditions

## How do I send the HTTP Response from inside the command modules.

Set the `RestResponseCallback` property of [InteractionServiceConfig] with a delegate for handling HTTP Responses and use
`RestInteractionModuleBase` to create your command modules. `RespondWithModalAsync()`, `RespondAsync()` and `DeferAsync()` methods of this module base will use the
`RestResponseCallback` to create interaction responses.

## Is there a cleaner way of creating parameter choices other than using `[Choice]`?

The default `enum` [TypeConverter] of the Interaction Service will
automatically register `enum`s as multiple choice options.

## How do I add an optional `enum` parameter but make the default value not visible to the user?

The default `enum` [TypeConverter] of the Interaction Service comes with `[Hide]` attribute that
can be used to prevent certain enum values from getting registered.

## How does the InteractionService determine the generic TypeConverter to use for a parameter type?

It compares the _target base type_ key of the
[TypeConverter] and chooses the one that sits highest on the inheritance hierarchy.

[TypeConverter]: xref:Discord.Interactions.TypeConverter
[Interactions FAQ]: xref: FAQ.Basics.Interactions
[InteractionServiceConfig]: xref:Discord.Interactions.InteractionServiceConfig
