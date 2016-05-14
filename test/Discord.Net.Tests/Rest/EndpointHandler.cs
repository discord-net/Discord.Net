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
            Handlers.Add("users/@me", Responses.Users.UserHandlers.Me_Handler);
        }

        public string HandleMessage(string method, string endpoint, string json)
        {
            if (Handlers.ContainsKey(endpoint))
                return Handlers[endpoint].Invoke(method, json);
            throw new NotImplementedException($"{method} -> {endpoint} -> {json}");
        }
    }
}
