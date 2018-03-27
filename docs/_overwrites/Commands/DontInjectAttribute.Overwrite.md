---
uid: Discord.Commands.DontInjectAttribute
---

### Remarks

The attribute can be applied to a public settable property inside a
@Discord.Commands.ModuleBase based class. By applying this property,
the marked property will not be automatically injected of the
dependency. See [Dependency Injection](../../guides/commands/commands.md#dependency-injection)
to learn more.

### Example

```cs
public class MyModule : ModuleBase<SocketCommandContext>
{
    [DontInject]
    public MyService MyService { get; set; }

    public MyModule()
    {
        MyService = new MyService();
    }
}
```