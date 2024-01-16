---
uid: Guides.GuildEvents.Creating
title: Creating Guild Events
---

# Creating guild events

You can create new guild events by using the `CreateEventAsync` function on a guild.

### Parameters

| Name          | Type                              | Summary                                                                      |
| ------------- | --------------------------------- | ---------------------------------------------------------------------------- |
| name          | `string`                          | Sets the name of the event.                                                  |
| startTime     | `DateTimeOffset`                  | Sets the start time of the event.                                            |
| type          | `GuildScheduledEventType`         | Sets the type of the event.                                                  |
| privacyLevel? | `GuildScheduledEventPrivacyLevel` | Sets the privacy level of the event                                          |
| description?  | `string`                          | Sets the description of the event.                                           |
| endTime?      | `DateTimeOffset?`                 | Sets the end time of the event.                                              |
| channelId?    | `ulong?`                          | Sets the channel id of the event, only valid on stage or voice channel types |
| location?     | `string`                          | Sets the location of the event, only valid on external types                 |

Lets create a basic test event.

```cs
var guild = client.GetGuild(guildId);

var guildEvent = await guild.CreateEventAsync("test event", DateTimeOffset.UtcNow.AddDays(1),  GuildScheduledEventType.External, endTime: DateTimeOffset.UtcNow.AddDays(2), location: "Space");
```

This code will create an event that lasts a day and starts tomorrow. It will be an external event that's in space.
