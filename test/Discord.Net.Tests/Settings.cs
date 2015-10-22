using Newtonsoft.Json;
using System.IO;

namespace Discord.Tests
{
	internal class Settings
	{
		private const string path = "../../config.json";
		public static readonly Settings Instance;
		static Settings()
		{
			if (!File.Exists(path))
				throw new FileNotFoundException("config.json is missing, rename config.json.example and add credentials for three separate unused accounts for testing.");
			Instance = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path));
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
		[JsonProperty("user3")]
		public Account User3 { get; set; }
	}
}
