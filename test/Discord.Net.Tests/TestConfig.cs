using Newtonsoft.Json;
using System.IO;

namespace Discord
{
    internal class TestConfig
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
        [JsonProperty("guild_id")]
        public ulong GuildId { get; private set; }
        
        public static TestConfig LoadFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
                return new JsonSerializer().Deserialize<TestConfig>(jsonReader);
        }
    }
}