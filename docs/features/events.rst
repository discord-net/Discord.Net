|stub| Events
=============

Usage
-----
To take advantage of Events in Discord.Net, you need to hook into them. 

There are two ways of hooking into events. See the example for examples on using these events.

Usable Events
-------------
+--------------------+--------------------+------------------------------------------+
| Event Name         | EventArgs          | Description                              |
+====================+====================+==========================================+
| UserBanned         | BanEventArgs       | Called when a user is banned.            |
+--------------------+--------------------+------------------------------------------+
| UserUnbanned       | BanEventArgs       | Called when a user is unbanned.          |
+--------------------+--------------------+------------------------------------------+
| ChannelCreated     | ChannelEventArgs   | Called when a channel is created.        |
+--------------------+--------------------+------------------------------------------+
| ChannelDestroyed   | ChannelEventArgs   | Called when a channel is destroyed.      |
+--------------------+--------------------+------------------------------------------+
| ChannelUpdated     | ChannelEventArgs   | Called when a channel is updated.        |
+--------------------+--------------------+------------------------------------------+
| MessageReceived    | MessageEventArgs   | Called when a message is received.       |
+--------------------+--------------------+------------------------------------------+
| MessageSent        | MessageEventArgs   | Called when a message is sent.           |
+--------------------+--------------------+------------------------------------------+
| MessageDeleted     | MessageEventArgs   | Called when a message is deleted.        |
+--------------------+--------------------+------------------------------------------+
| MessageUpdated     | MessageEventArgs   | Called when a message is updated\\edited.|
+--------------------+--------------------+------------------------------------------+
| MessageReadRemotely| MessageEventArgs   | Called when a message is read.           |
+--------------------+--------------------+------------------------------------------+
| RoleCreated        | RoleEventArgs      | Called when a role is created.           |
+--------------------+--------------------+------------------------------------------+
| RoleUpdated        | RoleEventArgs      | Called when a role is updated.           |
+--------------------+--------------------+------------------------------------------+
| RoleDeleted        | RoleEventArgs      | Called when a role is deleted.           |
+--------------------+--------------------+------------------------------------------+
| JoinedServer       | ServerEventArgs    | Called when a member joins a server.     |
+--------------------+--------------------+------------------------------------------+
| LeftServer         | ServerEventArgs    | Called when a member leaves a server.    |
+--------------------+--------------------+------------------------------------------+
| ServerUpdated      | ServerEventArgs    | Called when a server is updated.         |
+--------------------+--------------------+------------------------------------------+
| ServerUnavailable  | ServerEventArgs    | Called when a Discord server goes down.  |
+--------------------+--------------------+------------------------------------------+
| ServerAvailable    | ServerEventArgs    |Called when a Discord server goes back up.|
+--------------------+--------------------+------------------------------------------+
| UserJoined         | UserEventArgs      | Called when a user joins a Channel.      |
+--------------------+--------------------+------------------------------------------+
| UserLeft           | UserEventArgs      | Called when a user leaves a Channel.     |
+--------------------+--------------------+------------------------------------------+
| UserUpdated        | UserEventArgs      | ---                                      |
+--------------------+--------------------+------------------------------------------+
| UserPresenceUpdated| UserEventArgs      | Called when a user's presence changes.   |
|                    |                    | (Here\\Away)                             |
+--------------------+--------------------+------------------------------------------+
| UserVoiceState     | UserEventArgs      | Called when a user's voice state changes.|
| Updated            |                    | (Muted\\Unmuted)                         |
+--------------------+--------------------+------------------------------------------+
|UserIsTypingUpdated | UserEventArgs      | Called when a user starts\\stops typing. |
+--------------------+--------------------+------------------------------------------+
| UserIsSpeaking     | UserEventArgs      | Called when a user's voice state changes.|
| Updated            |                    | (Speaking\\Not Speaking)                 |
+--------------------+--------------------+------------------------------------------+
| ProfileUpdated     | N/A                | Called when a user's profile changes.    |
+--------------------+--------------------+------------------------------------------+
Example
-------
   
.. literalinclude:: /samples/events.cs
   :language: csharp6
   :tab-width: 2