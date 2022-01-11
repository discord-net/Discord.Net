// Sometimes injecting dependencies automatically with the provided
// methods in the prior example may not be desired.

// You may explicitly tell Discord.Net to **not** inject the properties
// by either...
// restricting the access modifier
// -or-
// applying DontInjectAttribute to the property

// Restricting the access modifier of the property
public class ImageModule : ModuleBase<SocketCommandContext>
{
    public ImageService ImageService { get; }
    public ImageModule()
    {
        ImageService = new ImageService();
    }
}

// Applying DontInjectAttribute
public class ImageModule : ModuleBase<SocketCommandContext>
{
    [DontInject]
    public ImageService ImageService { get; set; }
    public ImageModule()
    {
        ImageService = new ImageService();
    }
}
