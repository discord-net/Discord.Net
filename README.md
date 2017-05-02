<p align="center"><img src="https://s25.postimg.org/5qp3onrj3/D.Net.png"/></p>

<p align="center">
<a href="https://www.nuget.org/packages/Discord.Net"><img src="https://img.shields.io/nuget/vpre/Discord.Net.svg?maxAge=2592000?style=plastic" alt="Nuget"/></a>
<a href="https://www.myget.org/feed/Packages/discord-net"> <img src="https://img.shields.io/myget/discord-net/vpre/Discord.Net.svg" alt="MyGet"/></a>
<a href="https://ci.appveyor.com/project/RogueException/discord-net/branch/dev"><img src="(https://ci.appveyor.com/api/projects/status/5sb7n8a09w9clute/branch/dev?svg=true"/></a>
<a href="https://discord.gg/0SBTUU1wZTVjAMPx"><img src="https://discordapp.com/api/guilds/81384788765712384/widget.png" alt="Discord"/></a>
</p>

Discord.Net is an unofficial .NET API Wrapper for the [Discord client](http://discordapp.com). Please read the documentation on how to setup the bot and what has been changed in 1.0.

1.0 Documentation: https://discord.foxbot.me/docs/index.html
0.9.6 Documentation: http://rtd.discord.foxbot.me/en/legacy/

Example Bots:

[Foxbot Example C# Bot 1.0](https://github.com/420foxbot/DiscordExampleBot)
[Foxbot Example VB.Net Bot 1.0](https://github.com/420foxbot/DiscordExampleBot.VB)
[Aux Example C# Bot 1.0](https://github.com/Aux/Dogey)
[Aux Example C# Bot 0.9.6](https://github.com/Aux/Discord.Net-Example/tree/0.9)


## Installation
---
Discord.Net current stable version is 0.9.6 and can be obtained from [Nuget Packages](https://www.nuget.org/packages/Discord.Net/).
You can also install other individual components from Nuget or Nuget Package Manager.
- [Discord.Net.Rest](https://www.nuget.org/packages/Discord.Net.Rest/)
- [Discord.Net.Rpc](https://www.nuget.org/packages/Discord.Net.Rpc/)
- [Discord.Net.WebSocket](https://www.nuget.org/packages/Discord.Net.WebSocket/)
- [Discord.Net.Webhook](https://www.nuget.org/packages/Discord.Net.Webhook/)
- [Discord.Net.Commands](https://www.nuget.org/packages/Discord.Net.Commands/)

The following providers are available for platforms not supporting .NET Standard 1.3:
- [Discord.Net.Providers.UdpClient](https://www.nuget.org/packages/Discord.Net.Providers.UdpClient/)
- [Discord.Net.Providers.WS4Net](https://www.nuget.org/packages/Discord.Net.Providers.WS4Net/)

## Unstable/Beta/Latest (MyGet)
---
You can get the Beta build from MyGet. The Beta build version is 1.0.xxx and contains every new feature from discord such as Embeds.
Nightly builds are available through [MyGet feed](https://www.myget.org/F/discord-net/api/v3/index.json).

## Compiling
---
In order to compile Discord.Net you will need latest version of [Visual Studio 2017](https://www.microsoft.com/net/core#windowsvs2017) and [.Net Core SDK](https://www.microsoft.com/net/download/core). The .Net Core workload must be selected during Visual Studio Installation.

## Known Issues
---
### WebSockets (Win7 and earlier)
.NET Core 1.1 does not support WebSockets on Win7 and earlier. It's recommended to use the Discord.Net.Providers.WS4Net package until this is resolved.
Track the issue [here](https://github.com/dotnet/corefx/issues/9503).

## Example Ping Command
---
```cs
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
public class Program
{
    static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

    public async Task Start()
    {
        var client = new DiscordSocketClient();
        client.MessageReceived += MessageReceived;
        await client.LoginAsync(TokenType.Bot, "YOUR-TOKEN-GOES-HERE");
        await client.ConnectAsync();
        await Task.Delay(-1);
    }
    
    private async Task MessageReceived(SocketMessage message)
    {
      if (message.Content == "!ping"){
        await message.Channel.SendMessageAsync("Pong!");
        }
    }
}
  
```
