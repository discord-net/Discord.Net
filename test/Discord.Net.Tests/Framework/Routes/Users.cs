using UserMocks = Discord.Tests.Framework.Mocks.Rest.Users;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using Discord.Net;
using System.Net;

namespace Discord.Tests.Framework.Routes
{
    public static class Users
    {
        public static readonly string UserToken = "token.user";
        public static readonly string BotToken = "token.bot";
        public static readonly string BearerToken = "token.bearer";

        public static object Me(string json, IReadOnlyDictionary<string, string> requestHeaders)
        {
            if (!requestHeaders.ContainsKey("authorization")) throw new HttpException(HttpStatusCode.Forbidden);
            if (requestHeaders["authorization"] != UserToken) throw new HttpException(HttpStatusCode.Forbidden);
            return UserMocks.SelfUser;
        }
    }
}
