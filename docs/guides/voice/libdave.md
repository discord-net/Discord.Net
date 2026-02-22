---
uid: Guides.Voice.LibDave
title: Using libdave e2ee
---

# Setup

You'll need a build of Discord's [libdave](https://github.com/discord/libdave) library as either `libdave.dll`, `libdave.so` or `libdave.o` in your apps executing directory or `PATH`. The library will resolve `libdave` and start using it automatically.

## Force enable/disable

You can opt-in/opt-out of libdave with the following config option:

[!code-csharp[Opt-in to libdave](samples/libdave_config.cs)]

### libdave Log Sink

By default, `libdave` writes its logs to standard output, you can override that by specifying a custom log sink:

[!code-csharp[libdave sink](samples/libdave_sink.cs)]