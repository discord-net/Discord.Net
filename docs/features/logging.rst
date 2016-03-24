Logging
=======

Discord.Net will log all of its events/exceptions using a built-in LogManager.
This LogManager can be accessed through ``DiscordClient.Log``

Usage
-----

To handle Log Messages through Discord.Net's Logger, you must hook into the ``Log.Message<LogMessageEventArgs>`` Event.

The LogManager does not provide a string-based result for the message, you must put your own message format together using the data provided through LogMessageEventArgs
See the Example for a snippet of logging.

Logging Your Own Data
---------------------

The LogManager included in Discord.Net can also be used to log your own messages.

You can use ``DiscordClient.Log.Log(LogSeverity, Source, Message, [Exception])``, or one of the shortcut helpers, to log data.

Example:

.. code-block:: csharp6

    _client.MessageReceived += async (s, e) {
        // Log a new Message with Severity Info, Sourced from 'MessageReceived', with the Message Contents.
        _client.Log.Info("MessageReceived", e.Message.Text, null);
    };


.. warning::

    Starting in Discord.Net 1.0, you will not be able to log your own messages. You will need to create your own Logging manager, or use a pre-existing one.

Example
-------

.. literalinclude:: /samples/logging.cs
   :language: csharp6
   :tab-width: 2
