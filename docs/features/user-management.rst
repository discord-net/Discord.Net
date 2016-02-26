User Management
===============

Banning
-------

To ban a user, invoke the Ban function on a Server object.

.. code-block:: c#

    _server.Ban(_user, 30);

The pruneDays parameter, which defaults to 0, will remove all messages from a user dating back to the specified amount of days.

Kicking
-------

To kick a user, invoke the Kick function on the User.

.. code-block:: c#

    _user.Kick();
