Modes
======

Usage
-----
Using this library requires you to state the intention of the program using it. 
By default, the library assumes your application is a bot or otherwise automated program, and locks access to certain client-only features.
As we approach the official API, Discord will be creating a divide between bots and clients, so it's important to use the mode appropriate for your program to minimize breaking changes!

.. warning::
  This is not a complete list, new features will be added in the future.

Client-Only Features
--------------------

- Message Acknowledgement (Message.Acknowledge(), DiscordClient.MessageAcknowledged)
- Message Importing/Exporting
- Message Read States

Bot-Only Features
-----------------

- Currently, None
