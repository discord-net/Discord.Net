// fetch application role connection of the current user for the app with provided id.
var roleConnection = await client.GetUserApplicationRoleConnectionAsync(applicationid);

// create a new role connection metadata properties object & set some values.
var properties = new RoleConnectionProperties("Discord.Net Docs", "Cool Coding Guy")
    .WithNumber("eaten_cookies", 69)
    .WithBool("loves_cookies", true)
    .WithDate("last_eaten_cookie", DateTimeOffset.UtcNow);

// update current user's values with the given properties.
await client.ModifyUserApplicationRoleConnectionAsync(applicationId, properties);
