---
uid: FAQ.Commands.Interactions
title: Interaction service
---

# Interaction commands in services

A chapter talking about the interaction service framework.
For questions about interactions in general, refer to the [Interactions FAQ]

## Module dependencies aren't getting populated by Property Injection?

Make sure the properties are publicly accessible and publicly settable.

## How do I use this * interaction specific method/property?

If your interaction context holds a down-casted version of the interaction object, you need to up-cast it.
Ideally, use pattern matching to make sure its the type of interaction you are expecting it to be.

## `InteractionService.ExecuteAsync()` always returns a successful result, how do i access the failed command execution results?

If you are using `RunMode.Async` you need to setup your post-execution pipeline around `CommandExecuted` events.

## How do I check if the executing user has * permission?

Refer to the [documentation about preconditions]

## How do I send the HTTP Response from inside the command modules.

Set the `RestResponseCallback` property of [InteractionServiceConfig] with a delegate for handling HTTP Responses and use
`RestInteractionModuleBase` to create your command modules. `RespondAsync()` and `DeferAsync()` methods of this module base will use the
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
[documentation about preconditions]: xref: Guides.ChatCommands.Preconditions
