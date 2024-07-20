---
uid: Guides.TextCommands.Preconditions
title: Preconditions
---

# Preconditions

Preconditions serve as a permissions system for your Commands. Keep in
mind, however, that they are not limited to _just_ permissions and can
be as complex as you want them to be.

There are two types of Preconditions you can use:

* [PreconditionAttribute] can be applied to Modules, Groups, or Commands.
* [ParameterPreconditionAttribute] can be applied to Parameters.

You may visit their respective API documentation to find out more.

[PreconditionAttribute]: xref:Discord.Commands.PreconditionAttribute
[ParameterPreconditionAttribute]: xref:Discord.Commands.ParameterPreconditionAttribute

## Bundled Preconditions

@Discord.Commands ships with several bundled Preconditions for you
to use.

* @Discord.Commands.RequireContextAttribute
* @Discord.Commands.RequireOwnerAttribute
* @Discord.Commands.RequireBotPermissionAttribute
* @Discord.Commands.RequireUserPermissionAttribute
* @Discord.Commands.RequireNsfwAttribute
* @Discord.Commands.DoHierarchyCheckAttribute

## Using Preconditions

To use a precondition, simply apply any valid precondition candidate to
a command method signature as an attribute.

### Example - Using a Precondition

[!code-csharp[Precondition usage](samples/preconditions/precondition_usage.cs)]

## ORing Preconditions

When writing commands, you may want to allow some of them to be
executed when only some of the precondition checks are passed.

This is where the [Group] property of a precondition attribute comes in
handy. By assigning two or more preconditions to a group, the command
system will allow the command to be executed when one of the
precondition passes.

### Example - ORing Preconditions

[!code-csharp[OR Precondition](samples/preconditions/group_precondition.cs)]

[Group]: xref:Discord.Commands.PreconditionAttribute.Group

## Custom Preconditions

To write your own Precondition, create a new class that inherits from
either [PreconditionAttribute] or [ParameterPreconditionAttribute]
depending on your use.

In order for your Precondition to function, you will need to override
the [CheckPermissionsAsync] method.

If the context meets the required parameters, return
[PreconditionResult.FromSuccess], otherwise return
[PreconditionResult.FromError] and include an error message if
necessary.

> [!NOTE]
> Visual Studio can help you implement missing members
> from the abstract class by using the "Implement Abstract Class"
> IntelliSense hint.

### Example - Creating a Custom Precondition

[!code-csharp[Custom Precondition](samples/preconditions/require_role.cs)]

[CheckPermissionsAsync]: xref:Discord.Commands.PreconditionAttribute.CheckPermissionsAsync*
[PreconditionResult.FromSuccess]: xref:Discord.Commands.PreconditionResult.FromSuccess*
[PreconditionResult.FromError]: xref:Discord.Commands.PreconditionResult.FromError*
