using Discord.API.Rest;
using System;
using System.Threading.Tasks;

namespace Discord
{
    public interface ISelfUser : IUser, IUpdateable
    {
        /// <summary> Gets the email associated with this user. </summary>
        string Email { get; }
        /// <summary> Returns true if this user's email has been verified. </summary>
        bool IsVerified { get; }

        Task ModifyAsync(Action<ModifyCurrentUserParams> func);
    }
}