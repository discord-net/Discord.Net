---
uid: Root.Landing
title: Home
---

# Discord.NET Documentation

<div class="big-logo logo-switcher"></div>

[![GitHub](https://img.shields.io/github/last-commit/discord-net/Discord.Net?style=plastic)](https://github.com/discord-net/Discord.Net)
[![NuGet](https://img.shields.io/nuget/vpre/Discord.Net.svg?maxAge=2592000?style=plastic)](https://www.nuget.org/packages/Discord.Net)
[![MyGet](https://img.shields.io/myget/discord-net/vpre/Discord.Net.svg)](https://www.myget.org/feed/Packages/discord-net)
[![Build Status](https://dev.azure.com/discord-net/Discord.Net/_apis/build/status/discord-net.Discord.Net?branchName=dev)](https://dev.azure.com/discord-net/Discord.Net/_build/latest?definitionId=1&branchName=dev)
[![Discord](https://discord.com/api/guilds/848176216011046962/widget.png)](https://discord.gg/dnet)

## What is Discord.NET?

Discord.Net is an asynchronous, multi-platform .NET Library used to
interface with the [Discord API](https://discord.com/).

## Where to begin?

If this is your first time using Discord.Net, you should refer to the
[Intro](xref:Guides.Introduction) for tutorials.

If you're coming from Discord.Net V2, you should refer to the [V2 -> V3](xref:Guides.V2V3Guide) guides.

More experienced users might want to refer to the
[API Documentation](xref:API.Docs) for a breakdown of the individual
objects in the library.

## Nightlies

Nightlies are builds of Discord.NET that are still in an experimental phase, and have not been released.
These are not included in the main repository, and are instead taken over by [Discord.NET Labs].

Discord.NET Labs is an experimental fork of Discord.NET that implements the newest discord features
for testing and development to eventually get merged into Discord.NET.

[Installing Discord.NET Labs](xref:Guides.GettingStarted.Installation.Labs)

[Discord.Net Labs]: https://github.com/Discord-Net-Labs/Discord.Net-Labs

## Questions?

Frequently asked questions are covered in the
FAQ. Read it thoroughly because most common questions are already answered there.

If you still have unanswered questions after reading the [FAQ](xref:FAQ.Basics.GetStarted), further support is available on
[Discord](https://discord.gg/dnet).

## New in V3

#### Interaction Framework

A counterpart to the well-known command service of Discord.Net, the Interaction Framework implements the same
feature-rich structure to register & handle interactions like application commands & buttons.

- Read about the Interaction Framework
  [here](xref:Guides.IntFw.Intro)

#### Slash Commands

Slash commands are purposed to take over the normal prefixed commands in Discord and comes with good functionality to serve as a replacement.
Being interactions, they are handled as SocketInteractions. Creating and receiving slash commands is covered below.

- Find out more about slash commands in the
  [Slash Command Guides](xref:Guides.SlashCommands.Intro)

#### Context Message & User Commands

These commands can be pointed at messages and users, in custom application tabs.
Being interactions as well, they are able to be handled just like slash commands. They do not have options however.

- Learn how to create and handle these commands in the
  [Context Command Guides](xref:Guides.ContextCommands.Creating)

#### Message Components

Components of a message such as buttons and dropdowns, which can be interacted with and responded to.
Message components can be set in rows and multiple can exist on a single message!

- Explanation on how to add & respond to message components can be found in the
  [Message Component Guides](xref:Guides.MessageComponents.Intro)
