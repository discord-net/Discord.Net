//using Discord.Rest;
using System.Diagnostics;
using Model = Discord.API.Ban;

namespace Discord
{

    public interface IBan
    {
        IUser User { get; }
        string Reason { get; }
    }
}
