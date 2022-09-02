---
uid: Guides.DI.Services
title: Using DI in Interaction & Command Frameworks
---

# DI in the Interaction- & Command Service

For both the Interaction- and Command Service modules, DI is quite straight-forward to use.

You can inject any service into modules without the modules having to be registered to the provider.
Discord.Net resolves your dependencies internally.

> [!WARNING]
> The way DI is used in the Interaction- & Command Service are nearly identical, except for one detail:
> [Resolving Module Dependencies](xref:Guides.IntFw.Intro#resolving-module-dependencies)

## Registering the Service

Thanks to earlier described behavior of allowing already registered members as parameters of the available ctors,
The socket client & configuration will automatically be acknowledged and the XService(client, config) overload will be used.

[!code-csharp[Service Registration](samples/service-registration.cs)]

## Usage in modules

In the constructor of your module, any parameters will be filled in by
the @System.IServiceProvider that you've passed.

Any publicly settable properties will also be filled in the same
manner.

[!code-csharp[Module Injection](samples/modules.cs)]

If you accept `Command/InteractionService` or `IServiceProvider` as a parameter in your constructor or as an injectable property,
these entries will be filled by the `Command/InteractionService` that the module is loaded from and the `IServiceProvider` that is passed into it respectively.

> [!NOTE]
> Annotating a property with a [DontInjectAttribute] attribute will
> prevent the property from being injected.

## Services

Because modules are transient of nature and will reinstantiate on every request,
it is suggested to create a singleton service behind it to hold values across multiple command executions.

[!code-csharp[Services](samples/services.cs)]


