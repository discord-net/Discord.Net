---
uid: Discord.Commands.CommandException
remarks: *content
---

This @System.Exception class is typically used when diagnosing
an error thrown during the execution of a command. You will find the
thrown exception passed into
[LogMessage.Exception](xref:Discord.LogMessage.Exception), which is
sent to your [CommandService.Log](xref:Discord.Commands.CommandService.Log)
event handler.

---
uid: Discord.Commands.CommandException
example: [*content]
---

You may use this information to handle runtime exceptions after
execution. Below is an example of how you may use this:

```cs
public Task LogHandlerAsync(LogMessage logMessage)
{
    // Note that this casting method requires C#7 and up.
    if (logMessage?.Exception is CommandException cmdEx)
    {
        Console.WriteLine($"{cmdEx.GetBaseException().GetType()} was thrown while executing {cmdEx.Command.Aliases.First()} in {cmdEx.Context.Channel} by {cmdEx.Context.User}.");
    }
    return Task.CompletedTask;
}
```