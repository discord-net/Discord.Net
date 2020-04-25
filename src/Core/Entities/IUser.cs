namespace Discord.Core
{
    /// <summary>
    /// An interface representing the collection of operations that can be
    /// performed on a user.
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// Gets a value indicating the unique identifier for this user.
        /// </summary>
        ulong Id { get; }

        /// <summary>
        /// Gets a value representing the user's username.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Gets a value representing the user's 4-digit discriminator.
        /// </summary>
        short Discriminator { get; }

        /// <summary>
        /// Gets a value representing the user's avatar hash, or
        /// <code>null</code> if one is not present.
        /// </summary>
        string? AvatarHash { get; }

        /// <summary>
        /// Gets a value representing whether the user is a bot account, or
        /// <code>null</code> if it is not known.
        /// </summary>
        bool? IsBot { get; }

        /// <summary>
        /// Gets a value representing whether the user is an Official Discord
        /// System account, or <code>null</code> if it is not known.
        /// </summary>
        bool? IsSystem { get; }

        /// <summary>
        /// Gets a value representing whether the user has two-factor
        /// authentication enabled, or <code>null</code> if it is not known.
        /// </summary>
        /// <remarks>
        /// If <see cref="IsBot"/> is true, this field represents the owner's
        /// two factor authentication status.
        /// </remarks>
        bool? HasMfaEnabled { get; }

        /// <summary>
        /// Gets a value representing the user's locale, or <code>null</code>
        /// if it is not known.
        /// </summary>
        string? Locale { get; }

        /// <summary>
        /// Gets a value representing whether the user's E-Mail address is
        /// verified, or <code>null</code> if it is not known.
        /// </summary>
        bool? EmailVerified { get; }

        /// <summary>
        /// Gets a value representing the user's E-Mail address, or
        /// <code>null</code> if it is not known.
        /// </summary>
        string? EMailAddress { get; }

        /// <summary>
        /// Gets a value representing the flags on the user's account, or
        /// <code>null</code> if it is not known.
        /// </summary>
        UserFlags? Flags { get; }

        /// <summary>
        /// Gets a value representing the type of nitro subscription on the
        /// user's account, or <code>null</code> if it is not known.
        /// </summary>
        NitroType? NitroType { get; }

        /// <summary>
        /// Gets a value representing the public flags on the user's account,
        /// or <code>null</code> if it is not known.
        /// </summary>
        UserFlags? PublicFlags { get; }
    }
}
