Events
======

Usage
-----
Events in Discord.NET are raised using the Event system in c#. Most events are raised on the ``DiscordClient`` class.

Most events in Discord.NET explain theirselves by their name.

Messages
--------

The Four Message Events (MessageReceived, Updated, Deleted, and Acknowledged) are raised when a message has been modified/created.

Example of MessageReceived:

.. code-block:: c#

    // (Preface: Echo Bots are discouraged, make sure your bot is not running in a public server if you use them)

    // Hook into the MessageReceived event using a Lambda
    _client.MessageReceived += (s, e) => {
            // Check to make sure that the bot is not the author
            if (!e.Message.IsAuthor)
                // Echo the message back to the channel
                e.Channel.SendMessage(e.Message);
    };

Users
-----

There are Six User Events:

UserBanned: A user has been banned from a Server
UserUnbanned: A user was unbanned
UserJoined: A user joins a server
UserLeft: A user left (or was kicked) from a Server
UserIsTyping: A user in a channel starts typing
UserUpdated: A user object was updated. (caused by a presence update, role/permission change, or a voice state update)

.. note::
    UserUpdated Events include a ``User`` object for Before and After the change.
    When accessing the User, you should only use ``e.Before`` if comparing changes, otherwise use ``e.After``

Examples:

.. code-block:: c#

    // Register a Hook into the UserBanned event using a Lambda
    _client.UserBanned += (s, e) => {
        // Create a Channel object by searching for a channel named '#logs' on the server the ban occurred in.
        var logChannel = e.Server.FindChannels("logs").FirstOrDefault();
        // Send a message to the server's log channel, stating that a user was banned.
        logChannel.SendMessage($"User Banned: {e.User.Name}")
    };

    // Register a Hook into the UserUpdated event using a Lambda
    _client.UserUpdated += (s, e) => {
        // Check that the user is in a Voice channel
        if (e.After.VoiceChannel == null) return;

        // See if they changed Voice channels
        if (e.Before.VoiceChannel == e.After.VoiceChannel) return;

        // do something...
    };

Connection States
-----------------

Connection Events will be raised when the Connection State of your client changes.

.. warning::
    You should not use DiscordClient.Connected to run code when your client first connects to Discord.
    If you lose connection and automatically reconnect, this code will be ran again, which may lead to unexpected behavior.
