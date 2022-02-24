---
uid: Guides.OtherLibs.Serilog
title: Configuring Serilog
---

# Configuring serilog

## Prerequisites

- A basic working bot with a logging method as described in [Creating your first bot]: xref:Guides.GettingStarted.FirstBot#creating-a-discord-client

## Installing the Serilog package

You can install the following packages through your IDE or go to the nuget link to grab the dotnet cli command.

|Name|Link|
|--|--|
|`Serilog.Extensions.Logging`| [link](https://www.nuget.org/packages/Serilog.Extensions.Logging)|
|`Serilog.Sinks.Console`| [link](https://www.nuget.org/packages/Serilog.Sinks.Console)|

## Configuring Serilog

Serilog will be configured at the top of your async Main method, it looks like this

```cs
public class Program
{
  public static Task Main(string[] args) => new Program().MainAsync();

  public async Task MainAsync()
  {
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger();

    _client = new DiscordSocketClient();

    _client.Log += LogAsync;

    //  You can assign your bot token to a string, and pass that in to connect.
    //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
    var token = "token";

    // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
    // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
    // var token = File.ReadAllText("token.txt");
    // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

    await _client.LoginAsync(TokenType.Bot, token);
    await _client.StartAsync();

    // Block this task until the program is closed.
    await Task.Delay(-1);
}
}
```

## Modifying your logging method

For serilog to log discord events correctly, we have to map the discord `LogSeverity` to the serilog `LogEventLevel`. You can modify your log method to look like this.

```cs
private static Task LogAsync(LogMessage message)
{
    var severity = message.Severity switch
    {
        LogSeverity.Critical => LogEventLevel.Fatal,
        LogSeverity.Error => LogEventLevel.Error,
        LogSeverity.Warning => LogEventLevel.Warning,
        LogSeverity.Info => LogEventLevel.Information,
        LogSeverity.Verbose => LogEventLevel.Verbose,
        LogSeverity.Debug => LogEventLevel.Debug,
        _ => LogEventLevel.Information
    };
    Log.Write(severity, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
    return Task.CompletedTask;
}
```

## Testing

If you run your application now, you should see something similar to this
![Serilog output](images/serilog_output.png)

## Using your new logger in other places

Now that you have set up Serilog, you can use it everywhere in your application by simply calling

```cs
Log.Debug("Your log message, with {Variables}!", 10) // This will output "[21:51:00 DBG] Your log message, with 10!"
```

> [!NOTE]
> Depending on your configured log level, the log messages may or may not show up in your console. Refer to [Serilog's github page](https://github.com/serilog/serilog/wiki/Configuration-Basics#minimum-level) for more information about log levels.
