# Command-related Questions

## How can I restrict some of my commands so only certain users can execute them?

   Based on how you want to implement the restrictions, you can use the built-in `RequireUserPermission` precondition, which allows you to restrict the command based on the user's current permissions in the guild or channel (*e.g. `GuildPermission.Administrator`, `ChannelPermission.ManageMessages` etc.*). 
   If, however, you wish to restrict the commands based on the user's role, you can eithe create your own custom precondition or use Joe4evr's [Preconditions Addons](https://github.com/Joe4evr/Discord.Addons/tree/master/src/Discord.Addons.Preconditions) that provides a few custom preconditions that aren't provided in the stock library. Its source can also be used as an example for creating your own custom preconditions.

  
## I'm getting an error about `Assembly#GetEntryAssembly`. What now?

   You may be confusing `CommandService#AddModulesAsync` with `CommandService#AddModuleAsync`. The former is used to add modules via the assembly, while the latter is used to add a single module.


## What does [Remainder] do in the command signature?

   The `RemainderAttribute` leaves the string unparsed, meaning you don't have to add quotes around the text for the text to be recognized as a single object. Please note that if your method has multiple parameters, the remainder attribute can only be applied to the last parameter.
   ```cs
   // !echo repeat this message in chat
   [Command("echo")]
   [Summary("Replies whatever the user adds")]
   [Remarks("The entire message is considered one String")]
   public Task EchoAsync([Remainder]String text) => ReplyAsync(text);  
   
   // !echo repeat this message in chat
   [Command("echo")]
   [Summary("Replies whatever the user adds")]
   [Remarks("This command will error for having too many arguments.  
   The message would be seen as having 5 parameters while the method only accepts one.  
   Wrapping the message in quotes solves this - '!echo repeat this message in chat' -  
   this way, the system knows the entire message is to be parsed as a single String")]
   public Task EchoAsync(String text) => ReplyAsync(text);
   ```  
   
## What is a service? Why does my module not hold any data after execution?

   In Discord.NET, modules are created similarly to ASP.NET, meaning that they have a transient nature. This means that they are spawned every time when a request is received, and are killed from memory when the execution finishes. This is why you cannot store persistent data inside a module. To workaround this, consider using a service. Service is often used to hold data externally, so that they will persist throughout execution. Think of it like a chest that holds whatever you throw at it that won't be affected by anything unless you want it to. Note that you should also learn Microsoft's implementation of Dependency Injection before proceeding. You can learn more about it [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection), and how it works in Discord.NET [here](https://discord.foxbot.me/latest/guides/commands/commands.html#usage-in-modules). A brief example of service and dependency injection can be seen below,
   
```cs
public class MyService
{
    public string MyCoolString {get; set;}
}
public class SetupOrWhatever
{
    public IServiceProvider BuildProvider() => new ServiceCollection().AddSingleton<MyService>().BuildServiceProvider();
}
public class MyModule : ModuleBase<SocketCommandContext>
{
    // Inject via public settable prop
    public MyService MyService {get; set;}
    // or via ctor
    private readonly MyService _myService;
    public MyModule (MyService myService) => _myService = myService;
    [Command("setorprintstring")]
    public Task GetOrSetStringAsync()
    {
        if (_myService.MyCoolString == null) _myService.MyCoolString = "ya boi";
        return ReplyAsync(_myService.MyCoolString);
    }
}
```
 
## I have a long-running Task in my command, and Discord.NET keeps saying that a `MessageReceived` handler is blocking the gateway. What gives?

  By default, all commands are executed on the same thread as the gateway task, which is responsible for keeping the connection from your client to Discord alive. When you execute a long-running task, this blocks the gateway from communicating for as long as the command task is being executed. The library will warn you about any long running event handler (in this case, the command handler) that persists for more than 3 seconds. 
  
  To resolve this, the library has designed a flag called `RunMode`. There are 2 main `RunMode`s. One being `RunMode.Sync`, which is the default; another being `RunMode.Async`. `RunMode.Async` essentially calls an unawaited Task and continues with the execution without waiting for the command task to finish. You should use `RunMode.Async` in either the `CommandAttribute` or the `DefaultRunMode` flag in `CommandServiceConfig`. Further details regarding `RunMode.Async` can be found below.
  
## Okay, that's great and all, but how does `RunMode.Async` work, and if it's so great, why is the lib *not* using it by default?

  As with any async operation, `RunMode.Async` also comes at a cost. The following are the caveats with RunMode.Async,
  1) You introduce race condition.
  2) Unnecessary overhead caused by async state machine (learn more about it [here](https://www.red-gate.com/simple-talk/dotnet/net-tools/c-async-what-is-it-and-how-does-it-work/)).
  3) `CommandService#ExecuteAsync` will immediately return `ExecuteResult` instead of other result types (this is particularly important for those who wish to utilize `RuntimeResult` in 2.0).
  4) Exceptions are swallowed.
  
  However, there are ways to remedy #3 and #4.
  
  For #3, in Discord.NET 2.0, the library introduces a new event called `CommandExecuted`, which is raised whenever the command is finished. This event will be called regardless of the `RunMode` type and will return the appropriate execution result
  
  For #4, exceptions are caught in `CommandService#Log` under `(CommandException)LogMessage.Exception`.