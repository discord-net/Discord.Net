// fetches the current user's guild member object in a guild with provided id.
var member = await client.GetCurrentUserGuildMemberAsync(guildId);

// fetches the current user's guild member object in a RestUserGuild.
var guild = await client.GetGuildSummariesAsync().FlattenAsync().First();
var member = await guild.GetCurrentUserGuildMemberAsync();