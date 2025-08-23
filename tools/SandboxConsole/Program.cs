
using Discord;
using Discord.Rest;

var token = Environment.GetEnvironmentVariable("TOKEN");

if(token is null) throw new Exception("Missing environment variable 'TOKEN'");

var client = new DiscordRestClient(new DiscordConfig(new DiscordToken(token, TokenType.Bot)));

var user = await client.Users[1397804142042415165].GetAsync();

Console.WriteLine(user.Username);