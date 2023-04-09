---
uid: Guides.DI.Injection
title: Injection
---

# Injecting instances within the provider

You can inject registered services into any class that is registered to the `IServiceProvider`.
This can be done through property or constructor.

> [!NOTE]
> As mentioned above, the dependency *and* the target class have to be registered in order for the serviceprovider to resolve it.

## Injecting through a constructor

Services can be injected from the constructor of the class.
This is the preferred approach, because it automatically locks the readonly field in place with the provided service and isn't accessible outside of the class.

[!code-csharp[Constructor Injection](samples/ctor-injecting.cs)]

## Injecting through properties

Injecting through properties is also allowed as follows.

[!code-csharp[Property Injection](samples/property-injecting.cs)]

> [!WARNING]
> Dependency Injection will not resolve missing services in property injection, and it will not pick a constructor instead.
> If a publicly accessible property is attempted to be injected and its service is missing, the application will throw an error.

## Using the provider itself

You can also access the provider reference itself from injecting it into a class. There are multiple use cases for this:

- Allowing libraries (Like Discord.Net) to access your provider internally.
- Injecting optional dependencies.
- Calling methods on the provider itself if necessary, this is often done for creating scopes.

[!code-csharp[Provider Injection](samples/provider.cs)]

> [!NOTE]
> It is important to keep in mind that the provider will pick the 'biggest' available constructor.
> If you choose to introduce multiple constructors,
> keep in mind that services missing from one constructor may have the provider pick another one that *is* available instead of throwing an exception.
