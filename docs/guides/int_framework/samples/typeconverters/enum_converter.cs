internal sealed class EnumConverter<T> : TypeConverter<T> where T : struct, Enum
{
    public override ApplicationCommandOptionType GetDiscordType() => ApplicationCommandOptionType.String;

    public override Task<TypeConverterResult> ReadAsync(IInteractionCommandContext context, SocketSlashCommandDataOption option, IServiceProvider services)
    {
        if (Enum.TryParse<T>((string)option.Value, out var result))
            return Task.FromResult(TypeConverterResult.FromSuccess(result));
        else
            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"Value {option.Value} cannot be converted to {nameof(T)}"));
    }

    public override void Write(ApplicationCommandOptionProperties properties, IParameterInfo parameterInfo)
    {
        var names = Enum.GetNames(typeof(T));
        if (names.Length <= 25)
        {
            var choices = new List<ApplicationCommandOptionChoiceProperties>();

            foreach (var name in names)
                choices.Add(new ApplicationCommandOptionChoiceProperties
                {
                    Name = name,
                    Value = name
                });

            properties.Choices = choices;
        }
    }
}
