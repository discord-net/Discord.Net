---
uid: Guides.Commands.Preconditions
title: Preconditions
---

# Preconditions

Precondition serve as a permissions system for your Commands. Keep in
mind, however, that they are not limited to _just_ permissions and can
be as complex as you want them to be.

There are two types of Preconditions you can use:

* [PreconditionAttribute] can be applied to Modules, Groups, or Commands.
* [ParameterPreconditionAttribute] can be applied to Parameters.

You may visit their respective API documentation to find out more.

[PreconditionAttribute]: xref:Discord.Commands.PreconditionAttribute
[ParameterPreconditionAttribute]: xref:Discord.Commands.ParameterPreconditionAttribute

## Bundled Preconditions

@Discord.Commands ship with several bundled Preconditions; you may
view their usages on their respective API pages.

* @Discord.Commands.RequireContextAttribute
* @Discord.Commands.RequireOwnerAttribute
* @Discord.Commands.RequireBotPermissionAttribute
* @Discord.Commands.RequireUserPermissionAttribute
* @Discord.Commands.RequireNsfwAttribute

## Custom Preconditions

To write your own Precondition, create a new class that inherits from
either [PreconditionAttribute] or [ParameterPreconditionAttribute]
depending on your use.

In order for your Precondition to function, you will need to override
the [CheckPermissionsAsync] method.

Your IDE should provide an option to fill this in for you.

If the context meets the required parameters, return
[PreconditionResult.FromSuccess], otherwise return
[PreconditionResult.FromError] and include an error message if
necessary.

[!code-csharp[Custom Precondition](samples/require_owner.cs)]

[CheckPermissionsAsync]: xref:Discord.Commands.PreconditionAttribute.CheckPermissionsAsync*
[PreconditionResult.FromSuccess]: xref:Discord.Commands.PreconditionResult.FromSuccess*
[PreconditionResult.FromError]: xref:Discord.Commands.PreconditionResult.FromError*
