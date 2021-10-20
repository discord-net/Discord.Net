# Frequently Asked Questions

#### Whats the difference between DNet and DNet Labs?
DNet Labs is mostly the same as DNet as it just adds additional features. Discord.NET-labs adds several features Discord.NET does not. Examples of this are: Threads, application commands, message components, stage channels, role icons & more small functionality changes. More details [here](https://github.com/discord-net/Discord.Net/pull/1923).

#### Should I use DNet Labs instead of DNet? or can I use both at the same time?
DNet Labs implements new experimental/unstable features and generally shouldn't be used in production environments. You should only use it if you want to test out new Discord features. As DNet Labs builds on top of DNet, you can not use both at the same time, however DNet Labs should be a pretty straight forward drop-in replacement for DNet.

#### Why is x not doing y when it should?
Double check you are on the most recent version of Discord .Net labs, if you are and it looks like an issue, report it on [Discord](https://discord.com/invite/dnet-labs) or create an issue on [GitHub](https://github.com/Discord-Net-Labs/Discord.Net-Labs)

#### When is feature x going to be added?
When someone adds it. If a new Discord feature is currently untouched, submit an issue on [GitHub](https://github.com/Discord-Net-Labs/Discord.Net-Labs) regarding the request.

#### What's the difference between RespondAsync, DeferAsync and FollowupAsync?
The difference between these 3 functions is in how you handle the command response. [RespondAsync](https://discord-net-labs.com/api/Discord.WebSocket.SocketCommandBase.html#Discord_WebSocket_SocketCommandBase_DeferAsync_System_Boolean_Discord_RequestOptions_) and [DeferAsync](https://discord-net-labs.com/api/Discord.WebSocket.SocketCommandBase.html#Discord_WebSocket_SocketCommandBase_DeferAsync_System_Boolean_Discord_RequestOptions_) let the API know you have succesfully received the command. This is also called 'acknowledging' a command. DeferAsync will not send out a response, RespondAsync will not. [FollowupAsync](https://discord-net-labs.com/api/Discord.WebSocket.SocketCommandBase.html#Discord_WebSocket_SocketCommandBase_DeferAsync_System_Boolean_Discord_RequestOptions_) follows up on succesful acknowledgement. 

> [!WARNING]
> If you have not acknowledged the command FollowupAsync will not work.

#### What's the difference between global commands and guild commands? (Why isn't my command show up in discord?)
Global commands can be used in every guild your bot is in. However, it can take up to an hour for every guild to have access to the commands.
Guild commands can only be used in specific guilds. They are available within a few minutes. (This is great for testing purposes).

#### I'm getting a 50001: Missing access error while trying to create a guild command!
Guild commands require you give grant the bot the applications.commands OAuth2 scope in order to register guild commands in that guild. You can register global commands without this OAuth2 scope but will need it to use global commands in that guild.

#### I'm getting errors when trying to create a slash command (The application command failed to be created, 400 bad request, 50035: Invalid Form Body, etc)
This could be caused by several things but the most common one is an invalid "Name" for the command or any of the options/arguments for the command. Make sure your "Name" is all lowercase and only contains letters or dashes. It should also be less than 32 characters. If you are still having issues after checking this, read up on the other slash commands limits [here](https://discord.com/developers/docs/interactions/slash-commands#a-quick-note-on-limits).

> [!NOTE]
> In most cases, you can catch an [ApplicationCommandException](https://discord-net-labs.com/api/Discord.Net.ApplicationCommandException.html?q=applicationcommandexception) error from creation. 
> This exception will tell you what part of your command is invalid as well as why. 

#### How can I use Victoria with Labs?
You can add a special build of victoria that supports labs by adding the below to your references:
```xml
<PropertyGroup>
  <RestoreAdditionalProjectSources>https://www.myget.org/F/yucked/api/v3/index.json</RestoreAdditionalProjectSources>
</PropertyGroup>
```

#### I'm getting a 10062 exception: Unknown Interaction. What can I do?
This exception happens when an app tries to send an initial response to an interaction twice. This can be caused by:
- 2 instances of your app running at once.
- Responding 3+ seconds after the interaction was received.
If you're positive that your app doesn't do this and you are still receiving the exception, please submit an issue. 

#### Why is BannerId/AccentColour always null? 
Currently SocketUser/SocketGuildUser does not expose the [BannerId](https://discord-net-labs.com/api/Discord.IUser.html#Discord_IUser_BannerId) nor [AccentColor](https://discord-net-labs.com/api/Discord.IUser.html#Discord_IUser_AccentColor). To get this info, a [RestUser](https://discord-net-labs.com/api/Discord.Rest.RestUser.html?q=RestUser) must be requested. 
