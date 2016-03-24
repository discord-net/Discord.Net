Server Management
=================

Discord.Net will allow you to manage most settings of a Discord server.

Usage
-----

You can create Channels, Invites, and Roles on a server using the CreateChannel, CreateInvite, and CreateRole function of a Server, respectively.

You may also edit a server's name, icon, and region.

.. code-block:: csharp6

    // Create a Channel and retrieve the Channel object
    var _channel = await _server.CreateChannel("announcements", ChannelType.Text);

    // Create an Invite and retrieve the Invite object
    var _invite = await _server.CreateInvite(maxAge: null, maxUses: 25, tempMembership: false, withXkcd: false);

    // Create a Role and retrieve the Role object
    var _role = await _server.CreateRole(name: "Bots", permissions: null, color: Color.DarkMagenta, isHoisted: false);

    // Edit a server
    var _ioStream = new System.IO.StreamReader("clock-0500-1952.png").BaseStream
    _server.Edit(name: "19:52 | UTC-05:00", region: "east", icon: _ioStream, iconType: ImageType.Png);

    // Prune Users
    var _pruneCount = await _server.PruneUsers(30, true);

Invite Parameters
-----------------

maxAge: The time (in seconds) until the invite expires. Use null for infinite.
maxUses: The maximum amount of uses the invite has before it expires.
tempMembership: Whether or not to kick a user when they disconnect.
withXkcd: Generate the invite with an XKCD 936 style URL

Role Parameters
---------------

name: The name of the role
permissions: A set of ServerPermissions for the role to use by default
color: The color of the role, recommended to use Discord.Color
isHoisted: Whether a role's users should be displayed separately from other users in the user list.

Edit Parameters
---------------

name: The server's name
region: The region the voice server is hosted in
icon: A System.IO.Stream that will read an image file
iconType: The type of image being sent (png/jpeg).
