using System.Collections.Generic;
using Model = Discord.API.ApplicationCommandInteractionData;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents the base data tied with the <see cref="SocketCommandBase"/> interaction.
    /// </summary>
    public class SocketCommandBaseData<TOption> : SocketEntity<ulong>, IApplicationCommandInteractionData where TOption : IApplicationCommandInteractionDataOption
    {
        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <summary>
        ///     The <typeparamref name="TOption"/> received with this interaction.
        /// </summary>
        public virtual IReadOnlyCollection<TOption> Options { get; internal set; }

        internal readonly SocketResolvableData<Model> ResolvableData;

        private ApplicationCommandType Type { get; set; }

        internal SocketCommandBaseData(DiscordSocketClient client, Model model, ulong? guildId)
            : base(client, model.Id)
        {
            Type = model.Type;

            if (model.Resolved.IsSpecified)
            {
                ResolvableData = new SocketResolvableData<Model>(client, guildId, model);
            }
        }

        internal static SocketCommandBaseData Create(DiscordSocketClient client, Model model, ulong id, ulong? guildId)
        {
            var entity = new SocketCommandBaseData(client, model, guildId);
            entity.Update(model);
            return entity;
        }

        internal virtual void Update(Model model)
        {
            Name = model.Name;
        }

        IReadOnlyCollection<IApplicationCommandInteractionDataOption> IApplicationCommandInteractionData.Options
            => (IReadOnlyCollection<IApplicationCommandInteractionDataOption>)Options;
    }

    /// <summary>
    ///     Represents the base data tied with the <see cref="SocketCommandBase"/> interaction.
    /// </summary>
    public class SocketCommandBaseData : SocketCommandBaseData<IApplicationCommandInteractionDataOption>
    {
        internal SocketCommandBaseData(DiscordSocketClient client, Model model, ulong? guildId)
            : base(client, model, guildId) { }
    }
}
