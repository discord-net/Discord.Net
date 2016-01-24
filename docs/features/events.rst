Events
======

Usage
-----
Messages from the Discord server are exposed via events on the DiscordClient class and follow the standard EventHandler<EventArgs> C# pattern. 

.. warning::
    Note that all synchronous code in an event handler will run on the gateway socket's thread and should be handled as quickly as possible. 
    Using the async-await pattern to let the thread continue immediately is recommended and is demonstrated in the examples below.

Connection State
----------------

Connection Events will be raised when the Connection State of your client changes.

.. warning::
    You should not use DiscordClient.Connected to run code when your client first connects to Discord.
    If you lose connection and automatically reconnect, this code will be ran again, which may lead to unexpected behavior.
    
Messages
--------

- MessageReceived, MessageUpdated and MessageDeleted are raised when a new message arrives, an existing one has been updated (by the user, or by Discord itself), or deleted.
- MessageAcknowledged is only triggered in client mode, and occurs when a message is read on another device logged-in with your account.

Example of MessageReceived:

.. code-block:: c#

    // (Preface: Echo Bots are discouraged, make sure your bot is not running in a public server if you use them)

    // Hook into the MessageReceived event using a Lambda
    _client.MessageReceived += async (s, e) => {
            // Check to make sure that the bot is not the author
            if (!e.Message.IsAuthor)
                // Echo the message back to the channel
                await e.Channel.SendMessage(e.Message);
    };

Users
-----

There are several user events:

- UserBanned: A user has been banned from a server.
- UserUnbanned: A user was unbanned.
- UserJoined: A user joins a server.
- UserLeft: A user left (or was kicked from) a server.
- UserIsTyping: A user in a channel starts typing.
- UserUpdated: A user object was updated (presence update, role/permission change, or a voice state update).

.. note::
    UserUpdated Events include a ``User`` object for Before and After the change.
    When accessing the User, you should only use ``e.Before`` if comparing changes, otherwise use ``e.After``

Examples:

.. code-block:: c#

    // Register a Hook into the UserBanned event using a Lambda
    _client.UserBanned += async (s, e) => {
        // Create a Channel object by searching for a channel named '#logs' on the server the ban occurred in.
        var logChannel = e.Server.FindChannels("logs").FirstOrDefault();
        // Send a message to the server's log channel, stating that a user was banned.
        await logChannel.SendMessage($"User Banned: {e.User.Name}");
    };

    // Register a Hook into the UserUpdated event using a Lambda
    _client.UserUpdated += async (s, e) => {
        // Check that the user is in a Voice channel
        if (e.After.VoiceChannel == null) return;

        // See if they changed Voice channels
        if (e.Before.VoiceChannel == e.After.VoiceChannel) return;

        await logChannel.SendMessage($"User {e.After.Name} changed voice channels!");
    };
