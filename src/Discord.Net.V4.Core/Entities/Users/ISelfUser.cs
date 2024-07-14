using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using IModifiable = IModifiable<ulong, ISelfUser, ModifySelfUserProperties, ModifyCurrentUserParams, ISelfUserModel>;

/// <summary>
///     Represents the logged-in Discord user.
/// </summary>
public partial interface ISelfUser :
    IUser,
    ISelfUserActor,
    IModifiable,
    IRefreshable<ISelfUser, ulong, ISelfUserModel>
{
    [SourceOfTruth]
    new Task RefreshAsync(RequestOptions? options, CancellationToken token)
        => (this as IRefreshable<ISelfUser, ulong, ISelfUserModel>).RefreshAsync(options, token);

    static IApiOutRoute<ISelfUserModel> IRefreshable<ISelfUser, ulong, ISelfUserModel>.RefreshRoute(
        IPathable path,
        ulong id
    ) => Routes.GetCurrentUser;

    static IApiInOutRoute<ModifyCurrentUserParams, IEntityModel> IModifiable.ModifyRoute(
        IPathable path,
        ulong id,
        ModifyCurrentUserParams args
    ) => Routes.ModifyCurrentUser(args);

    /// <summary>
    ///     Gets the email associated with this user.
    /// </summary>
    string Email { get; }

    /// <summary>
    ///     Indicates whether or not this user has their email verified.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if this user's email has been verified; <see langword="false" /> if not.
    /// </returns>
    bool IsVerified { get; }

    /// <summary>
    ///     Indicates whether or not this user has MFA enabled on their account.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if this user has enabled multi-factor authentication on their account;
    ///     <see langword="false" /> if not.
    /// </returns>
    bool IsMfaEnabled { get; }

    /// <summary>
    ///     Gets the flags that are applied to a user's account.
    /// </summary>
    /// <remarks>
    ///     This value is determined by bitwise OR-ing <see cref="UserFlags" /> values together.
    /// </remarks>
    /// <returns>
    ///     The value of flags for this user.
    /// </returns>
    UserFlags Flags { get; }

    /// <summary>
    ///     Gets the type of Nitro subscription that is active on this user's account.
    /// </summary>
    /// <remarks>
    ///     This information may only be available with the identify OAuth scope.
    /// </remarks>
    /// <returns>
    ///     The type of Nitro subscription the user subscribes to, if any.
    /// </returns>
    PremiumType PremiumType { get; }

    /// <summary>
    ///     Gets the user's chosen language option.
    /// </summary>
    /// <returns>
    ///     The IETF language tag of the user's chosen region, if provided.
    ///     For example, a locale of "English, US" is "en-US", "Chinese (Taiwan)" is "zh-TW", etc.
    /// </returns>
    string Locale { get; }
}
