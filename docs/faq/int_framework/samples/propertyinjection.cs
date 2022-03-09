public class MyModule
{
    // Intended.
    public InteractionService Service { get; set; }

    // Will not work. A private setter cannot be accessed by the serviceprovider.
    private InteractionService Service { get; private set; }
}
