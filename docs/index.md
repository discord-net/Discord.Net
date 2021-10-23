---
uid: Root.Landing
title: Home
---

# Discord.NET Labs Documentation

<div class="big-logo logo-switcher"></div>

[![GitHub](https://img.shields.io/github/last-commit/Discord-Net-Labs/Discord.Net-Labs?style=plastic)](https://github.com/Discord-Net-Labs/Discord.Net-Labs)
[![NuGet](https://img.shields.io/nuget/vpre/Discord.Net.Labs.svg?maxAge=2592000?style=plastic)](https://www.nuget.org/packages/Discord.Net.Labs/)
[![MyGet](https://img.shields.io/myget/discord-net-labs/vpre/Discord.Net.Labs.svg)](https://www.myget.org/feed/Packages/discord-net-labs)
[![Build Status](https://dev.azure.com/Discord-Net-Labs/Discord-Net-Labs/_apis/build/status/discord-net.Discord.Net?branchName=dev)](https://dev.azure.com/Discord-Net-Labs/Discord-Net-Labs/_build/latest?definitionId=1&branchName=release%2F3.x)
[![Discord](https://discord.com/api/guilds/848176216011046962/widget.png)](https://discord.gg/dnet-labs)

## What is Discord.NET Labs?

Discord.NET Labs is an experimental fork of Discord.NET that implements the newest discord features for testing and development to eventually get merged into Discord.NET. Serving as a developer branch to the default project, it is a drop in replacement and can be used instead. It is ill advised to use Discord.NET Labs in a production environment normally. However if approached correctly, will work as an improved and up-to-date  replacement to Discord.NET!

## Where to begin?

If you are new to Discord.NET in general, you should refer their 
[Documentation](https://docs.stillu.cc/) for guides & examples.

Is this your first time using Labs, but you are already familiar with Discord.NET? 
Refer to our [Guides](xref:Guides.Introduction)

## Questions?

Frequently asked questions are covered in the 
[FAQ](https://discord-net-labs.com/FAQ.html). Read it thoroughly because most common questions are already answered there. 

If you still have unanswered questions after reading the FAQ, further support is available on 
[Discord](https://discord.gg/dnet-labs).

## Commonly used features

#### Slash commands

Slash commands are purposed to take over the normal prefixed commands in Discord and comes with good functionality to serve as a replacement. 
Being interactions, they are handled as SocketInteractions. Creating and receiving slashcommands is covered below.

- Find out more about slash commands in the 
[Slash Command Guides](xref:Guides.SlashCommands.Intro)

#### Message & User commands

These commands can be pointed at messages and users, in custom application tabs. 
Being interactions as well, they are able to be handled just like slash commands. They do not have options however.

- Learn how to create and handle these commands in the 
[Context Command Guides](xref:Guides.ContextCommands.Creating)

#### Message components

Components of a message such as buttons and dropdowns, which can be interacted with and responded to. 
Message components can be set in rows and multiple can exist on a single message!

- Explanation on how to add & respond to message components can be found in the 
[Message Component Guides](xref:Guides.MessageComponents.GettingStarted)

#### Note

More experienced users might want to refer to the
[API Documentation](xref:API.Docs) for a breakdown of the individual
components in the library.

