namespace Discord;

public readonly struct IntegrationApplication(
    ulong id,
    string name,
    string? icon,
    string description,
    IUserActor? bot
) :
    IIdentifiable<ulong>,
    IModelConstructable<IntegrationApplication, Models.IApplicationModel>
{
    public readonly ulong Id = id;
    public readonly string Name = name;
    public readonly string? Icon = icon;
    public readonly string Description = description;
    public readonly IUserActor? Bot = bot;

    public static IntegrationApplication Construct(IDiscordClient client, Models.IApplicationModel model,
        IUserActor? bot)
    {
        return new(
            model.Id,
            model.Name,
            model.Icon,
            model.Description,
            bot ?? (model.BotId.HasValue
                ? client.User(model.BotId.Value)
                : null)
        );
    }

    public static IntegrationApplication Construct(IDiscordClient client, Models.IApplicationModel model) =>
        new(
            model.Id,
            model.Name,
            model.Icon,
            model.Description,
            model.BotId.HasValue
                ? client.User(model.BotId.Value)
                : null
        );

    ulong IIdentifiable<ulong>.Id => Id;
    IdentityDetail IIdentifiable<ulong>.Detail => IdentityDetail.Entity;
}
