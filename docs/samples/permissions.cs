
// Find a User's Channel Permissions
var UserPerms = _channel.GetPermissionsRule(_user);

// Set a User's Channel Permissions

var NewOverwrites = new ChannelPermissionOverrides(sendMessages: PermValue.Deny);
await channel.AddPermissionsRule(_user, NewOverwrites);
