# Discord.Net v1.0.0-rc
[![MyGet](https://img.shields.io/myget/discord-net/vpre/Discord.Net.svg)](https://www.myget.org/feed/Packages/discord-net) 
[![MyGet Build Status](https://www.myget.org/BuildSource/Badge/discord-net?identifier=15bf7c42-22dd-4406-93e5-3cafc62bbc85)](https://www.myget.org/)
[![Discord](https://discordapp.com/api/guilds/81384788765712384/widget.png)](https://discord.gg/0SBTUU1wZTVjAMPx)

An unofficial .Net API Wrapper for the Discord client (http://discordapp.com).

Check out the [documentation](https://discord.foxbot.me/docs/) or join the [Discord API Chat](https://discord.gg/0SBTUU1wZTVjAMPx).

## Installation 
### Stable (NuGet)
Our stable builds available from NuGet through the Discord.Net metapackage:
- [Discord.Net](https://www.nuget.org/packages/Discord.Net/)

The individual components may also be installed from NuGet:
- [Discord.Net.Rest](https://www.nuget.org/packages/Discord.Net.Rest/)
- [Discord.Net.Rpc](https://www.nuget.org/packages/Discord.Net.Rpc/)
- [Discord.Net.WebSocket](https://www.nuget.org/packages/Discord.Net.WebSocket/)
- [Discord.Net.Commands](https://www.nuget.org/packages/Discord.Net.Commands/)

The following providers are available for platforms not supporting .NET Standard 1.3:
- [Discord.Net.Providers.UdpClient](https://www.nuget.org/packages/Discord.Net.Providers.UdpClient/)
- [Discord.Net.Providers.WS4Net](https://www.nuget.org/packages/Discord.Net.Providers.WS4Net/)

### Unstable (MyGet)
Nightly builds are available through our MyGet feed (`https://www.myget.org/F/discord-net/api/v3/index.json`).

## Compiling
In order to compile Discord.Net, you require the following:

### Using Visual Studio
- [Visual Studio 2017 RC Build 26014.0](https://www.microsoft.com/net/core#windowsvs2017)

The .NET Core and Docker (Preview) workload is required during Visual Studio installation.

### Using Command Line
- [.Net Core 1.1 SDK](https://www.microsoft.com/net/download/core)

## Known Issues

### WebSockets (Win7 and earlier)
.Net Core 1.1 does not support WebSockets on Win7 and earlier. It's recommended to use the Discord.Net.Providers.WS4Net package until this is resolved.
Track the issue [here](https://github.com/dotnet/corefx/issues/9503).
