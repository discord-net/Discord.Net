# Discord.Net

**Discord.Net** is an unofficial .NET API Wrapper for the Discord API (https://discord.com/developers/docs/intro).

## 📄 Documentation

- https://discordnet.dev
- [Sample Projects](https://github.com/discord-net/Discord.Net/tree/dev/samples) in our repository

## Feedback

You report issues with the library by opening an issue in our [GitHub Repository](https://github.com/discord-net/Discord.Net)

If you need help with using Discord.Net check out the following:
- ask on our [Discord Support Server](https://discord.gg/dnet) 
- start a thread in [GitHub Discussions](https://github.com/discord-net/Discord.Net/discussions)

## 📥 Installation

### Stable (NuGet)

Our stable builds available from NuGet through the Discord.Net metapackage:

- [Discord.Net](https://www.nuget.org/packages/Discord.Net/)

The individual components may also be installed from NuGet:

- _Webhooks_
  - [Discord.Net.Webhook](https://www.nuget.org/packages/Discord.Net.Webhook/)

- _Text-Command & Interaction services._
  - [Discord.Net.Commands](https://www.nuget.org/packages/Discord.Net.Commands/)
  - [Discord.Net.Interactions](https://www.nuget.org/packages/Discord.Net.Interactions/)

- _Complete API coverage._
  - [Discord.Net.WebSocket](https://www.nuget.org/packages/Discord.Net.WebSocket/)
  - [Discord.Net.Rest](https://www.nuget.org/packages/Discord.Net.Rest/)

- _The API core. Implements only entities and barebones functionality._
  - [Discord.Net.Core](https://www.nuget.org/packages/Discord.Net.Core/)

### Nightlies

Nightlies are builds of Discord.NET that are still in an experimental phase, and have not been released.  
They are available through 2 different sources:
- [BaGet](https://baget.discordnet.dev/)
- [GitHub Packages](https://github.com/orgs/discord-net/packages?repo_name=Discord.Net)

> [!NOTE]
> GitHub Packages requires authentication. You can find more information [here](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry#authenticating-to-github-packages).

## 🩷 Supporting Discord.Net

Discord.Net is an MIT-licensed open source project with its development made possible entirely by volunteers. 
If you'd like to support our efforts financially, please consider:

- [Contributing on Open Collective](https://opencollective.com/discordnet).

## 🛑 Known Issues

### WebSockets (Win7 and earlier)

.NET Core 1.1 does not support WebSockets on Win7 and earlier.
This issue has been fixed since the release of .NET Core 2.1.
It is recommended to target .NET Core 2.1 or above for your project if you wish to run your bot on legacy platforms;
alternatively, you may choose to install the
[Discord.Net.Providers.WS4Net](https://www.nuget.org/packages/Discord.Net.Providers.WS4Net/) package.

### TLS on .NET Framework.

Discord supports only TLS1.2+ on all their websites including the API since 07/19/2022.
.NET Framework does not support this protocol by default.
If you depend on .NET Framework, it is suggested to upgrade your project to `net6-windows`.
This framework supports most of the windows-only features introduced by fx, and resolves startup errors from the TLS protocol mismatch.

## 🗃️ Versioning Guarantees

This library generally abides by [Semantic Versioning](https://semver.org). Packages are published in `MAJOR.MINOR.PATCH` version format.
