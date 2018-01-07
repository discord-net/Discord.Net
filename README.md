# Discord.Net
[![NuGet](https://img.shields.io/nuget/vpre/Discord.Net.svg?maxAge=2592000?style=plastic)](https://www.nuget.org/packages/Discord.Net)
[![MyGet](https://img.shields.io/myget/discord-net/vpre/Discord.Net.svg)](https://www.myget.org/feed/Packages/discord-net) 
[![Build status](https://ci.appveyor.com/api/projects/status/5sb7n8a09w9clute/branch/dev?svg=true)](https://ci.appveyor.com/project/RogueException/discord-net/branch/dev)
[![Discord](https://discordapp.com/api/guilds/81384788765712384/widget.png)](https://discord.gg/jkrBmQR)

An unofficial .NET API Wrapper for the Discord client (http://discordapp.com).

Check out the [documentation](https://discord.foxbot.me/docs/) or join the [Discord API Chat](https://discord.gg/jkrBmQR).

## Installation 
### Stable (NuGet)
Our stable builds available from NuGet through the Discord.Net metapackage:
- [Discord.Net](https://www.nuget.org/packages/Discord.Net/)

The individual components may also be installed from NuGet:
- [Discord.Net.Commands](https://www.nuget.org/packages/Discord.Net.Commands/)
- [Discord.Net.Rest](https://www.nuget.org/packages/Discord.Net.Rest/)
- [Discord.Net.Rpc](https://www.nuget.org/packages/Discord.Net.Rpc/)
- [Discord.Net.WebSocket](https://www.nuget.org/packages/Discord.Net.WebSocket/)
- [Discord.Net.Webhook](https://www.nuget.org/packages/Discord.Net.Webhook/)

The following provider is available for platforms not supporting .NET Standard 1.3:
- [Discord.Net.Providers.WS4Net](https://www.nuget.org/packages/Discord.Net.Providers.WS4Net/)

### Unstable (MyGet)
Nightly builds are available through our MyGet feed (`https://www.myget.org/F/discord-net/api/v3/index.json`).

## Compiling
In order to compile Discord.Net, you require the following:

### Using Visual Studio
- [Visual Studio 2017](https://www.microsoft.com/net/core#windowsvs2017)
- [.NET Core SDK](https://www.microsoft.com/net/download/core)

The .NET Core workload must be selected during Visual Studio installation.

### Using Command Line
- [.NET Core SDK](https://www.microsoft.com/net/download/core)

## Known Issues

### WebSockets (Win7 and earlier)
.NET Core 1.1 does not support WebSockets on Win7 and earlier. It's recommended to use the Discord.Net.Providers.WS4Net package until this is resolved.
Track the issue [here](https://github.com/dotnet/corefx/issues/9503).
