Permissions
===========

There are two types of permissions: *Channel Permissions* and *Server Permissions*.

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

If a user has a permission, the value is true. Otherwise, it must be null.

Dual Channel Permissions
------------------------
You may also access a user's permissions in a channel with the DualChannelPermissions class.
Unlike normal ChannelPermissions, DualChannelPermissions hold three values:

If a user has a permission, the value is true. If a user is denied a permission, it will be false. If the permission is not set, the value will return null.

Setting Channel Permissions
---------------------------

To set channel permissions, you may use either two ChannelPermissions, or one DualChannelPermissions.

In the case of using two Channel Permissions, you must create one list of allowed permissions, and one list of denied permissions.
Otherwise, you can use a single DualChannelPermissions.

Server Permissions
------------------

Server Permissions can be accessed by ``Server.GetPermissions(User)``, and updated with ``Server.UpdatePermissions(User, ServerPermissions)``

A user's server permissions also contain the default values for it's channel permissions, so the channel permissions listed above are also valid flags for Server Permissions. There are also a few extra Server Permissions:

======================= ======= ==============
Flag                    Type    Description
======================= ======= ==============
BanMembers              Server  Ban users from the server.
KickMembers             Server  Kick users from the server. They can still rejoin.
ManageRoles             Server  Manage roles on the server, and their permissions.
ManageChannels          Server  Manage channels that exist on the server (add, remove them)
ManageServer            Server  Manage the server settings.

Roles
-----

Managing permissions for roles is much easier than for users in channels. For roles, just access the flag under `Role.Permissions`.

Example
-------

.. literalinclude:: /samples/permissions.cs
   :language: csharp6
   :tab-width: 2
