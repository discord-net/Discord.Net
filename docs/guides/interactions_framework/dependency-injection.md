---
uid: Guides.InteractionsFramework.DependencyInjection
title: Dependency Injection
---

# Dependency Injection

Interaction Service uses dependency injection to perform most of its operations. This way, you can access service dependencies throughout the framework.

## Setup

1. Create a `Microsoft.Extensions.DependencyInjection.ServiceCollection`.
2. Add the dependencies you wish to use in the modules.
3. Build a `IServiceProvider` using the `BuildServiceProvider()` method of the `ServiceCollection`.
4. Pass the `IServiceProvider` to `AddModulesAsync()`, `AddModuleAsync()` and `ExecuteAsync()` methods.

## Accessing the Dependencies

Services of a `IServiceProvider` can be accessed using *Contructor Injection* and *Property Injection*.

Interaction Service will populate the constructor parameters using the provided `IServiceProvider`. Any public settable class Property will also be populated in the same manner.

## Service Scopes

Interaction Service has built-in support for scoped service types. Scoped lifetime services are instantiated once per command execution. Including the Preconditon checks, every module operation is executed within a single service scope (which is sepearate from the global service scope).

> For more in-depth information about service lifetimes check out [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0#service-lifetimes-1).
