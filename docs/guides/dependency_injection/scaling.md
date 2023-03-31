---
uid: Guides.DI.Scaling
title: Scaling your DI
---

# Scaling your DI

Dependency injection has a lot of use cases, and is very suitable for scaled applications.
There are a few ways to make registering & using services easier in large amounts.

## Using a range of services.

If you have a lot of services that all have the same use such as handling an event or serving a module,
you can register and inject them all at once by some requirements:

- All classes need to inherit a single interface or abstract type.
- While not required, it is preferred if the interface and types share a method to call on request.
- You need to register a class that all the types can be injected into.

### Registering implicitly

Registering all the types is done through getting all types in the assembly and checking if they inherit the target interface.

[!code-csharp[Registering](samples/implicit-registration.cs)]

> [!NOTE]
> As seen above, the interfaceType and activatorType are undefined. For our usecase below, these are `IService` and `ServiceActivator` in order.

### Using implicit dependencies

In order to use the implicit dependencies, you have to get access to the activator you registered earlier.

[!code-csharp[Accessing the activator](samples/access-activator.cs)]

When the activator is accessed and the `ActivateAsync()` method is called, the following code will be executed:

[!code-csharp[Executing the activator](samples/enumeration.cs)]

As a result of this, all the services that were registered with `IService` as its implementation type will execute their starting code, and start up.
