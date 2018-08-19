using System;
using System.IO;
using Newtonsoft.Json;

namespace Discord
{
    internal class TestConfig
    {
        [JsonProperty("token")] public string Token { get; private set; }

        [JsonProperty("guild_id")] public ulong GuildId { get; private set; }

        public static TestConfig LoadFile(string path)
        {
            if (!File.Exists(path))
                return new TestConfig
                {
                    Token = Environment.GetEnvironmentVariable("DNET_TEST_TOKEN"),
                    GuildId = ulong.Parse(Environment.GetEnvironmentVariable("DNET_TEST_GUILDID"))
                };
            using (var stream = new FileStream(path, FileMode.Open))
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
                return new JsonSerializer().Deserialize<TestConfig>(jsonReader);
        }
    }
}
