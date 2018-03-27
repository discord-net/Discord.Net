---
uid: Discord.Commands.DontAutoLoadAttribute
---

### Remarks

The attribute can be applied to a public class that inherits
@Discord.Commands.ModuleBase. By applying this attribute,
@Discord.Commands.CommandService.AddModulesAsync* will not discover and
add the marked module to the CommandService.

### Example

```cs
[DontAutoLoad]
public class MyModule : ModuleBase<SocketCommandContext>
{
    // ...
}
```