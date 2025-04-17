using System.Threading.Tasks;
using System;

namespace Discord.Interactions;

/// <summary>
///     Requires the bot to have a specific permission in the context a command is invoked in.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequireContextPermissionAttribute : PreconditionAttribute
{
    /// <summary>
    ///     Gets the specified <see cref="Discord.GuildPermission" /> of the precondition.
    /// </summary>
    public GuildPermission? GuildPermission { get; }

    /// <summary>
    ///     Gets the specified <see cref="Discord.ChannelPermission" /> of the precondition.
    /// </summary>
    public ChannelPermission? ChannelPermission { get; }

    /// <summary>
    ///     Gets or sets the error message if the precondition
    ///     fails due to being run outside of a Guild channel.
    /// </summary>
    public string NotAGuildErrorMessage { get; set; }

    /// <summary>
    ///     Requires the bot account to have a specific <see cref="Discord.GuildPermission"/>.
    /// </summary>
    /// <remarks>
    ///     This precondition will always fail if the command is being invoked in a <see cref="IPrivateChannel"/>.
    /// </remarks>
    /// <param name="permission">
    ///     The <see cref="Discord.GuildPermission"/> that the bot must have. Multiple permissions can be specified
    ///     by ORing the permissions together.
    /// </param>
    public RequireContextPermissionAttribute(GuildPermission permission)
    {
        GuildPermission = permission;
        ChannelPermission = null;
    }
    /// <summary>
    ///     Requires that the bot account to have a specific <see cref="Discord.ChannelPermission"/>.
    /// </summary>
    /// <param name="permission">
    ///     The <see cref="Discord.ChannelPermission"/> that the bot must have. Multiple permissions can be
    ///     specified by ORing the permissions together.
    /// </param>
    public RequireContextPermissionAttribute(ChannelPermission permission)
    {
        ChannelPermission = permission;
        GuildPermission = null;
    }

    /// <inheritdoc />
    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo command, IServiceProvider services)
    {
        if (GuildPermission.HasValue)
        {
            if (context.Interaction.GuildId is null)
                return Task.FromResult(PreconditionResult.FromError(NotAGuildErrorMessage ?? "Command must be used in a guild channel."));
            if (!context.Interaction.Permissions.Has(GuildPermission.Value))
                return Task.FromResult(PreconditionResult.FromError(ErrorMessage ?? $"Bot requires guild permission {GuildPermission.Value}."));
        }

        if (ChannelPermission.HasValue)
        {
            var channelPerms = new ChannelPermissions(context.Interaction.Permissions.RawValue);
            if (!channelPerms.Has(ChannelPermission.Value))
                return Task.FromResult(PreconditionResult.FromError(ErrorMessage ?? $"Bot requires channel permission {ChannelPermission.Value}."));
        }

        return Task.FromResult(PreconditionResult.FromSuccess());
    }

}
