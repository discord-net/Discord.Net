---
uid: FAQ.Interactions.General
title: Interaction Basics
---

# Interaction Basics

This chapter mostly refers to interactions in general,
and will include questions that are common among users of the Interaction Framework
as well as users that register and handle commands manually.

## What's the difference between RespondAsync, DeferAsync and FollowupAsync?

The difference between these 3 functions is in how you handle the command response.
[RespondAsync] and
[DeferAsync] let the API know you have successfully received the command. This is also called 'acknowledging' a command.
DeferAsync will not send out a response, RespondAsync will.
[FollowupAsync] follows up on successful acknowledgement.

> [!WARNING]
> If you have not acknowledged the command FollowupAsync will not work! the interaction has not been responded to, so you cannot follow it up!

[RespondAsync]: xref:Discord.IDiscordInteraction
[DeferAsync]: xref:Discord.IDiscordInteraction
[FollowUpAsync]: xref:Discord.IDiscordInteraction

## Im getting System.TimeoutException: 'Cannot respond to an interaction after 3 seconds!'

This happens because your computer's clock is out of sync or you're trying to respond after 3 seconds.
If your clock is out of sync and you can't fix it, you can set the `UseInteractionSnowflakeDate` to false in the [DiscordSocketConfig].

[!code-csharp[Interaction Sync](samples/interactionsyncing.cs)]

[DiscordClientConfig]: xref:Discord.WebSocket.DiscordSocketConfig

## How do I use this * interaction specific method/property?

If your interaction context holds a down-casted version of the interaction object, you need to up-cast it.
Ideally, use pattern matching to make sure its the type of interaction you are expecting it to be.

> [!NOTE]
> Further documentation on pattern matching can be found [here](xref:Guides.Entities.Casting).

## My interaction commands are not showing up?

- Try to check for any errors in the console, there is a good chance something might have been thrown.
- - Make sure you have setup logging. If you use `InteractionService` hook into [`InteractionService.Log`]) event

- Register your commands after the Ready event in the client. The client is not configured to register commands before this moment.

- Check if no bad form exception is thrown;

- Do you have the application commands scope checked when adding your bot to guilds?

- Try reloading your Discord client. On desktop it's done with `Ctrl+R` key combo.

## Do I need to create commands on startup?

If you are registering your commands for the first time, it is required to create them once.
After this, commands will exist indefinitely until you overwrite them.
Overwriting is only required if you make changes to existing commands, or add new ones.

## I can't see all of my user/message commands, why?

Message and user commands have a limit of 5 per guild, and another 5 globally.
If you have more than 5 guild-only message commands being registered, no more than 5 will actually show up.
You can get up to 10 entries to show if you register 5 per guild, and another 5 globally.


[`InteractionService.Log`]: xref:Discord.Interactions.InteractionService.Log
