# Discord.Net v1.0.0-dev
[![MyGet](https://img.shields.io/myget/discord-net/vpre/Discord.Net.svg?maxAge=2592000)](https://www.myget.org/feed/Packages/discord-net) [![AppVeyor](https://img.shields.io/appveyor/ci/foxbot/discord-net.svg?maxAge=2592000?style=plastic)](https://ci.appveyor.com/project/foxbot/discord-net/) [![Discord](https://discordapp.com/api/guilds/81384788765712384/widget.png)](https://discord.gg/0SBTUU1wZTYLhAAW)

An unofficial .Net API Wrapper for the Discord client (http://discordapp.com).

Check out the [documentation](http://rtd.discord.foxbot.me/en/docs-dev/index.html) or join the [Discord API Chat](https://discord.gg/0SBTUU1wZTVjAMPx).

### Installation 
#### Stable (NuGet)
Our stable builds are available from NuGet:
- [Discord.Net](https://www.nuget.org/packages/Discord.Net/)
- [Discord.Net.Commands](https://www.nuget.org/packages/Discord.Net.Commands/)

#### Unstable (MyGet)
Bleeding edge builds are available using our `https://www.myget.org/F/discord-net/api/v3/index.json` feed. These builds may break at any time - use with caution.

### Compiling
In order to compile Discord.Net, you require the following:

#### Using Visual Studio 2015
- [VS2015 Update 3](https://www.microsoft.com/net/core#windows)
- [.Net Core 1.0 VS Plugin](https://www.microsoft.com/net/core#windows)

#### Using CLI
- [.Net Core 1.0 SDK](https://www.microsoft.com/net/core)

### Known Issues

#### WebSockets
The current stable .Net Core websocket package does not support Linux, or pre-Win8.

##### Linux
Get the latest version of `System.Net.WebSockets.Client` from the .Net Core MyGet feed: `https://dotnet.myget.org/F/dotnet-core/api/v3/index.json`.

##### Windows 7 and earlier
There is currently no workaround, track the issue [here](https://github.com/dotnet/corefx/issues/9503).
