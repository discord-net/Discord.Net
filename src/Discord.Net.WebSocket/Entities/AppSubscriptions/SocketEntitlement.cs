using Discord.Rest;
using System;

using Model = Discord.API.Entitlement;

namespace Discord.WebSocket;

public class SocketEntitlement : SocketEntity<ulong>, IEntitlement
{
    /// <inheritdoc/>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc/>
    public ulong SkuId { get; private set; }

    /// <inheritdoc cref="IEntitlement.UserId"/>
    public Cacheable<SocketUser, RestUser, IUser, ulong>? User { get; private set; }

    /// <inheritdoc cref="IEntitlement.GuildId"/>
    public Cacheable<SocketGuild, RestGuild, IGuild, ulong>? Guild { get; private set; }

    /// <inheritdoc/>
    public ulong ApplicationId { get; private set; }

    /// <inheritdoc/>
    public EntitlementType Type { get; private set; }

    /// <inheritdoc/>
    public bool IsConsumed { get; private set; }

    /// <inheritdoc/>
    public DateTimeOffset? StartsAt { get; private set; }

    /// <inheritdoc/>
    public DateTimeOffset? EndsAt { get; private set; }

    internal SocketEntitlement(DiscordSocketClient discord, ulong id) : base(discord, id)
    {
    }

    internal static SocketEntitlement Create(DiscordSocketClient discord, Model model)
    {
        var entity = new SocketEntitlement(discord, model.Id);
        entity.Update(model);
        return entity;
    }

    internal void Update(Model model)
    {
        SkuId = model.SkuId;

        if (model.UserId.IsSpecified)
        {
            var user = Discord.GetUser(model.UserId.Value);

            User = new Cacheable<SocketUser, RestUser, IUser, ulong>(user, model.UserId.Value, user is not null, async ()
                    => await Discord.Rest.GetUserAsync(model.UserId.Value));
        }

        if (model.GuildId.IsSpecified)
        {
            var guild = Discord.GetGuild(model.GuildId.Value);

            Guild = new Cacheable<SocketGuild, RestGuild, IGuild, ulong>(guild, model.GuildId.Value, guild is not null, async ()
                => await Discord.Rest.GetGuildAsync(model.GuildId.Value));
        }

        ApplicationId = model.ApplicationId;
        Type = model.Type;
        IsConsumed = model.IsConsumed.GetValueOrDefault(false);
        StartsAt = model.StartsAt.IsSpecified
            ? model.StartsAt.Value
            : null;
        EndsAt = model.EndsAt.IsSpecified
            ? model.EndsAt.Value
            : null;
    }

    internal SocketEntitlement Clone() => MemberwiseClone() as SocketEntitlement;

    #region IEntitlement

    /// <inheritdoc/>
    ulong? IEntitlement.GuildId => Guild?.Id;

    /// <inheritdoc/>
    ulong? IEntitlement.UserId => User?.Id;

    #endregion

}
