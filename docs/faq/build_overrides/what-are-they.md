---
uid: FAQ.BuildOverrides.WhatAreThey
title: Build Overrides
---

# Build Overrides

Build overrides are a way for library developers to override the default behavior of the library on the fly. Adding them to your code is really simple.

## Installing the package

The build override package can be installed on nuget [here](https://www.nuget.org/packages/Discord.Net.BuildOverrides) or by using the package manager

```
PM> Install-Package Discord.Net.BuildOverrides
```

## Adding an override

```cs
public async Task MainAsync()
{
  // hook into the log function
  BuildOverrides.Log += (buildOverride, message) =>
  {
    Console.WriteLine($"{buildOverride.Name}: {message}");
    return Task.CompletedTask;
  };

  // add your overrides
  await BuildOverrides.AddOverrideAsync("example-override-name");
}

```

Overrides are normally built for specific problems, for example if someone is having an issue and we think we might have a fix then we can create a build override for them to test out the fix.

## Security and Transparency

Overrides can only be created and updated by library developers, you should only apply an override if a library developer asks you to.
Code for the overrides server and the overrides themselves can be found [here](https://github.com/discord-net/Discord.Net.BuildOverrides).
