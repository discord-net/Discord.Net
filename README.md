# Discord.Net v1.0.0-beta2
[![MyGet](https://img.shields.io/myget/discord-net/vpre/Discord.Net.svg)](https://www.myget.org/feed/Packages/discord-net) 
[![MyGet Build Status](https://www.myget.org/BuildSource/Badge/discord-net?identifier=15bf7c42-22dd-4406-93e5-3cafc62bbc85)](https://www.myget.org/)
[![Discord](https://discordapp.com/api/guilds/81384788765712384/widget.png)](https://discord.gg/0SBTUU1wZTYLhAAW)

An unofficial .Net API Wrapper for the Discord client (http://discordapp.com).

Check out the [documentation](https://discord.foxbot.me/docs/) or join the [Discord API Chat](https://discord.gg/0SBTUU1wZTVjAMPx).

## Installation 
### Stable (NuGet)
Our stable builds are available from NuGet:
- [Discord.Net](https://www.nuget.org/packages/Discord.Net/)
- [Discord.Net.Commands](https://www.nuget.org/packages/Discord.Net.Commands/)

### Unstable (MyGet)
Bleeding edge builds are available using our MyGet feed (`https://www.myget.org/F/discord-net/api/v3/index.json`). These builds may break at any time - use with caution.

## Compiling
In order to compile Discord.Net, you require the following:

### Using Visual Studio
- [Visual Studio 2017 RC](https://www.microsoft.com/net/core#windowsvs2017)

The .NET Core and Docker (Preview) workload is required during Visual Studio installation.

### Using Command Line
- [.Net Core 1.1 SDK](https://www.microsoft.com/net/download/core)

## Known Issues

### WebSockets (Win7 and earlier)
The .Net Core 1.1 does not support WebSockets on Win7 and earlier. Track the issue [here](https://github.com/dotnet/corefx/issues/9503).
