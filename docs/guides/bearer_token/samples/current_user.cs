// gets the user object stored in DiscordRestClient.
var user = client.CurrentUser;

// fetches the current user with a REST call & updates the CurrentUser property.
var refreshedUser = await client.GetCurrentUserAsync();