---
uid: Guides.IntFw.PostExecution
title: Post-Command execution
---

# Post-Execution Logic

Interaction Service uses [IResult] to provide information about the state of command execution.
These can be used to log internal exceptions or provide some insight to the command user.

If you are running your commands using `RunMode.Sync` these command results can be retrieved from
the return value of [InteractionService.ExecuteCommandAsync] method or by
registering delegates to Interaction Service events.

If you are using the `RunMode.Async` to run your commands,
you must use the Interaction Service events to get the execution results. When using `RunMode.Async`,
[InteractionService.ExecuteCommandAsync] will always return a successful result.

[InteractionService.ExecuteCommandAsync]: xref:Discord.Interactions.InteractionService.ExecuteCommandAsync*

## Results

Interaction Result come in a handful of different flavours:

1. [AutocompletionResult]: returned by Autocompleters
2. [ExecuteResult]: contains the result of method body execution process
3. [PreconditionGroupResult]: returned by Precondition groups
4. [PreconditionResult]: returned by preconditions
5. [RuntimeResult]: a user implementable result for returning user defined results
6. [SearchResult]: returned by command lookup map
7. [TypeConverterResult]: returned by TypeConverters

> [!NOTE]
> You can either use the [IResult.Error] property of an Interaction result or create type check for the
> aforementioned result types to branch out your post-execution logic to handle different situations.


[AutocompletionResult]: xref:Discord.AutocompleteResult
[ExecuteResult]: xref:Discord.Interactions.ExecuteResult
[PreconditionGroupResult]: xref:Discord.Interactions.PreconditionGroupResult
[PreconditionResult]: xref:Discord.Interactions.PreconditionResult
[SearchResult]: xref:Discord.Interactions.SearchResult`1
[TypeConverterResult]: xref:Discord.Interactions.TypeConverterResult
[IResult.Error]: xref:Discord.Interactions.IResult.Error*

## CommandExecuted Events

Every time a command gets executed, Interaction Service raises a `CommandExecuted` event.
These events can be used to create a post-execution pipeline.

[!code-csharp[Error Review](samples/postexecution/error_review.cs)]

## Log Event

InteractionService regularly outputs information about the occuring events to keep the developer informed.

## Runtime Result

Interaction commands allow you to return `Task<RuntimeResult>` to pass on additional information about the command execution
process back to your post-execution logic.

Custom [RuntimeResult] classes can be created by inheriting the base [RuntimeResult] class.

If command execution process reaches the method body of the command and no exceptions are thrown during
the execution of the method body, [RuntimeResult] returned by your command will be accessible by casting/type-checking the
[IResult] parameter of the `CommandExecuted` event delegate.

[RuntimeResult]: xref:Discord.Interactions.RuntimeResult
[IResult]: xref:Discord.Interactions.IResult
