# Voice

**Information on this page is subject to change!**

>[!WARNING]
>Audio in 1.0 is incomplete. Most of the below documentation is untested.

## Installation

To use Audio, you must first configure your [DiscordSocketClient] 
with Audio support.

In your [DiscordSocketConfig], set `AudioMode` to the appropriate 
[AudioMode] for your bot. For most bots, you will only need to use 
`AudioMode.Outgoing`.

[DiscordSocketClient]: xref:Discord.WebSocket.DiscordSocketClient
[DiscordSocketConfig]: xref:Discord.WebSocket.DiscordSocketConfig
[AudioMode]: xref:Discord.Audio.AudioMode

### Dependencies

Audio requires two native libraries, `libsodium` and `opus`. 
Both of these libraries must be placed in the runtime directory of your 
bot. (When developing on .NET Framework, this would be `bin/debug`, 
when developing on .NET Core, this is where you execute `dotnet run` 
from; typically the same directory as your csproj).

For Windows Users, precompiled binaries are available for your 
convienence [here](https://discord.foxbot.me/binaries/)

For Linux Users, you will need to compile [Sodium] and [Opus] from 
source, or install them from your package manager.

[Sodium]: https://download.libsodium.org/libsodium/releases/
[Opus]: http://downloads.xiph.org/releases/opus/

## Joining a Channel

Joining a channel is the first step to sending audio, and will return 
an [IAudioClient] to send data with.

To join a channel, simply await [ConnectAsync] on any instance of an
@Discord.IVoiceChannel.

[!code-csharp[Joining a Channel](samples/joining_audio.cs)]

The client will sustain a connection to this channel until it is 
kicked, disconnected from Discord, or told to disconnect.

It should be noted that voice connections are created on a per-guild 
basis; only one audio connection may be open by the bot in a single 
guild. To switch channels within a guild, invoke [ConnectAsync] on 
another voice channel in the guild.

[IAudioClient]: xref:Discord.Audio.IAudioClient
[ConnectAsync]: xref:Discord.IVoiceChannel#Discord_IVoiceChannel_ConnectAsync

## Transmitting Audio

### With FFmpeg

[FFmpeg] is an open source, highly versatile AV-muxing tool. This is 
the recommended method of transmitting audio.

Before you begin, you will need to have a version of FFmpeg downloaded 
and placed somewhere in your PATH (or alongside the bot, in the same 
location as libsodium and opus). Windows binaries are available on 
[FFmpeg's download page].

[FFmpeg]: https://ffmpeg.org/
[FFmpeg's download page]: https://ffmpeg.org/download.html

First, you will need to create a Process that starts FFmpeg. An 
example of how to do this is included below, though it is important 
that you return PCM at 48000hz.

>[!NOTE]
>As of the time of this writing, Discord.Audio struggles significantly 
>with processing audio that is already opus-encoded; you will need to 
>use the PCM write streams.

[!code-csharp[Creating FFmpeg](samples/audio_create_ffmpeg.cs)]

Next, to transmit audio from FFmpeg to Discord, you will need to 
pull an [AudioOutStream] from your [IAudioClient]. Since we're using 
PCM audio, use [IAudioClient.CreatePCMStream].

The sample rate argument doesn't particularly matter, so long as it is 
a valid rate (120, 240, 480, 960, 1920, or 2880). For the sake of 
simplicity, I recommend using 1920.

Channels should be left at `2`, unless you specified a different value 
for `-ac 2` when creating FFmpeg.

[AudioOutStream]: xref:Discord.Audio.AudioOutStream
[IAudioClient.CreatePCMStream]: xref:Discord.Audio.IAudioClient#Discord_Audio_IAudioClient_CreatePCMStream_System_Int32_System_Int32_System_Nullable_System_Int32__System_Int32_

Finally, audio will need to be piped from FFmpeg's stdout into your 
AudioOutStream. This step can be as complex as you'd like it to be, but 
for the majority of cases, you can just use [Stream.CopyToAsync], as 
shown below.

[Stream.CopyToAsync]: https://msdn.microsoft.com/en-us/library/hh159084(v=vs.110).aspx

If you are implementing a queue for sending songs, it's likely that 
you will want to wait for audio to stop playing before continuing on 
to the next song. You can await `AudioOutStream.FlushAsync` to wait for 
the audio client's internal buffer to clear out.

[!code-csharp[Sending Audio](samples/audio_ffmpeg.cs)]