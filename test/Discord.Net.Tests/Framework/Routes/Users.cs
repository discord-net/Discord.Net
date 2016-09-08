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
        

        public static object Me(string json, IReadOnlyDictionary<string, string> requestHeaders)
        {
            Contracts.EnsureAuthorization(requestHeaders);

            if (requestHeaders["authorization"] == Contracts.UserToken || requestHeaders["authorization"] == $"Bearer {Contracts.BearerToken}")
                return UserMocks.SelfUser;
            else
                return UserMocks.BotSelfUser;
        }

        public static object Public(string json, IReadOnlyDictionary<string, string> requestHeaders)
        {
            Contracts.EnsureAuthorization(requestHeaders);

            return UserMocks.PublicUser;
        }
        public static object InvalidPublic(string json, IReadOnlyDictionary<string, string> requestHeaders)
        {
            Contracts.EnsureAuthorization(requestHeaders);

            throw new HttpException(HttpStatusCode.NotFound);
        }
    }
}
