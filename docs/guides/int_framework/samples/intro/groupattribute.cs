[SlashCommand("blep", "Send a random adorable animal photo")]
public async Task Blep([Choice("Dog", "dog"), Choice("Cat", "cat"), Choice("Guinea pig", "GuineaPig")] string animal)
{
    ...
}

// In most cases, you can use an enum to replace the separate choice attributes in a command.

public enum Animal
{
    Cat,
    Dog,
    // You can also use the ChoiceDisplay attribute to change how they appear in the choice menu.
    [ChoiceDisplay("Guinea pig")]
    GuineaPig
}

[SlashCommand("blep", "Send a random adorable animal photo")]
public async Task Blep(Animal animal)
{
    ...
}
```
