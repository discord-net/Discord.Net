---
uid: Discord.Commands.DontAutoLoadAttribute
remarks: *content
---

The attribute can be applied to a public class that inherits
@Discord.Commands.ModuleBase. By applying this attribute,
@Discord.Commands.CommandService.AddModulesAsync* will not discover and
add the marked module to the CommandService.

---
uid: Discord.Commands.DontAutoLoadAttribute
example: [*content]
---

```cs
[DontAutoLoad]
public class MyModule : ModuleBase<SocketCommandContext>
{
    // ...
}
```