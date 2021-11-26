---
uid: FAQ.Commands.DI
title: Questions about Dependency Injection with Commands
---

# Dependency-injection-related Questions

In the following section, you will find common questions and answers
to utilizing dependency injection with @Discord.Commands, as well as
common troubleshooting steps regarding DI.

## What is a service? Why does my module not hold any data after execution?

In Discord.Net, modules are created similarly to ASP.NET, meaning
that they have a transient nature; modules are spawned whenever a
request is received, and are killed from memory when the execution
finishes. In other words, you cannot store persistent
data inside a module. Consider using a service if you wish to
workaround this.

Service is often used to hold data externally so that they persist
throughout execution. Think of it like a chest that holds
whatever you throw at it that won't be affected by anything unless
you want it to. Note that you should also learn Microsoft's
implementation of [Dependency Injection] \([video]) before proceeding,
as well as how it works in [Discord.Net](xref:Guides.Commands.DI#usage-in-modules).

A brief example of service and dependency injection can be seen below.

[!code-csharp[DI](samples/DI.cs)]

[Dependency Injection]: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
[video]: https://www.youtube.com/watch?v=QtDTfn8YxXg

## Why is my `CommandService` complaining about a missing dependency?

If you encounter an error similar to `Failed to create MyModule,
dependency MyExternalDependency was not found.`, you may have
forgotten to add the external dependency to the dependency container.

Starting from Discord.Net 2.0, all dependencies required by each
module must be present when the module is loaded into the
[CommandService]. This means when loading the module, you must pass a
valid [IServiceProvider] with the dependency loaded before the module
can be successfully registered.

For example, if your module, `MyModule`, requests a `DatabaseService`
in its constructor, the `DatabaseService` must be present in the
[IServiceProvider] when registering `MyModule`.

[!code-csharp[Missing Dependencies](samples/missing-dep.cs)]

[IServiceProvider]: xref:System.IServiceProvider
[CommandService]: xref:Discord.Commands.CommandService
