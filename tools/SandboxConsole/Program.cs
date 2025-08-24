
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Discord;
using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

var ctx = new DiscordJsonContext();

var model = JsonSerializer.Deserialize<IChannelModel>(
    """
    {
      "id": "41771983423143937",
      "guild_id": "41771983423143937",
      "name": "general",
      "type": 0,
      "position": 6,
      "permission_overwrites": [],
      "rate_limit_per_user": 2,
      "nsfw": true,
      "topic": "24/7 chat about how to gank Mike #2",
      "last_message_id": "155117677105512449",
      "parent_id": "399942396007890945",
      "default_auto_archive_duration": 60
    }
    """,
    (JsonTypeInfo<IChannelModel>)ctx.GetTypeInfo(typeof(IChannelModel))!
);

var token = Environment.GetEnvironmentVariable("TOKEN");

if(token is null) throw new Exception("Missing environment variable 'TOKEN'");

var client = new DiscordRestClient(new DiscordConfig(new DiscordToken(token, TokenType.Bot)));

var user = await client.Users[1397804142042415165].GetAsync();

Console.WriteLine(user.Username);