---
uid: Guides.InteractionsFramework.PostEx
title: Post-Execution
---

# Post-Execution Logic

Interaction Service uses `IResult`s to provide information about the state of command execution. These can be used to log internal exceptions or provide some insight to the command user.

If you are running your commands using `RunMode.Sync` these command results can be retrieved from the return value of `InteractionService.ExecuteCommandAsync()` method or by registering delegates to Interaction Service events.

If you are using the `RunMode.Async` to run your commands, you must use the Interaction Service events to get the execution results. When using `RunMode.Async`, `InteractionService.ExecuteCommandAsync()` will always return a successful result.

## Results

Interaction Result come in a handful of different flavours:

1. `AutocompletionResult`: returned by Autocompleters
2. `ExecuteResult`: contains the result of method body execution process
3. `PreconditionGroupResult`: returned by Precondition groups
4. `PreconditionResult`: returned by preconditions
5. `RuntimeResult`: a user implementable result for returning user defined results
6. `SearchResult`: returned by command lookup map
7. `TypeConverterResult`: returned by TypeConverters

You can either use the `IResult.Error` property of an Interaction result or create type check for the afformentioned result types to branch out your post-execution logic to handle different situations.

## CommandExecuted Events

Every time a command gets executed, Interaction Service raises a *CommandExecuted event. These events can be used to create a post-execution pipeline.

```csharp
interactionService.SlashCommandExecuted += SlashCommandExecuted;

async Task SlashCommandExecuted (SlashCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        await arg2.Interaction.RespondAsync($"Unmet Precondition: {arg3.ErrorReason}");
                        break;
                    case InteractionCommandError.UnknownCommand:
                        await arg2.Interaction.RespondAsync("Unknown command");
                        break;
                    case InteractionCommandError.BadArgs:
                        await arg2.Interaction.RespondAsync("Invalid number or arguments");
                        break;
                    case InteractionCommandError.Exception:
                        await arg2.Interaction.RespondAsync($"Command exception:{arg3.ErrorReason}");
                        break;
                    case InteractionCommandError.Unsuccessful:
                        await arg2.Interaction.RespondAsync("Command could not be executed");
                        break;
                    default:
                        break;
                }
            }
        }
```

## Log Event

InteractionService regularly outputs information about the occuring events to keep the developer informed.

## Runtime Result

Interaction commands allow you to return `Task<RuntimeResult>` to pass on additional information about the command execution process back to your post-execution logic.

Custom `RuntimeResult` classes can be created by inheriting the base `RuntimeResult` class.

If command execution process reaches the method body of the command and no exceptions are thrown during the execution of the method body, `RuntimeResult` returned by your command will be accessible by casting/type-checking the `IResult` parameter of the *CommandExecuted event delegate.
