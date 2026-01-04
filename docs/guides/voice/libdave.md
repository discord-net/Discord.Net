---
uid: Guides.Voice.LibDave
title: Using libdave e2ee
---

# Setup

You'll first need a build of Discords' [libdave](https://github.com/discord/libdave) library as either `libdave.dll`, `libdave.so` or `libdave.o` in your apps executing directory.

Then, enable dave voice encryption in your socket config:

```cs
var client = new DiscordSocketClient(new DiscordSocketConfig()
{
    EnableVoiceDaveEncryption = true,
    ...
});
```

That's it, `libdave` will be used when sending/receiving voice.

### libdave Log Sink

By default, `libdave` writes its logs to standard output, you can override that by specifying a custom log sink:
```cs
Discord.LibDave.Dave.SetLogSink(MyLogSink);

void MyLogSink(
    Discord.LibDave.LoggingSeverity severity, 
    string filePath,
    int lineNumber,
    string message
)
{
    // do what you want with the logs
    Console.WriteLine($"[{severity} | LIBDAVE @ {file}#{line}]: {message}");
}
```
