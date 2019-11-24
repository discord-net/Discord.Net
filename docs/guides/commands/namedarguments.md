---
uid: Guides.Commands.NamedArguments
title: Named Arguments
---

# Named Arguments

By default, arguments for commands are parsed positionally, meaning
that the order matters. But sometimes you may want to define a command
with many optional parameters, and it'd be easier for developers
to only specify what they want to set, instead of needing them
to specify everything by hand.

## Setting up Named Arguments

In order to be able to specify different arguments by name, you have
to create a new class that contains all of the optional values that
the command will use, and apply an instance of
[NamedArgumentTypeAttribute] on it.

### Example - Creating a Named Arguments Type

```cs
[NamedArgumentType]
public class NamableArguments
{
    public string First { get; set; }
    public string Second { get; set; }
    public string Third { get; set; }
    public string Fourth { get; set; }
}
```

## Usage in a Command

The command where you want to use these values can be declared like so:
```cs
[Command("act")]
public async Task Act(int requiredArg, NamableArguments namedArgs)
```

The command can now be invoked as
`.act 42 first: Hello fourth: "A string with spaces must be wrapped in quotes" second: World`.

A TypeReader for the named arguments container type is
automatically registered.
It's important that any other arguments that would be required
are placed before the container type.

> [!IMPORTANT]
> A single command can have only __one__ parameter of a
> type annotated with [NamedArgumentTypeAttribute], and it
> **MUST** be the last parameter in the list.
> A command parameter of such an annotated type
> is automatically treated as if that parameter
> has [RemainderAttribute](xref:Discord.Commands.RemainderAttribute)
> applied.

## Complex Types

The TypeReader for Named Argument Types will look for a TypeReader
of every property type, meaning any other command parameter type
will work just the same.

You can also read multiple values into a single property
by making that property an `IEnumerable<T>`. So for example, if your
Named Argument Type has the following field,
```cs
public IEnumerable<int> Numbers { get; set; }
```
then the command can be invoked as
`.cmd numbers: "1, 2, 4, 8, 16, 32"`

## Additional Notes

The use of [`[OverrideTypeReader]`](xref:Discord.Commands.OverrideTypeReaderAttribute)
is also supported on the properties of a Named Argument Type.

[NamedArgumentTypeAttribute]: xref:Discord.Commands.NamedArgumentTypeAttribute
