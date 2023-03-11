// fetches the guilds the current user participate in.
var guilds = await client.GetGuildSummariesAsync().FlattenAsync();