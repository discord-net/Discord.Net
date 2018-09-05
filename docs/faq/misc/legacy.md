---
uid: FAQ.Legacy
title: Questions about Legacy Versions
---

# Legacy Questions

This section refers to legacy library-related questions that do not
apply to the latest or recent version of the Discord.Net library.

## X, Y, Z does not work! It doesn't return a valid value anymore.

If you are currently using an older version of the stable branch,
please upgrade to the latest pre-release version to ensure maximum
compatibility. Several features may be broken in older
versions and will likely not be fixed in the version branch due to
their breaking nature.

Visit the repo's [release tag] to see the latest public pre-release.

[release tag]: https://github.com/RogueException/Discord.Net/releases

## I came from an earlier version of Discord.Net 1.0, and DependencyMap doesn't seem to exist anymore in the later revision? What happened to it?

The `DependencyMap` has been replaced with Microsoft's
[DependencyInjection] Abstractions. An example usage can be seen
[here](https://github.com/foxbot/DiscordBotBase/blob/csharp/src/DiscordBot/Program.cs#L36).

[DependencyInjection]: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection