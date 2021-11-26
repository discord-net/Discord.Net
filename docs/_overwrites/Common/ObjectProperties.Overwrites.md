---
uid: Discord.GuildChannelProperties
example: [*content]
---

The following example uses @Discord.IGuildChannel.ModifyAsync* to
apply changes specified in the properties,

```cs
var channel = _client.GetChannel(id) as IGuildChannel;
if (channel == null) return;

await channel.ModifyAsync(x =>
{
    x.Name = "new-name";
    x.Position = channel.Position - 1;
});
```

---
uid: Discord.TextChannelProperties
example: [*content]
---

The following example uses @Discord.ITextChannel.ModifyAsync* to
apply changes specified in the properties,

```cs
var channel = _client.GetChannel(id) as ITextChannel;
if (channel == null) return;

await channel.ModifyAsync(x =>
{
    x.Name = "cool-guys-only";
    x.Topic = "This channel is only for cool guys and adults!!!";
    x.Position = channel.Position - 1;
    x.IsNsfw = true;
});
```

---
uid: Discord.VoiceChannelProperties
example: [*content]
---

The following example uses @Discord.IVoiceChannel.ModifyAsync* to
apply changes specified in the properties,

```cs
var channel = _client.GetChannel(id) as IVoiceChannel;
if (channel == null) return;

await channel.ModifyAsync(x =>
{
    x.UserLimit = 5;
});
```

---
uid: Discord.EmoteProperties
example: [*content]
---

The following example uses @Discord.IGuild.ModifyEmoteAsync* to
apply changes specified in the properties,

```cs
await guild.ModifyEmoteAsync(x =>
{
    x.Name = "blobo";
});
```

---
uid: Discord.MessageProperties
example: [*content]
---

The following example uses @Discord.IUserMessage.ModifyAsync* to
apply changes specified in the properties,

```cs
var message = await channel.SendMessageAsync("boo");
await Task.Delay(TimeSpan.FromSeconds(1));
await message.ModifyAsync(x => x.Content = "boi");
```

---
uid: Discord.GuildProperties
example: [*content]
---

The following example uses @Discord.IGuild.ModifyAsync* to
apply changes specified in the properties,

```cs
var guild = _client.GetGuild(id);
if (guild == null) return;

await guild.ModifyAsync(x =>
{
    x.Name = "VERY Fast Discord Running at Incredible Hihg Speed";
});
```

---
uid: Discord.RoleProperties
example: [*content]
---

The following example uses @Discord.IRole.ModifyAsync* to
apply changes specified in the properties,

```cs
var role = guild.GetRole(id);
if (role == null) return;

await role.ModifyAsync(x =>
{
    x.Name = "cool boi";
    x.Color = Color.Gold;
    x.Hoist = true;
    x.Mentionable = true;
});
```

---
uid: Discord.GuildUserProperties
example: [*content]
---

The following example uses @Discord.IGuildUser.ModifyAsync* to
apply changes specified in the properties,

```cs
var user = guild.GetUser(id);
if (user == null) return;

await user.ModifyAsync(x =>
{
    x.Nickname = "I need healing";
});
```

---
uid: Discord.SelfUserProperties
example: [*content]
---

The following example uses @Discord.ISelfUser.ModifyAsync* to
apply changes specified in the properties,

```cs
await selfUser.ModifyAsync(x =>
{
    x.Username = "Mercy";
});
```

---
uid: Discord.WebhookProperties
example: [*content]
---

The following example uses @Discord.IWebhook.ModifyAsync* to
apply changes specified in the properties,

```cs
await webhook.ModifyAsync(x =>
{
    x.Name = "very fast fox";
    x.ChannelId = newChannelId;
});
```