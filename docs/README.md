# Discord.Net #
[![Discord](https://discordapp.com/api/guilds/81384788765712384/widget.png)](https://discord.gg/discord-api)

An unofficial .NET library for the [Discord API](https://discordapp.com/).

## Installation ##

Our stable builds available from NuGet through the Discord.Net metapackage:
[Discord.Net](https://www.nuget.org/packages/Discord.Net/)

Unstable "nightly" builds are available through our MyGet feed:
`https://www.myget.org/F/discord-net/api/v3/index.json`


## Building ##

At the minimum, .NET Core 2.1 is required to successfully build Discord.Net,
but .NET Core 3.1 is recommended. You should install the appropriate tools and
components for your editor to open the Discord.Net solution file:

- The .NET Core workload for Visual Studio
- The C# build tools for Visual Studio Code
- The .NET Core SDK for command line

### Versioning ###

As a rule, Discord.Net attempts to follow
[Semantic Versioning](https://semver.org). This means `MAJOR.MINOR.PATCH`
versioning.

However, as Discord evolves and adds or changes existing endpoints, we may need
to add code to support these changes. To do this, we increment the minor version
when adding new API surfaces to the library. To counter any inevitable breaking
changes, we have decided that our public interfaces should be for
*consumption only* - bot developers should not need to implement these
interfaces for normal code.

Additionally, as the Discord API changes, we may need to add, remove, or change
parameters to certain methods. To ensure this breaks as little code as
possible, we use the following approach to deprecate old APIs and redirect
users to new APIs, while retaining binary compatibility:

```cs
// OLD API
[Obsolete("This API has been superseded by GetUserAsync.", false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public ValueTask<IUser> GetUserAsync(ulong userId)
  => GetUserAsync(userId, "default");

// NEW API
public ValueTask<IUser> GetUserAsync(ulong userId,
  string additionalParameter = "default");
```

These obsolete methods will be removed in the next major version, and will be
clearly documented in our change logs.
