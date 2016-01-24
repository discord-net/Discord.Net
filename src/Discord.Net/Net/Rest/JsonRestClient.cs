using Discord.Logging;
using Newtonsoft.Json;

namespace Discord.Net.Rest
{
    public class JsonRestClient : RestClient
    {
        private JsonSerializerSettings _deserializeSettings;

        public JsonRestClient(DiscordConfig config, string baseUrl, Logger logger)
            : base(config, baseUrl, logger)
        {
            _deserializeSettings = new JsonSerializerSettings();
#if TEST_RESPONSES
            _deserializeSettings.CheckAdditionalContent = true;
            _deserializeSettings.MissingMemberHandling = MissingMemberHandling.Error;
#else
            _deserializeSettings.CheckAdditionalContent = false;
            _deserializeSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
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
            return JsonConvert.DeserializeObject<T>(json, _deserializeSettings);
        }
    }
}
