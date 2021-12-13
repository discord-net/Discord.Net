[SlashCommand("blep", "Send a random adorable animal photo")]
public async Task Blep([Choice("Dog", "dog"), Choice("Cat", "cat"), Choice("Penguin", "penguin")] string animal)
{
    ...
}

// In most cases, you can use an enum to replace the seperate choice attributes in a command.

public enum Animal
{
    Cat,
    Dog,
    Penguin
}

[SlashCommand("blep", "Send a random adorable animal photo")]
public async Task Blep(Animal animal)
{
    ...
}
```
