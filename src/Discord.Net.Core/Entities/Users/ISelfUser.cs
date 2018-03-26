using System;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary> Represents a logged-in Discord user. </summary>
    public interface ISelfUser : IUser
    {
        /// <summary> Gets the email associated with this user. </summary>
        string Email { get; }
        /// <summary> Returns true if this user's email has been verified. </summary>
        bool IsVerified { get; }
        /// <summary> Returns true if this user has enabled MFA on their account. </summary>
        bool IsMfaEnabled { get; }

        Task ModifyAsync(Action<SelfUserProperties> func, RequestOptions options = null);
    }
}
