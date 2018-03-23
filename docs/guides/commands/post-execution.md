# Preface

When developing a command system or modules, you may want to consider
building a post-execution handling system so you can have a finer 
control over commands. Discord.NET offers several different 
post-execution workflow for you to work with.

If you recall, in the [Command Guide], we've shown the following 
example for executing and handling commands,

[!code[Command Handler](samples/command_handler.cs)]

You may notice that after we perform [ExecuteAsync], we store the 
result and print it to the chat. This is essentially the most
basic post-execution handling. With this in mind, we could start doing
things like the following,

[!code[Basic Command Handler](samples/post-execution_basic.cs)]

**But!** This may not always be preferred, because you are 
creating your  post-execution logic *with* the essential command 
handler. This could lead to messy code and has another potential 
issue, working with `RunMode.Async`.

If your command is marked with `RunMode.Async`, [ExecuteAsync] will 
return a successful [ExecuteResult] instead of whatever results 
the actual command may return. Because of the way `RunMode.Async` 
[works](../../faq/commands.md), handling within the command handler 
may not always achieve the desired effect.

## CommandExecuted Event

Enter [CommandExecuted], an event that was introduced in 
Discord.NET 2.0. This event is raised **when the command is 
sucessfully executed** and is not prone to `RunMode.Async`'s 
[ExecuteAsync] drawbacks.

[CommandExecuted]: xref:Discord.Commands.CommandService.CommandExecuted
[ExecuteAsync]: xref:Discord.Commands.CommandService.ExecuteAsync*
[ExecuteResult]: xref:Discord.Commands.ExecuteResult
[Command Guide]: Commands.md