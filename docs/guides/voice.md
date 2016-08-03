# Voice

**Information on this page is subject to change!**

>[!WARNING]
>Audio in 1.0 is incomplete. Most of the below documentation is untested.

## Installation

To use Audio, you must first configure your `DiscordSocketClient` with Audio support.

In your @Discord.DiscordSocketConfig, set `AudioMode` to the appropriate @Discord.Audio.AudioMode for your bot. For most bots, you will only need to use `AudioMode.Outgoing`.

### Dependencies

Audio requires two native libraries, `libsodium` and `opus`. Both of these libraries must be placed in the runtime directory of your bot (for .NET 4.6, the directory where your exe is located; for .NET core, directory where your project.json is located) 

For Windows Users, precompiled binaries are available for your convienence [here](https://discord.foxbot.me/binaries/)

For Linux Users, you will need to compile from source. [Sodium Source Code](https://download.libsodium.org/libsodium/releases/), [Opus Source Code](http://downloads.xiph.org/releases/opus/).

## Joining a Channel

Joining Voice Channels is relatively straight-forward, and is a requirement for sending or receiving audio. This will also allow us to create an @Discord.Audio.IAudioClient, which will be used later to send or receive audio.

[!code-csharp[Joining a Channel](samples/joining_audio.cs)]

The client will sustain a connection to this channel until it is kicked, disconnected from Discord, or told to disconnect.