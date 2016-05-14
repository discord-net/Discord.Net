using System;
using System.Collections.Generic;

namespace Discord.Tests.Rest
{
    public class EndpointHandler
    {
        public static EndpointHandler Instance;

        public delegate string RestMessageHandler(string method, string json);

        public Dictionary<string, RestMessageHandler> Handlers;

        public EndpointHandler()
        {
            Instance = this;

            // Setup Endpoints
            Handlers = new Dictionary<string, RestMessageHandler>();

            // /users Endpoints
            Handlers.Add("GET->users/@me", Responses.Users.UserHandlers.Me_Handler);
            Handlers.Add("GET->users/96642168176807936", Responses.Users.UserHandlers.Id_User_Valid);
            Handlers.Add("GET->users/1", Responses.Users.UserHandlers.Id_User_Invalid);
            Handlers.Add("GET->users/@me/guilds", Responses.Users.UserHandlers.Me_Guilds);

            // /guilds endpoints
            Handlers.Add("GET->guilds/66078535390867456", Responses.Guilds.GuildHandlers.Id_Valid);
            Handlers.Add("GET->guilds/66078535390867456/bans", Responses.Guilds.GuildHandlers.Bans_Valid);
            Handlers.Add("GET->guilds/66078535390867456/members?limit=2147483647&offset=0", Responses.Guilds.GuildHandlers.Members_Valid);
            Handlers.Add("GET->guilds/66078535390867456/members/1", Responses.Guilds.GuildHandlers.Member_Invalid);
            Handlers.Add("GET->guilds/66078535390867456/members/66078337084162048", Responses.Guilds.GuildHandlers.Member_Valid);
            Handlers.Add("GET->guilds/1", Responses.Guilds.GuildHandlers.Id_Invalid);
        }

        public string HandleMessage(string method, string endpoint, string json)
        {
            var key = $"{method}->{endpoint}";
            if (Handlers.ContainsKey(key))
                return Handlers[key].Invoke(method, json);
            throw new NotImplementedException($"{method} -> {endpoint} -> {json}");
        }
    }
}
