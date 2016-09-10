using UserMocks = Discord.Tests.Framework.Mocks.Rest.Users;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Tests.Framework.Routes
{
    public static class Users
    {
        public static object Me(string json, IReadOnlyDictionary<string, string> requestHeaders)
        {
            if (requestHeaders["authorization"] == Contracts.UserToken || requestHeaders["authorization"] == $"Bearer {Contracts.BearerToken}")
                return UserMocks.SelfUser;
            else
                return UserMocks.BotSelfUser;
        }

        public static object Public(string json, IReadOnlyDictionary<string, string> requestHeaders) =>
            UserMocks.PublicUser;
        public static object Query(string json, IReadOnlyDictionary<string, string> requestHeaders) =>
            ImmutableArray.Create(UserMocks.PublicUser);
    }
}
