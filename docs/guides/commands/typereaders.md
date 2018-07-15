---
uid: Guides.Commands.TypeReaders
title: Type Readers
---

# Type Readers

Type Readers allow you to parse different types of arguments in
your commands.

By default, the following Types are supported arguments:

* `bool`
* `char`
* `sbyte`/`byte`
* `ushort`/`short`
* `uint`/`int`
* `ulong`/`long`
* `float`, `double`, `decimal`
* `string`
* `enum`
* `DateTime`/`DateTimeOffset`/`TimeSpan`
* Any nullable value-type (e.g. `int?`, `bool?`)
* Any implementation of `IChannel`/`IMessage`/`IUser`/`IRole`

## Creating a Type Reader

To create a `TypeReader`, create a new class that imports @Discord and
@Discord.Commands and ensure the class inherits from
@Discord.Commands.TypeReader. Next, satisfy the `TypeReader` class by
overriding the [ReadAsync] method.

Inside this Task, add whatever logic you need to parse the input
string.

If you are able to successfully parse the input, return
[TypeReaderResult.FromSuccess] with the parsed input, otherwise return
[TypeReaderResult.FromError] and include an error message if
necessary.

> [!NOTE]
> Visual Studio can help you implement missing members
> from the abstract class by using the "Implement Abstract Class"
> IntelliSense hint.

[TypeReaderResult]: xref:Discord.Commands.TypeReaderResult
[TypeReaderResult.FromSuccess]: xref:Discord.Commands.TypeReaderResult.FromSuccess*
[TypeReaderResult.FromError]: xref:Discord.Commands.TypeReaderResult.FromError*
[ReadAsync]: xref:Discord.Commands.TypeReader.ReadAsync*

### Example - Creating a Type Reader

[!code-csharp[TypeReaders](samples/typereaders/typereader.cs)]

## Registering a Type Reader

TypeReaders are not automatically discovered by the Command Service
and must be explicitly added.

To register a TypeReader, invoke [CommandService.AddTypeReader].

> [!IMPORTANT]
> TypeReaders must be added prior to module discovery, otherwise your
> TypeReaders may not work!

[CommandService.AddTypeReader]: xref:Discord.Commands.CommandService.AddTypeReader*

### Example - Adding a Type Reader

[!code-csharp[Adding TypeReaders](samples/typereaders/typereader-register.cs)]