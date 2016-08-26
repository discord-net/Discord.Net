# Discord.Net v1.0.0-beta
[![MyGet](https://img.shields.io/myget/discord-net/vpre/Discord.Net.svg)](https://www.myget.org/feed/Packages/discord-net) 
[![MyGet Build Status](https://www.myget.org/BuildSource/Badge/discord-net?identifier=15bf7c42-22dd-4406-93e5-3cafc62bbc85)](https://www.myget.org/)
[![Discord](https://discordapp.com/api/guilds/81384788765712384/widget.png)](https://discord.gg/0SBTUU1wZTYLhAAW)

[![NuGet Pre Release](https://img.shields.io/nuget/vpre/Discord.Net.svg?maxAge=2592000?style=plastic)](https://www.nuget.org/packages/Discord.Net) [![AppVeyor](https://img.shields.io/appveyor/ci/foxbot/discord-net.svg?maxAge=2592000?style=plastic)](https://ci.appveyor.com/project/foxbot/discord-net/) [![Discord](https://discordapp.com/api/servers/81384788765712384/widget.png)](https://discord.gg/0SBTUU1wZTYLhAAW)

Discord.Net is an API wrapper for [Discord](http://discordapp.com) written in C#.

## Installation 
### Stable (NuGet)
Our stable builds are available from NuGet:
- [Discord.Net](https://www.nuget.org/packages/Discord.Net/)
- [Discord.Net.Commands](https://www.nuget.org/packages/Discord.Net.Commands/)

### Unstable (MyGet)
Bleeding edge builds are available using our MyGet feed (`https://www.myget.org/F/discord-net/api/v3/index.json`). These builds may break at any time - use with caution.

## Compiling
In order to compile Discord.Net, you require the following:

### Using Visual Studio 2015
- [VS2015 Update 3](https://www.microsoft.com/net/core#windows)
- [.Net Core 1.0 VS Plugin](https://www.microsoft.com/net/core#windows)

### Using CLI
- [.Net Core 1.0 SDK](https://www.microsoft.com/net/core)

## Known Issues

### WebSockets
The current stable .Net Core websocket package does not support Linux, or pre-Win8.

#### Linux
Add the latest version of `System.Net.WebSockets.Client` from the .Net Core MyGet feed (`https://dotnet.myget.org/F/dotnet-core/api/v3/index.json`) to your project.

#### Windows 7 and earlier
There is currently no workaround, track the issue [here](https://github.com/dotnet/corefx/issues/9503).
