---
uid: Discord.Commands.DontInjectAttribute
remarks: *content
---

The attribute can be applied to a public settable property inside a
@Discord.Commands.ModuleBase based class. By applying this attribute,
the marked property will not be automatically injected of the
dependency. See @Guides.Commands.DI to learn more.

---
uid: Discord.Commands.DontInjectAttribute
example: [*content]
---

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