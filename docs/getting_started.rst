Getting Started
===============

Requirements
------------

Discord.Net currently requires logging in with a claimed account - anonymous logins are not supported. You can `register for a Discord account here`.

New accounts are also useless when not connected to a server, so you should create an invite code for whatever server you intend to test on using the official Discord client.

.. _register for a Discord account here: https://discordapp.com/register

Installation
------------

You can get Discord.Net from NuGet:

* `Discord.Net`_
* `Discord.Net.Commands`_

You can also pull the latest source from `GitHub`_ 

.. _Discord.Net: https://discordapp.com/register
.. _Discord.Net.Commands: https://discordapp.com/register
.. _GitHub: https://github.com/RogueException/Discord.Net/>

Async
-----

Discord.Net uses C# tasks extensively - nearly all operations return one. It is highly recommended that these tasks be awaited whenever possible.
To do so requires the calling method be marked as async, which can be problematic in a console application. An example of how to get around this is provided below.

For more information, go to `MSDN's Await-Async section`_.

.. _MSDN's Await-Async section: https://msdn.microsoft.com/en-us/library/hh191443.aspx

Example
-------
   
.. literalinclude:: samples/getting_started.cs
   :language: csharp6
   :tab-width: 2