using Newtonsoft.Json;
using System.IO;
using System;

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
            if (File.Exists(path))
            {
                using (var stream = new FileStream(path, FileMode.Open))
                using (var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader))
                    return new JsonSerializer().Deserialize<TestConfig>(jsonReader);
            }
            else
            {
                return new TestConfig()
                {
                    Token = Environment.GetEnvironmentVariable("DNET_TEST_TOKEN"),
                    GuildId = ulong.Parse(Environment.GetEnvironmentVariable("DNET_TEST_GUILDID"))
                };
            }
        }
    }
}