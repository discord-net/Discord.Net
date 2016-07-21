# Using the Logger

Discord.Net will automatically output log messages through the [Log](xref:Discord.DiscordClient#Discord_DiscordClient_Log) event.

## Usage

To handle Log Messages through Discord.Net's Logger, hook into the [Log](xref:Discord.DiscordClient#Discord_DiscordClient_Log) event.

The @Discord.LogMessage object has a custom `ToString` method attached to it, when outputting log messages, it is reccomended you use this, instead of building your own output message.

[!code-csharp[](samples/logging.cs)]