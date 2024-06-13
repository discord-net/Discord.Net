<p align="center">
  <a href="https://discordnet.dev/" title="Click to visit the documentation!">
    <img src="https://raw.githubusercontent.com/discord-net/Discord.Net/dev/docs/marketing/logo/SVG/Combinationmark%20White%20Border.svg" alt="Logo">
  </a>
    <br />
    <br />
  <a href="https://www.nuget.org/packages/Discord.Net/">
    <img src="https://img.shields.io/nuget/vpre/Discord.Net.svg?maxAge=2592000?style=plastic" alt="NuGet">
  </a>
  <a href="https://www.myget.org/feed/Packages/discord-net">
    <img src="https://img.shields.io/myget/discord-net/vpre/Discord.Net.svg" alt="MyGet">
  </a>
  <a href="https://github.com/discord-net/Discord.Net/actions/workflows/docs.yml">
    <img src="https://github.com/discord-net/Discord.Net/actions/workflows/docs.yml/badge.svg" alt="Docs Build Status">
  </a>
  <a href="https://github.com/discord-net/Discord.Net/actions/workflows/dotnet.yml">
    <img src="https://github.com/discord-net/Discord.Net/actions/workflows/dotnet.yml/badge.svg" alt="Dotnet Build Status">
  </a>
  <a href="https://discord.gg/dnet">
    <img src="https://img.shields.io/discord/848176216011046962?logo=discord&logoColor=white&label=discord&color=%235865F2" alt="Discord">
  </a>
</p>
Discord.Net is an unofficial .NET API Wrapper for the Discord API (https://discord.com).

## 📄 Documentation

- https://discordnet.dev

## 🩷 Supporting Discord.Net

Discord.Net is an MIT-licensed open source project with its development made possible entirely by volunteers. 
If you'd like to support our efforts financially, please consider:

- [Contributing on Open Collective](https://opencollective.com/discordnet).

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

### Patch component

An increment of the **PATCH** component always indicates that an internal-only change was made, generally a bugfix. These changes will not affect the public-facing API in any way, and are always guaranteed to be forward- and backwards-compatible with your codebase, any pre-compiled dependencies of your codebase.

### Minor component

An increment of the **MINOR** component indicates that some addition was made to the library,
and this addition is not backwards-compatible with prior versions.
However, Discord.Net **does not guarantee forward-compatibility** on minor additions.
In other words, we permit a limited set of breaking changes on a minor version bump.

Due to the nature of the Discord API, we will oftentimes need to add a property to an entity to support the latest API changes.
Discord.Net provides interfaces as a method of consuming entities; and as such, introducing a new field to an entity is technically a breaking change.
Major version bumps generally indicate some major change to the library,
and as such we are hesitant to bump the major version for every minor addition to the library.
To compromise, we have decided that interfaces should be treated as **consumable only**,
and your applications should typically not be implementing interfaces.

> For applications where interfaces are implemented, such as in test mocks, we apologize for this inconsistency with SemVer.

While we will never break the API (outside of interface changes) on minor builds,
we will occasionally need to break the ABI, by introducing parameters to a method to match changes upstream with Discord.
As such, a minor version increment may require you to recompile your code, and dependencies,
such as addons, may also need to be recompiled and republished on the newer version.
When a binary breaking change is made, the change will be noted in the release notes.

### Major component

An increment of the **MAJOR** component indicates that breaking changes have been made to the library;
consumers should check the release notes to determine what changes need to be made.

## 📚 Branches

### Release/X.X

Release branch following Major.Minor. Upon release, patches will be pushed to these branches.
New NuGet releases will be tagged on these branches.

### Dev

Development branch, available on MyGet. This branch is what pull requests are targetted to.

### Feature/X

Branches that target Dev, adding new features. Feel free to explore these branches and give feedback where necessary.

### Docs/X

Usually targets Dev. These branches are used to update documentation with either new features or existing feature rework.
