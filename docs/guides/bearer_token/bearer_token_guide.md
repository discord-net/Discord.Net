---
uid: Guides.BearerToken
title: Working with Bearer token
---

# Working with Bearer token

Some endpoints in Discord API require a Bearer token, which can be obtained through [OAuth2 flow](https://discord.com/developers/docs/topics/oauth2). Discord.Net allows you to interact with these endpoints using the [DiscordRestClient].

## Initializing a new instance of the client
[!code-csharp[Initialize DiscordRestClient](samples/rest_client_init.cs)]

## Getting current user

The [DiscordRestClient] gets the current user when `LoginAsync()` is called. The user object can be found in the `CurrentUser` property.

If you need to fetch the user again, the `GetCurrentUserAsync()` method can be used.

[!code-csharp[Get current user](samples/current_user.cs)]

> [!NOTE]
> Some properties might be `null` depending on which scopes users authorized your app with.
> For example: `email` scope is required to fetch current user's email address.

## Fetching current user's guilds

The `GetGuildSummariesAsync()` method is used to fetch current user's guilds. Since it returns an `IAsyncEnumerable` you need to call `FlattenAsync()` to get a plain `IEnumerable` containing [RestUserGuild] objects.

[!code-csharp[Get current user's guilds](samples/current_user_guilds.cs)]

> [!WARNING]
> This method requires `guilds` scope

## Fetching current user's guild member object

To fetch the current user's guild member object, the `GetCurrentUserGuildMemberAsync()` method can be used. 

[!code-csharp[Get current user's guild member](samples/current_user_guild_member.cs)]

> [!WARNING]
> This method requires `guilds.members.read` scope

## Get user connections

The `GetConnectionsAsync` method can be used to fetch current user's connections to other platforms.

[!code-csharp[Get current user's connections](samples/current_user_connections.cs)]

> [!WARNING]
> This method requires `connections` scope

## Application role connection

In addition to previous features, Discord.Net supports fetching & updating user's application role connection metadata values. `GetUserApplicationRoleConnectionAsync()` returns a [RoleConnection] object of the current user for the given application id.

The `ModifyUserApplicationRoleConnectionAsync()` method is used to update current user's role connection metadata values. A new set of values can be created with [RoleConnectionProperties] object.

[!code-csharp[Get current user's connections](samples/app_role_connection.cs)]

> [!WARNING]
> This method requires `role_connections.write` scope



[DiscordRestClient]: xref:Discord.Rest.DiscordRestClient
[RestUserGuild]: xref:Discord.Rest.RestUserGuild
[RoleConnection]: xref:Discord.RoleConnection
[RoleConnectionProperties]: xref:Discord.RoleConnectionProperties
