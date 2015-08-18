using Newtonsoft.Json;
using System.IO;

namespace Discord.Net.Tests
{
	internal class Settings
	{
		private const string path = "../../config.json";
		public static Settings Load()
		{
			if (!File.Exists(path))
				throw new FileNotFoundException("config.json is missing, rename config.json.example and add data for two separate unused accounts for testing.");
            return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path));
		}

		public class Account
		{
			[JsonProperty("email")]
			public string Email { get; set; }
			[JsonProperty("password")]
			public string Password { get; set; }
		}

		[JsonProperty("user1")]
		public Account User1 { get; set; }
		[JsonProperty("user2")]
		public Account User2 { get; set; }
	}
}
