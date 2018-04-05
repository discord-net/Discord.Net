# Basic Concepts / Getting Started

## How do I add my bot to my server/guild?

You can do so by using the [permission calculator] provided
by FiniteReality.
This tool allows you to set the permissions that the bot will be
added with, and invite the bot into your guild. With this method,
bots will also be assigned their own special roles that normal users
cannot use; this is what we call a `Managed` role, and this is a much
safer method of permission management than to create a role that any
users can be assigned to.

[permission calculator]: https://finitereality.github.io/permissions-calculator

## What is a token?

A token is a credential used to log into an account. This information
should be kept **private** and for your eyes only. Anyone with your
token can log into your account. This applies to both user and bot
accounts. That also means that you should never ever hardcode your
token or add it into source control, as your identity may be stolen
by scrape bots on the internet that scours through constantly to
obtain a token.

## What is a client/user/object ID?

Each user and object on Discord has its own snowflake ID generated
based on various conditions.

![Snowflake Generation](images/snowflake.png)

The ID can be seen by anyone; it is public. It is merely used to
identify an object in the Discord ecosystem. Many things in the
Discord ecosystem require an ID to retrieve or identify the said
object.

There are 2 common ways to obtain the said ID.

### [Discord Developer Mode](#tab/dev-mode)

By enabling the developer mode you can right click on most objects
to obtain their snowflake IDs (please note that this may not apply to
all objects, such as role IDs, or DM channel IDs).

![Developer Mode](images/dev-mode.png)

### [Escape Character](#tab/escape-char)

You can escape an object by using `\` in front the object in the 
Discord client. For example, when you do `\@Example#1234` in chat,
it will return the user ID of the aforementioned user.

***

## How do I get the role ID?

> [!WARNING]
> Right-clicking on the role and copying the ID will **not** work.
> This will only copy the message ID.

Several common ways to do this:

1. Make the role mentionable and mention the role, and escape it
  using the `\` character in front.
2. Inspect the roles collection within the guild via your debugger.