using Discord.Interactions;

namespace InteractionFramework;

public enum ExampleEnum
{
    First,
    Second,
    Third,
    Fourth,
    [ChoiceDisplay("Twenty First")]
    TwentyFirst
}
