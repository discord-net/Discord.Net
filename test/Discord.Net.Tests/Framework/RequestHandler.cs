using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using UserRoutes = Discord.Tests.Framework.Routes.Users;
using Newtonsoft.Json;
using Discord.Net.Converters;

namespace Discord.Tests.Framework
{
    public class RequestHandler
    {
        public delegate object Response(string json, IReadOnlyDictionary<string, string> requestHeaders);

        internal static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings() { ContractResolver = new DiscordContractResolver() };

        internal Dictionary<string, Response> Routes = new Dictionary<string, Response>()
        {
            ["GET users/@me"] = new Response(UserRoutes.Me)
        };

        internal Stream GetMock(string method, string endpoint, string json, IReadOnlyDictionary<string, string> requestHeaders)
        {
            var key = string.Format("{0} {1}", method.ToUpperInvariant(), endpoint.ToLowerInvariant());
            if (!Routes.ContainsKey(key))
                throw new NotImplementedException($"{key}: {json}");
            var model = Routes[key].Invoke(json, requestHeaders);
            var textResponse = JsonConvert.SerializeObject(model, SerializerSettings);
            return new MemoryStream(Encoding.UTF8.GetBytes(textResponse));
        }
    }
}
