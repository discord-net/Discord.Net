using Discord.Models.Json;

namespace Discord;

/// <summary>
///     Properties that are used to modify the <see cref="ICurrentUser" /> with the specified changes.
/// </summary>
/// <seealso cref="ICurrentUser.ModifyAsync" />
public sealed class ModifySelfUserProperties : IEntityProperties<ModifyCurrentUserParams>
{
    /// <summary>
    ///     Gets or sets the username.
    /// </summary>
    public Optional<string> Username { get; set; }

    /// <summary>
    ///     Gets or sets the avatar.
    /// </summary>
    public Optional<Image?> Avatar { get; set; }

    public ModifyCurrentUserParams ToApiModel(ModifyCurrentUserParams? existing = default)
    {
        existing ??= new ModifyCurrentUserParams();

        existing.Username = Username;
        existing.Avatar = Avatar.Map(v => v?.ToImageData());

        return existing;
    }
}
