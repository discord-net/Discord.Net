using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.ApplicationCommandInteractionData;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents the data tied with the <see cref="SocketSlashCommand"/> interaction.
    /// </summary>
    public class SocketSlashCommandData : SocketCommandBaseData<SocketSlashCommandDataOption>, IDiscordInteractionData
    {
        internal SocketSlashCommandData(DiscordSocketClient client, Model model, ulong? guildId)
            : base(client, model, guildId) { }

        internal static SocketSlashCommandData Create(DiscordSocketClient client, Model model, ulong? guildId)
        {
            var entity = new SocketSlashCommandData(client, model, guildId);
            entity.Update(model);
            return entity;
        }
        internal override void Update(Model model)
        {
            base.Update(model);

            Options = model.Options.IsSpecified
                ? model.Options.Value.Select(x => new SocketSlashCommandDataOption(this, x)).ToImmutableArray()
                : ImmutableArray.Create<SocketSlashCommandDataOption>();
        }
    }
}
