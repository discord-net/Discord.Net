using Discord.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord.Tests.Framework.Routes
{
    public static class Contracts
    {
        public static readonly string UserToken = "token.user";
        public static readonly string BotToken = "token.bot";
        public static readonly string BearerToken = "token.bearer";

        public static void EnsureAuthorization(IReadOnlyDictionary<string, string> requestHeaders)
        {
            if (!requestHeaders.ContainsKey("authorization")) throw new HttpException(HttpStatusCode.Forbidden);
            if (requestHeaders["authorization"] != UserToken
                && requestHeaders["authorization"] != $"Bot {BotToken}"
                && requestHeaders["authorization"] != $"Bearer {BearerToken}") throw new HttpException(HttpStatusCode.Forbidden);
        }
    }
}
