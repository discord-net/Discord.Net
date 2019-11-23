---
uid: Guides.Commands.NamedArguments
title: Named Arguments
---

# Named Arguments

By default, arguments for commands are parsed positionally, meaning
that the order matters. But sometimes you may want to define a command
with many optional parameters and it'd be easier for users to specify
only what they want to set instead of needing them to specify everything.

## Setting up Named Arguments

In order to be able to specify different arguments by name, you have
to create a new class that contains all of the optional values that
the command will use and applying an instance of
[`NamedArgumentTypeAttribute`](xref:Discord.Commands.NamedArgumentTypeAttribute) on it.

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

## Use in a command

The command where you want to use these values can be declared like so:
```cs
[Command("act")]
public async Task Act(int requiredArg, NamableArguments namedArgs)
```

A TypeReader for the named arguments container type is
automatically registered.
It's important that any other arguments that would be required
are placed before the container type.

> [!IMPORTANT]
> A single command can have only __one__ parameter of a
> type annotated with `[NamedArgumentType]` and it
> **MUST** be the last parameter in the list.
> A command parameter of such an annotated type
> is automatically treated as if that parameter
> has [`[Remainder]`](xref:Discord.Commands.RemainderAttribute)

The command can now be invoked as follows:
`.act 42 first: Hello fourth: "A string with spaces must be wrapped in quotes" second: World`

## Complex types

The TypeReader for Named Argument Types will look for a TypeReader
of every property type, meaning any other command parameter type
will work just the same.

You can also read multiple values into a single property
by making that property an `IEnumerable<T>`. So if your
Named Argument Type has, for example:
```cs
public IEnumerable<int> Numbers { get; set; }
```
the command can be invoked as:
`.cmd numbers: "1, 2, 4, 8, 16, 32"`

## Additional

The use of [`[OverrideTypeReader]`](xref:Discord.Commands.OverrideTypeReaderAttribute)
is also supported on the properties of a Named Argument Type.
