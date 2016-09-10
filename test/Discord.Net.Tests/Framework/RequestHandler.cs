using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using UserRoutes = Discord.Tests.Framework.Routes.Users;
using GuildRoutes = Discord.Tests.Framework.Routes.Guilds;
using Contracts = Discord.Tests.Framework.Routes.Contracts;
using Newtonsoft.Json;
using Discord.Net.Converters;
using Discord.Net;

namespace Discord.Tests.Framework
{
    public class RequestHandler
    {
        public delegate object Response(string json, IReadOnlyDictionary<string, string> requestHeaders);

        internal static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings() { ContractResolver = new DiscordContractResolver() };

        internal Dictionary<string, Response> Routes = new Dictionary<string, Response>()
        {
            // --- USERS
            // Get Current User
            ["GET users/@me"] = new Response(UserRoutes.Me),
            // Get User by ID
            ["GET users/66078337084162048"] = new Response(UserRoutes.Public),
            // Get User by Tag
            ["GET users?q=foxbot%230282&limit=1"] = new Response(UserRoutes.Query),
            // --- GUILDS
            ["GET guilds/81384788765712384"] = new Response(GuildRoutes.DiscordApi)
        };

        internal Stream GetMock(string method, string endpoint, string json, IReadOnlyDictionary<string, string> requestHeaders)
        {
            var key = string.Format("{0} {1}", method.ToUpperInvariant(), endpoint.ToLowerInvariant());
            if (!Routes.ContainsKey(key))
                throw new HttpException(HttpStatusCode.NotFound, $"{key}: {json}");
            Contracts.EnsureAuthorization(requestHeaders);
            var model = Routes[key].Invoke(json, requestHeaders);
            var textResponse = JsonConvert.SerializeObject(model, SerializerSettings);
            return new MemoryStream(Encoding.UTF8.GetBytes(textResponse));
        }
    }
}
