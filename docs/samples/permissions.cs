// Find a User's Channel Permissions
var userChannelPermissions = user.GetPermissions(channel);

// Find a User's Server Permissions
var userServerPermissions = user.ServerPermissions();
var userServerPermissions = server.GetPermissions(user);

// Set a User's Channel Permissions (using DualChannelPermissions)

var userPerms = user.GetPermissions(channel);
userPerms.ReadMessageHistory = false;
userPerms.AttachFiles = null;
channel.AddPermissionsRule(user, userPerms);
}
