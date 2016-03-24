Permissions
===========

|outdated|

There are two types of permissions: *Channel Permissions* and *Server Permissions*.

Permission Overrides
--------------------

Channel Permissions are expressed using an enum, ``PermValue``.

The three states are fairly straightforward - 

``PermValue.Allow``: Allow the user to perform a permission.
``PermValue.Deny``: Deny the user to perform a permission.
``PermValue.Inherit``: The user will inherit the permission from its role.

Channel Permissions
-------------------
Channel Permissions are controlled using a set of flags:

======================= ======= ==============
Flag                    Type    Description
======================= ======= ==============
AttachFiles             Text    Send files to a channel.
Connect                 Voice   Connect to a voice channel.
CreateInstantInvite     General Create an invite to the channel.
DeafenMembers           Voice   Prevent users of a voice channel from hearing other users (server-wide).
EmbedLinks              Text    Create embedded links.
ManageChannel           General Manage a channel.
ManageMessages          Text    Remove messages in a channel.
ManagePermissions       General Manage the permissions of a channel.
MentionEveryone         Text    Use @everyone in a channel.
MoveMembers             Voice   Move members around in voice channels.
MuteMembers             Voice   Mute users of a voice channel (server-wide).
ReadMessageHistory      Text    Read the chat history of a voice channel.
ReadMessages            Text    Read any messages in a text channel; exposes the text channel to users.
SendMessages            Text    Send messages in a text channel.
SendTTSMessages         Text    Send TTS messages in a text channel.
Speak                   Voice   Speak in a voice channel.
UseVoiceActivation      Voice   Use Voice Activation in a text channel (for large channels where PTT is preferred)
======================= ======= ==============

Each flag is a PermValue; see the section above.

Setting Channel Permissions
---------------------------

To set channel permissions, create a new ``ChannelPermissionOverrides``, and specify the flags/values that you want to override.

Then, update the user, by doing ``Channel.AddPermissionsRule(_user, _overwrites);``

Roles
-----

Accessing/modifying permissions for roles is done the same way as user permissions, just using the overload for a Role. See above sections.

Server Permissions
------------------

Server Permissions can be viewed with ``User.ServerPermissions``, but **at the time of this writing** cannot be set.

A user's server permissions also contain the default values for it's channel permissions, so the channel permissions listed above are also valid flags for Server Permissions. There are also a few extra Server Permissions:

======================= ======= ==============
Flag                    Type    Description
======================= ======= ==============
BanMembers              Server  Ban users from the server.
KickMembers             Server  Kick users from the server. They can still rejoin.
ManageRoles             Server  Manage roles on the server, and their permissions.
ManageChannels          Server  Manage channels that exist on the server (add, remove them)
ManageServer            Server  Manage the server settings.
======================= ======= ==============

Example
-------

.. literalinclude:: /samples/permissions.cs
   :language: csharp6
   :tab-width: 2
