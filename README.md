<p align="center">
  <a href="https://discord-net-labs.com/" title="Click to visit the documentation!">
    <img src="https://discord-net-labs.com/marketing/Logo/SVG/Combinationmark%20White%20Border.svg" alt="Logo">
  </a>
    <br />
    <br />
  <a href="https://www.nuget.org/packages/Discord.Net.Labs/">
    <img src="https://img.shields.io/nuget/vpre/Discord.Net.Labs.svg?maxAge=2592000?style=plastic" alt="NuGet">
  </a>
  <a href="https://www.myget.org/feed/Packages/discord-net-labs">
    <img src="https://img.shields.io/myget/discord-net-labs/vpre/Discord.Net.Labs.svg" alt="MyGet">
  </a>
  <a href="https://dev.azure.com/Discord-Net-Labs/Discord-Net-Labs/_build/latest?definitionId=1&amp;branchName=release%2F3.x">
    <img src="https://dev.azure.com/Discord-Net-Labs/Discord-Net-Labs/_apis/build/status/Discord-Net-Labs.Discord.Net-Labs?branchName=release%2F3.x" alt="Build Status">
  </a>
  <a href="https://discord.gg/dvSfUTet3K">
    <img src="https://discord.com/api/guilds/848176216011046962/widget.png" alt="Discord">
  </a>
</p>

This repo is a custom fork of Discord.Net that introduces the newest features of discord for testing and experimenting. Nothing here is guaranteed to work but you are more than welcome to submit bugs in the issues tabs

----

- 📄 [Documentation](https://discord-net-labs.com)
- 🔗 [Support](https://discord.com/invite/dnet-labs)
- 📚 [Guides](https://discord-net-labs.com/guides/introduction/intro.html)

## Sponsor us! ❤
- If this library benefits you consider [sponsoring](https://github.com/sponsors/quinchs) the project as it really helps out. *Only sponsor if you're financially stable!*

## Known compatibility issues

- Playwo's [InteractivityAddon](https://www.nuget.org/packages/Discord.InteractivityAddon)
  * ❌ Reason: The default package depends on Discord.NET instead of labs.
  * ✔ Fix: [InteractivityAddon.Labs](https://www.nuget.org/packages/Discord.InteractivityAddon.Labs), which implements some of the features added in Discord.Net-Labs.

- [Victoria](https://github.com/Yucked/Victoria)
  * ❌ Reason: Victoria is built around Discord.NET and is not supported by labs.
  * ✔ Fix: A custom build based on Discord.NET-labs: 
```xml
<PropertyGroup>
  <RestoreAdditionalProjectSources>https://www.myget.org/F/yucked/api/v3/index.json</RestoreAdditionalProjectSources>
</PropertyGroup>
```

## How to use
Setting up labs in your project is really simple, here's how to do it:
1) Remove Discord.Net from your project
2) Add Discord.Net Labs nuget to your project
3) That's all!

## Branches

### Dev
This branch is kept up to date with dnets dev branch. we pull of it to ensure that labs will work with pre existing dnet code.

### release/3.x
This branch is what will be pushed to nuget, sometimes its not up to date as we wait for other features to be finished.

### feature/xyz
These branches are features for new things, you are more than welcome to clone them and give feedback in the discord server or issues tab.
