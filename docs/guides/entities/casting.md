---
uid: Guides.Entities.Casting
title: Casting & Unboxing
---

# Casting

Casting can be done in many ways, and is the only method to box and unbox types to/from their base definition.
Casting only works for types that inherit the base type that you want to unbox from.
`IUser` cannot be cast to `IMessage`.

> [!NOTE]
> Interfaces **can** be cast to other interfaces, as long as they inherit each other.
> The same goes for reverse casting. As long as some entity can be simplified into what it inherits, your cast will pass.

## Boxing

A boxed object is the definition of an object that was simplified (or trimmed) by incoming traffic,
but still owns the data of the originally constructed type. Boxing is an implicit operation.

Through casting, we can **unbox** this type, and access the properties that were inaccessible before.

## Unboxing

Unboxing is the most direct way to access the real definition of an object.
If we want to return a type from its interface, we can unbox it directly.

[!code-csharp[Unboxing](samples/unboxing.cs)]

## Regular casting

In 'regular' casting, we use the `as` keyword to assign the given type to the object.
If the boxed type can indeed be cast into given type,
it will become said type, and its properties can be accessed.
[!code-csharp[Casting](samples/casting.cs)]

> [!WARNING]
> If the type you're casting to is null, a `NullReferenceException` will be thrown when it's called.
> This makes safety casting much more interesting to use, as it prevents this exception from being thrown.

## Safety casting

Safety casting makes sure that the type you're trying to cast to can never be null, as it passes checks upon calling them.

There are 3 different ways to safety cast an object:

### Basic safety casting:

To safety cast an object, all we need to do is check if it is of the member type in a statement.
If this check fails, it will continue below, making sure we don't try to access null.
[!code-csharp[Base](samples/safety-cast.cs)]

### Object declaration:

Here we declare the object we are casting to,
making it so that you can immediately work with its properties without reassigning through regular casting.
[!code-csharp[Declare](samples/safety-cast-var.cs)]

### Reverse passage:

In previous examples, we want to let code continue running after the check, or if the check fails.
In this example, the cast will return the entire method (ignoring the latter) upon failure,
and declare the variable for further use into the method:
[!code-csharp[Pass](samples/safety-cast-pass.cs)]

> [!NOTE]
> Usage of `is`, `not` and `as` is required in cast assignment and/or type checks. `==`, `!=` and `=` are invalid assignment,
> as these operators only apply to initialized objects and not their types.
