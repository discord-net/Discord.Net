---
uid: Discord.IEmote
seealso:
    - linkId: Discord.Emote
    - linkId: Discord.Emoji
    - linkId: Discord.GuildEmote
    - linkId: Discord.IUserMessage
remarks: *content
---

This interface is often used with reactions. It can represent an
unicode-based @Discord.Emoji, or a custom @Discord.Emote.

---
uid: Discord.Emote
seealso:
    - linkId: Discord.IEmote
    - linkId: Discord.GuildEmote
    - linkId: Discord.Emoji
    - linkId: Discord.IUserMessage
remarks: *content
---

> [!NOTE]
> A valid @Discord.Emote format is `<:emoteName:emoteId>`. This can be
> obtained by escaping with a `\` in front of the emote using the
> Discord chat client.

This class represents a custom emoji. This type of emoji can be
created via the @Discord.Emote.Parse* or @Discord.Emote.TryParse*
method.

---
uid: Discord.Emoji
seealso:
    - linkId: Discord.Emote
    - linkId: Discord.GuildEmote
    - linkId: Discord.Emoji
    - linkId: Discord.IUserMessage
remarks: *content
---

> [!NOTE]
> A valid @Discord.Emoji format is Unicode-based. This means only
> something like `🙃` or `\U0001f643` would work, instead of
> `:upside_down:`.
>
> A Unicode-based emoji can be obtained by escaping with a `\` in
> front of the emote using the Discord chat client or by looking up on
> [Emojipedia](https://emojipedia.org).

This class represents a standard Unicode-based emoji. This type of emoji
can be created by passing the Unicode into the constructor.

---
uid: Discord.IEmote
example: [*content]
---

[!include[Example Section](IEmote.Inclusion.md)]

---
uid: Discord.Emoji
example: [*content]
---

[!include[Example Section](IEmote.Inclusion.md)]

---
uid: Discord.Emote
example: [*content]
---

[!include[Example Section](IEmote.Inclusion.md)]

---
uid: Discord.GuildEmote
example: [*content]
---

[!include[Example Section](IEmote.Inclusion.md)]