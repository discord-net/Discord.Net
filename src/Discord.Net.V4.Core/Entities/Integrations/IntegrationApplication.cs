namespace Discord;

public readonly struct IntegrationApplication(ulong id, string name, string? icon, string description, IUser? bot)
    :
        IIdentifiable<ulong>,
        IConstructable<IntegrationApplication, Models.Json.IntegrationApplication>
{
    public readonly ulong Id = id;
    public readonly string Name = name;
    public readonly string? Icon = icon;
    public readonly string Description = description;
    public readonly IUser? Bot = bot;


    public static IntegrationApplication Construct(IDiscordClient client, Models.Json.IntegrationApplication model)
    {
        return new IntegrationApplication(
            model.Id,
            model.Name,
            model.Icon,
            model.Description,
            ~model.Bot.Map(client.CreateEntity)
        );
    }

    ulong IIdentifiable<ulong>.Id => Id;
}
