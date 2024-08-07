using Discord.Models.Json;

namespace Discord;

/// <summary>
///     Properties that are used to modify an <see cref="IMember" /> with the following parameters.
/// </summary>
/// <seealso cref="IMember.ModifyAsync" />
public sealed class ModifyGuildUserProperties : IEntityProperties<ModifyGuildMemberParams>
{
    /// <summary>
    ///     Gets or sets whether the user should be muted in a voice channel.
    /// </summary>
    /// <remarks>
    ///     If this value is set to <see langword="true" />, no user will be able to hear this user speak in the guild.
    /// </remarks>
    public Optional<bool> Mute { get; set; }

    /// <summary>
    ///     Gets or sets whether the user should be deafened in a voice channel.
    /// </summary>
    /// <remarks>
    ///     If this value is set to <see langword="true" />, this user will not be able to hear anyone speak in the guild.
    /// </remarks>
    public Optional<bool> Deaf { get; set; }

    /// <summary>
    ///     Gets or sets the user's nickname.
    /// </summary>
    /// <remarks>
    ///     To clear the user's nickname, this value can be set to <see langword="null" /> or
    ///     <see cref="string.Empty" />.
    /// </remarks>
    public Optional<string> Nickname { get; set; }

    /// <summary>
    ///     Gets or sets the roles the user should have.
    /// </summary>
    public Optional<IEnumerable<EntityOrId<ulong, IRole>>> Roles { get; set; }

    /// <summary>
    ///     Moves a user to a voice channel. If <see langword="null" />, this user will be disconnected from their current
    ///     voice channel.
    /// </summary>
    /// <remarks>
    ///     This user MUST already be in a <see cref="IVoiceChannel" /> for this to work.
    ///     When set, this property takes precedence over <see cref="ChannelId" />.
    /// </remarks>
    public Optional<EntityOrId<ulong, IVoiceChannel>?> Channel { get; set; }

    /// <summary>
    ///     Sets a timestamp how long a user should be timed out for.
    /// </summary>
    /// <remarks>
    ///     <see langword="null" /> or a time in the past to clear a currently existing timeout.
    /// </remarks>
    public Optional<DateTimeOffset?> TimedOutUntil { get; set; }

    /// <summary>
    ///     Gets or sets the flags of the guild member.
    /// </summary>
    /// <remarks>
    ///     Not all flags can be modified, these are reserved for Discord.
    /// </remarks>
    public Optional<GuildMemberFlags> Flags { get; set; }

    public ModifyGuildMemberParams ToApiModel(ModifyGuildMemberParams? existing = default)
    {
        existing ??= new ModifyGuildMemberParams();

        existing.Nickname = Nickname;
        existing.IsDeaf = Deaf;
        existing.IsMute = Mute;
        existing.RoleIds = Roles.Map(v => v.Select(v => v.Id).ToArray());
        existing.UserFlags = Flags.Map(v => (int)v);
        existing.CommunicationDisabledUntil = TimedOutUntil;
        existing.VoiceChannelId = Channel.Map(v => v?.Id);

        return existing;
    }
}
