---
uid: Guides.GuildEvents.Modifying
title: Modifying Guild Events
---

# Modifying Events

You can modify events using the `ModifyAsync` method to modify the event. Here are the properties you can modify:

| Name         | Type                              | Description                                  |
| ------------ | --------------------------------- | -------------------------------------------- |
| ChannelId    | `ulong?`                          | Gets or sets the channel id of the event.    |
| string       | `string`                          | Gets or sets the location of this event.     |
| Name         | `string`                          | Gets or sets the name of the event.          |
| PrivacyLevel | `GuildScheduledEventPrivacyLevel` | Gets or sets the privacy level of the event. |
| StartTime    | `DateTimeOffset`                  | Gets or sets the start time of the event.    |
| EndTime      | `DateTimeOffset`                  | Gets or sets the end time of the event.      |
| Description  | `string`                          | Gets or sets the description of the event.   |
| Type         | `GuildScheduledEventType`         | Gets or sets the type of the event.          |
| Status       | `GuildScheduledEventStatus`       | Gets or sets the status of the event.        |

> [!NOTE]
> All of these properties are optional.
