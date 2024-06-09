namespace Discord;

public readonly struct PropertiesOrModel<T, U>
    where T : IEntityProperties<U>
{
    public readonly U Model;

    public PropertiesOrModel(U model)
    {
        Model = model;
    }

    public PropertiesOrModel(T properties)
    {
        Model = properties.ToApiModel();
    }

    public static implicit operator PropertiesOrModel<T, U>(T properties) => new(properties);
    public static implicit operator PropertiesOrModel<T, U>(U model) => new(model);
}
