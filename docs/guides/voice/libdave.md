---
uid: Guides.Voice.LibDave
title: Using libdave e2ee
---

# Setup

You'll first need a build of Discords' [libdave](https://github.com/discord/libdave) library as either `libdave.dll`, `libdave.so` or `libdave.o` in your apps executing directory.

Then, enable dave voice encryption in your socket config:

[!code-csharp[Opt-in to libdave](samples/libdave_config.cs)]

That's it, `libdave` will be used when sending/receiving voice.

### libdave Log Sink

By default, `libdave` writes its logs to standard output, you can override that by specifying a custom log sink:

[!code-csharp[libdave sink](samples/libdave_sink.cs)]
