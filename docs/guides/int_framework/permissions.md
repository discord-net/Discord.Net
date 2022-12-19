---
uid: Guides.IntFw.Perms
title: How to handle permissions.
---

# Permissions

This page covers everything to know about setting up permissions for Slash & context commands.

Application command (Slash, User & Message) permissions are set up at creation.
When you add your commands to a guild or globally, the permissions will be set up from the attributes you defined.

Commands that are added will only show up for members that meet the required permissions.
There is no further internal handling, as Discord deals with this on its own.

> [!WARNING]
> Permissions can only be configured at top level commands. Not in subcommands.

## Disallowing commands in DM

Commands can be blocked from being executed in DM if a guild is required to execute them in as followed:

[!code-csharp[no-DM permission](samples/permissions/guild-only.cs)]

> [!TIP]
> This attribute only works on global-level commands. Commands that are registered in guilds alone do not have a need for it.

## Server permissions

As previously shown, a command like ban can be blocked from being executed inside DMs,
as there are no members to ban inside of a DM. However, for a command like this,
we'll also want to make block it from being used by members that do not have the [permissions].
To do this, we can use the `DefaultMemberPermissions` attribute:

[!code-csharp[Server permissions](samples/permissions/guild-perms.cs)]

### Stacking permissions

If you want a user to have multiple [permissions] in order to execute a command, you can use the `|` operator, just like with setting up intents:

[!code-csharp[Permission stacking](samples/permissions/perm-stacking.cs)]

### Nesting permissions

Alternatively, permissions can also be nested.
It will look for all uses of `DefaultMemberPermissions` up until the highest level class.
The `EnabledInDm` attribute can be defined at top level as well,
and will be set up for all of the commands & nested modules inside this class.

[!code-csharp[Permission stacking](samples/permissions/perm-nesting.cs)]

The amount of nesting you can do is realistically endless.

> [!NOTE]
> If the nested class is marked with `Group`, as required for setting up subcommands, this example will not work.
> As mentioned before, subcommands cannot have seperate permissions from the top level command.

### NSFW Commands
Commands can be limited to only age restricted channels and DMs:

[!code-csharp[Nsfw-Permissions](samples/permissions/nsfw-permissions.cs)]

[permissions]: xref:Discord.GuildPermission

