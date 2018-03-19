# Legacy Questions
   
## X, Y, Z does not work! It doesn't return a valid value anymore.
  If you're currently using 1.0.0, please upgrade to the latest 2.0 beta to ensure maximum compatibility. Several methods or props may be broken in 1.0.x and will not be fixed in the 1.0 branch due to their breaking nature. 
  Notable breaking changes are as follows,
  * `IChannel#IsNsfw` has been replaced with `ITextChannel#IsNsfw` and now returns valid value in 2.0.
  * Bulk message removal (`DeletedMessagesAsync`) has been moved from `IMessageChannel` to `ITextChannel`.
  * `IAsyncEnumerable#Flatten` has been renamed to `FlattenAsync`.
   
## I came from an earlier version of Discord.NET 1.0, and DependencyMap doesn't seem to exist anymore in the later revision? What happened to it?
   The `DependencyMap` has been replaced with Microsoft's [DependencyInjection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection) Abstractions. An example usage can be seen [here](https://github.com/foxbot/DiscordBotBase/blob/csharp/src/DiscordBot/Program.cs#L36).