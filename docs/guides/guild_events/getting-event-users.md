---
uid: Guides.GuildEvents.GettingUsers
title: Getting Guild Event Users
---

# Getting Event Users

You can get a collection of users who are currently interested in the event by calling `GetUsersAsync`. This method works like any other get users method as in it returns an async enumerable. This method also supports pagination by user id.

```cs
// get all users and flatten the result into one collection.
var users = await event.GetUsersAsync().FlattenAsync();

// get users around the 613425648685547541 id.
var aroundUsers = await event.GetUsersAsync(613425648685547541, Direction.Around).FlattenAsync();
```
