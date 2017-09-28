---
title: Working with Events
---

Events in Discord.Net are consumed in a similar manner to the standard
convention, with the exception that every event must be of the type
`System.Threading.Tasks.Task` and instead of using `EventArgs`, the
event's parameters are passed directly into the handler.

This allows for events to be handled in an async context directly
instead of relying on `async void`.

### Usage

To receive data from an event, hook into it using C#'s delegate
event pattern.

You may either opt to hook an event to an anonymous function (lambda)
or a named function.

### Safety

All events are designed to be thread-safe; events are executed
synchronously off the gateway task in the same context as the gateway
task.

As a side effect, this makes it possible to deadlock the gateway task
and kill a connection. As a general rule of thumb, any task that takes
longer than three seconds should **not** be awaited directly in the
context of an event, but should be wrapped in a `Task.Run` or
offloaded to another task.

This also means that you should not await a task that requests data
from Discord's gateway in the same context of an event. Since the
gateway will wait on all invoked event handlers to finish before
processing any additional data from the gateway, this will create
a deadlock that will be impossible to recover from.

Exceptions in commands will be swallowed by the gateway and logged out
through the client's log method.

### Common Patterns

As you may know, events in Discord.Net are only given a signature of
`Func<T1, ..., Task>`. There is no room for predefined argument names,
so you must either consult IntelliSense, or view the API documentation
directly.

That being said, there are a variety of common patterns that allow you
to infer what the parameters in an event mean.

#### Entity, Entity

An event handler with a signature of `Func<Entity, Entity, Task>`
typically means that the first object will be a clone of the entity
_before_ a change was made, and the latter object will be an attached
model of the entity _after_ the change was made.

This pattern is typically only found on `EntityUpdated` events.

#### Cacheable

An event handler with a signature of `Func<Cacheable, Entity, Task>`
means that the `before`	state of the entity was not provided by the
API, so it can either be pulled from the client's cache or
downloaded from the API.

See the documentation for [Cacheable] for more information on this
object.

[Cacheable]: xref:Discord.Cacheable`2

### Samples

[!code-csharp[Event Sample](samples/events.cs)]

### Tips

Many events relating to a Message entity (i.e. `MessageUpdated` and 
`ReactionAdded`) rely on the client's message cache, which is
**not** enabled by default. Set the `MessageCacheSize` flag in
[DiscordSocketConfig] to enable it.

[DiscordSocketConfig]: xref:Discord.WebSocket.DiscordSocketConfig