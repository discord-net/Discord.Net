using Discord.Logging;
using Newtonsoft.Json;
#if TEST_RESPONSES
using System;
#endif
using System.IO;

namespace Discord.Net.Rest
{
    public class JsonRestClient : RestClient
    {
        private JsonSerializer _serializer;

        public JsonRestClient(DiscordConfig config, string baseUrl, ILogger logger = null)
            : base(config, baseUrl, logger)
        {
            _serializer = new JsonSerializer();
#if TEST_RESPONSES
            _serializer.CheckAdditionalContent = true;
            _serializer.MissingMemberHandling = MissingMemberHandling.Error;
#else
            _serializer.CheckAdditionalContent = false;
            _serializer.MissingMemberHandling = MissingMemberHandling.Ignore;
#endif
        }

        protected override string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        protected override T Deserialize<T>(string json)
        {
#if TEST_RESPONSES
			if (string.IsNullOrEmpty(json))
				throw new Exception("API check failed: Response is empty.");
#endif
            using (var reader = new JsonTextReader(new StringReader(json)))
                return (T)_serializer.Deserialize(reader, typeof(T));
        }
    }
}
