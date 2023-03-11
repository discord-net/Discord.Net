using Discord;
using Discord.Rest;

await using var client = new DiscordRestClient();
await client.LoginAsync(TokenType.Bearer, "bearer token obtained through oauth2 flow");