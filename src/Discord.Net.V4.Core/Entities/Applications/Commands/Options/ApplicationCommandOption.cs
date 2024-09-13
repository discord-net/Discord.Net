using System.Collections.Frozen;
using System.Globalization;
using Discord.Models;

namespace Discord;

public class ApplicationCommandOption :
    IEntityOf<IApplicationCommandOptionModel>
{
    public ApplicationCommandOptionType Type => (ApplicationCommandOptionType) Model.Type;

    public string Name => Model.Name;
    public string Description => Model.Description;

    public IReadOnlyDictionary<CultureInfo, string> NameLocalizations { get; }
    public IReadOnlyDictionary<CultureInfo, string> DescriptionLocalizations { get; }

    public IDiscordClient Client { get; }

    protected virtual IApplicationCommandOptionModel Model => _model;

    private readonly IApplicationCommandOptionModel _model;

    protected ApplicationCommandOption(
        IDiscordClient client,
        IApplicationCommandOptionModel model)
    {
        Client = client;
        _model = model;

        NameLocalizations =
            model
                .NameLocalizations?
                .ToFrozenDictionary(
                    x => CultureInfo.GetCultureInfo(x.Key),
                    x => x.Value
                )
            ?? FrozenDictionary<CultureInfo, string>.Empty;

        DescriptionLocalizations =
            model
                .DescriptionLocalizations?
                .ToFrozenDictionary(
                    x => CultureInfo.GetCultureInfo(x.Key),
                    x => x.Value
                )
            ?? FrozenDictionary<CultureInfo, string>.Empty;
    }

    public static ApplicationCommandOption Construct(
        IDiscordClient client,
        IApplicationCommandOptionModel model)
    {
        return model switch
        {
            IAttachmentApplicationCommandOptionModel attachment => new AttachmentApplicationCommandOption(
                client,
                attachment
            ),
            IBooleanApplicationCommandOptionModel boolean => new BooleanApplicationCommandOption(client, boolean),
            IChannelApplicationCommandOptionModel channel => new ChannelApplicationCommandOption(client, channel),
            IIntegerApplicationCommandOptionModel integer => new IntegerApplicationCommandOption(client, integer),
            IMentionableApplicationCommandOptionModel mentionable => new MentionableApplicationCommandOption(
                client,
                mentionable
            ),
            INumberApplicationCommandOptionModel number => new NumberApplicationCommandOption(client, number),
            IRoleApplicationCommandOptionModel role => new RoleApplicationCommandOption(client, role),
            IStringApplicationCommandOptionModel str => new StringApplicationCommandOption(client, str),
            ISubCommandApplicationCommandOptionModel subCommand => new SubCommandApplicationCommandOption(
                client,
                subCommand
            ),
            ISubCommandGroupApplicationCommandOptionModel subCommandGroup 
                => new SubCommandGroupApplicationCommandOption(client, subCommandGroup),
            IUserApplicationCommandOptionModel user => new UserApplicationCommandOption(client, user),
            _ => new ApplicationCommandOption(client, model)
        };
    }

    public virtual IApplicationCommandOptionModel GetModel() => Model;
}