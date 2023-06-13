using System;
using System.Linq;

namespace Discord.Net
{
    [Obsolete("Please use HttpException instead of this. Will be removed in next major version.", false)]
    public class ApplicationCommandException : HttpException
    {
        public ApplicationCommandException(HttpException httpError)
            : base(httpError.HttpCode, httpError.Request, httpError.DiscordCode, httpError.Reason, httpError.Errors.ToArray())
        {

        }
    }
}
