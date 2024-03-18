---
uid: FAQ.Legacy
title: Legacy Questions
---

# Legacy Questions

This section refers to legacy library-related questions that do not
apply to the latest or recent version of the Discord.Net library.

## Migrating your commands to application commands.

The new interaction service was designed to act like the previous service for text-based commands.
Your pre-existing code will continue to work, but you will need to migrate your modules and response functions to use the new
interaction service methods. Documentation on this can be found in the [Guides](xref:Guides.IntFw.Intro).

## Gateway event parameters changed, why?

With 3.0, a higher focus on [Cacheable]'s was introduced.
[Cacheable]'s get an entity from cache, rather than making an API call to retrieve it's data.
The entity can be retrieved from cache by calling `GetOrDownloadAsync()` on the [Cacheable] type.

> [!NOTE]
> GetOrDownloadAsync will download the entity if its not available directly from the cache.

[Cacheable]: xref:Discord.Cacheable`2

## X, Y, Z does not work! It doesn't return a valid value anymore.

If you are currently using an older version of the stable branch,
please upgrade to the latest release version to ensure maximum
compatibility. Several features may be broken in older
versions and will likely not be fixed in the version branch due to
their breaking nature.

Visit the repo's [release tag] to see the latest public release.

[release tag]: https://github.com/discord-net/Discord.Net/releases

## I came from an earlier version of Discord.Net 1.0, and DependencyMap doesn't seem to exist anymore in the later revision? What happened to it?

The `DependencyMap` has been replaced with Microsoft's
[DependencyInjection] Abstractions. An example usage can be seen
[here](https://github.com/Discord-Net-Labs/Discord.Net-Labs/blob/release/3.x/samples/InteractionFramework/Program.cs#L66).

[DependencyInjection]: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
