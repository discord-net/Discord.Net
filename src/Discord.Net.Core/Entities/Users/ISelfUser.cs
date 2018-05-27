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
        ///     Returns <c>true</c> if this user's email has been verified.
        /// </summary>
        bool IsVerified { get; }
        /// <summary>
        ///     Returns <c>true</c> if this user has enabled MFA on their account.
        /// </summary>
        bool IsMfaEnabled { get; }

        /// <summary>
        ///     Modifies the user's properties.
        /// </summary>
        Task ModifyAsync(Action<SelfUserProperties> func, RequestOptions options = null);
    }
}
