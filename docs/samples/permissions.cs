// Finding User Permissions

void FindPermissions(User u, Channel c)
{
	ChannelPermissions cperms = u.GetPermissions(c);
	ServerPermissions sperms = u.GetServerPermissions();
}

void SetPermissionsChannelPerms(User u, Channel c)
{
	ChannelPermissions allow = new ChannelPermissions();
	ChannelPermissions deny = new ChannelPermissions();

	allow.Connect = true;
	deny.AttachFiles = true;

	client.SetChannelPermissions(c, u, allow, deny)
}

void SetPermissionsDualPerms(User u, Channel c)
{
	DualChannelPermissions dual = new DualChannelPermissions();
    dual.ReadMessageHistory = false;
    dual.Connect = true;
    dual.AttachFiles = null;

    client.SetChannelPermissions(c, u, dual);
}