using System;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents the logged-in Discord user.
    /// </summary>
    public interface ISelfUser : IUser
    {
        /// <summary>
        ///     Gets the email associated with this user.
        /// </summary>
        string Email { get; }
        /// <summary>
        ///     Indicates whether or not this user has their email verified.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this user's email has been verified; <c>false</c> if not.
        /// </returns>
        bool IsVerified { get; }
        /// <summary>
        ///     Indicates whether or not this user has MFA enabled on their account.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this user has enabled multi-factor authentication on their account; <c>false</c> if not.
        /// </returns>
        bool IsMfaEnabled { get; }

        /// <summary>
        ///     Modifies the user's properties.
        /// </summary>
        Task ModifyAsync(Action<SelfUserProperties> func, RequestOptions options = null);
    }
}
