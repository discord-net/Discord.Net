---
uid: Discord.Commands.OverrideTypeReaderAttribute
remarks: *content
---

This attribute is used to override a command parameter's type reading
behaviour. This may be useful when you have multiple custom
@Discord.Commands.TypeReader and would like to specify one.

---
uid: Discord.Commands.OverrideTypeReaderAttribute
examples: [*content]
---

The following example will override the @Discord.Commands.TypeReader
of @Discord.IUser to `MyUserTypeReader`.

```cs
public async Task PrintUserAsync(
    [OverrideTypeReader(typeof(MyUserTypeReader))] IUser user)
{
    //...
}
```