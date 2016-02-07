using Discord.ETF;
using System.IO;
using System;
using Discord.Logging;

namespace Discord.Net.Rest
{
    public class ETFRestClient : RestClient
    {
        private readonly ETFWriter _serializer;

        public ETFRestClient(DiscordConfig config, string baseUrl, ILogger logger = null)
            : base(config, baseUrl, logger)
        {
            _serializer = new ETFWriter(new MemoryStream());
        }

        protected override string Serialize<T>(T obj)
        {
            throw new NotImplementedException();
        }
        protected override T Deserialize<T>(string json)
        {
            throw new NotImplementedException();
        }
    }
}
