---
uid: Discord.Commands.PreconditionAttribute
remarks: *content
---

This precondition attribute can be applied on module-level or
method-level for a command.

[!include[Additional Remarks](PreconditionAttribute.Remarks.Inclusion.md)]

---
uid: Discord.Commands.ParameterPreconditionAttribute
remarks: *content
---

This precondition attribute can be applied on parameter-level for a
command.

[!include[Additional Remarks](PreconditionAttribute.Remarks.Inclusion.md)]

---
uid: Discord.Commands.PreconditionAttribute
example: [*content]
---

The following example creates a precondition to see if the user has
sufficient role required to access the command.

```cs
public class RequireRoleAttribute : PreconditionAttribute
{
    private readonly ulong _roleId;

    public RequireRoleAttribute(ulong roleId)
    {
        _roleId = roleId;
    }

    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
        CommandInfo command, IServiceProvider services)
    {
        var guildUser = context.User as IGuildUser;
        if (guildUser == null)
            return PreconditionResult.FromError("This command cannot be executed outside of a guild.");

        var guild = guildUser.Guild;
        if (guild.Roles.All(r => r.Id != _roleId))
            return PreconditionResult.FromError(
                $"The guild does not have the role ({_roleId}) required to access this command.");

        return guildUser.RoleIds.Any(rId => rId == _roleId)
            ? PreconditionResult.FromSuccess()
            : PreconditionResult.FromError("You do not have the sufficient role required to access this command.");
    }
}
```

---
uid: Discord.Commands.ParameterPreconditionAttribute
example: [*content]
---

The following example creates a precondition on a parameter-level to
see if the targeted user has a lower hierarchy than the user who
executed the command.

```cs
public class RequireHierarchyAttribute : ParameterPreconditionAttribute
{
    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
        ParameterInfo parameter, object value, IServiceProvider services)
    {
        // Hierarchy is only available under the socket variant of the user.
        if (!(context.User is SocketGuildUser guildUser))
            return PreconditionResult.FromError("This command cannot be used outside of a guild.");

        SocketGuildUser targetUser;
        switch (value)
        {
            case SocketGuildUser targetGuildUser:
                targetUser = targetGuildUser;
                break;
            case ulong userId:
                targetUser = await context.Guild.GetUserAsync(userId).ConfigureAwait(false) as SocketGuildUser;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (targetUser == null)
            return PreconditionResult.FromError("Target user not found.");

        if (guildUser.Hierarchy < targetUser.Hierarchy)
            return PreconditionResult.FromError("You cannot target anyone else whose roles are higher than yours.");

        var currentUser = await context.Guild.GetCurrentUserAsync().ConfigureAwait(false) as SocketGuildUser;
        if (currentUser?.Hierarchy < targetUser.Hierarchy)
            return PreconditionResult.FromError("The bot's role is lower than the targeted user.");

        return PreconditionResult.FromSuccess();
    }
}
```