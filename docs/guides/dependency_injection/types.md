---
uid: Guides.DI.Dependencies
title: Types of Dependencies
---

# Dependency Types

There are 3 types of dependencies to learn to use. Several different usecases apply for each.

> [!WARNING]
> When registering types with a serviceType & implementationType,
> only the serviceType will be available for injection, and the implementationType will be used for the underlying instance.

## Singleton

A singleton service creates a single instance when first requested, and maintains that instance across the lifetime of the application.
Any values that are changed within a singleton will be changed across all instances that depend on it, as they all have the same reference to it.

### Registration:

[!code-csharp[Singleton Example](samples/singleton.cs)]

> [!NOTE]
> Types like the Discord client and Interaction/Command services are intended to be singleton,
> as they should last across the entire app and share their state with all references to the object.

## Scoped

A scoped service creates a new instance every time a new service is requested, but is kept across the 'scope'.
As long as the service is in view for the created scope, the same instance is used for all references to the type.
This means that you can reuse the same instance during execution, and keep the services' state for as long as the request is active.

### Registration:

[!code-csharp[Scoped Example](samples/scoped.cs)]

> [!NOTE]
> Without using HTTP or libraries like EFCORE, scopes are often unused in Discord bots.
> They are most commonly used for handling HTTP and database requests.

## Transient

A transient service is created every time it is requested, and does not share its state between references within the target service.
It is intended for lightweight types that require little state, to be disposed quickly after execution.

### Registration:

[!code-csharp[Transient Example](samples/transient.cs)]

> [!NOTE]
> Discord.Net modules behave exactly as transient types, and are intended to only last as long as the command execution takes.
> This is why it is suggested for apps to use singleton services to keep track of cross-execution data.
