using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.InviteEvent;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based invite.
    /// </summary>
    public class SocketInvite : SocketEntity<string>
    {
        /// <summary>
        ///     Gets the channel where this invite was created.
        /// </summary>
        public ISocketMessageChannel Channel { get; private set; }

        /// <summary>
        ///     Gets the channel ID where this invite was created.
        /// </summary>
        public ulong ChannelId { get; private set; }

        /// <summary>
        ///     Gets the guild where this invite was created.
        /// </summary>
        public IGuild Guild { get; private set; }

        /// <summary>
        ///     Gets the guild ID where this invite was created.
        /// </summary>
        public ulong GuildId { get; private set; }

        /// <summary>
        ///     Gets the unique identifier for this invite.
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        ///     Gets the user who created this invite.
        /// </summary>
        public Optional<SocketUser> Inviter { get; private set; }

        /// <summary>
        ///     Gets when this invite was created.
        /// </summary>
        public Optional<DateTimeOffset> CreatedAt { get; private set; }

        /// <summary>
        ///     Gets the time (in seconds) until the invite expires.
        /// </summary>
        public Optional<int> MaxAge { get; private set; }

        /// <summary>
        ///     Gets the max number of uses this invite may have.
        /// </summary>
        public Optional<int> MaxUses { get; private set; }

        /// <summary>
        ///     Gets the number of times this invite has been used.
        /// </summary>
        public Optional<int> Uses { get; set; }

        /// <summary>
        ///     Gets a value that indicates whether the invite is a temporary one.
        /// </summary>
        public Optional<bool> IsTemporary { get; private set; }

        internal SocketInvite(DiscordSocketClient discord, Model model)
            : base(discord, model.Code)
        {
        }

        internal static SocketInvite Create(DiscordSocketClient discord, Model model, SocketUser inviter, IGuild guild, ISocketMessageChannel channel)
        {
            var entity = new SocketInvite(discord, model);
            entity.Update(model, inviter, guild, channel);
            return entity;
        }

        internal static SocketInvite Create(DiscordSocketClient discord, Model model, IGuild guild, ISocketMessageChannel channel)
        {
            var entity = new SocketInvite(discord, model);
            entity.Update(model.Code, guild, channel);
            return entity;
        }

        internal void Update(Model model, SocketUser inviter, IGuild guild, ISocketMessageChannel channel)
        {
            Channel = channel;
            ChannelId = model.ChannelId;
            Guild = guild;
            GuildId = model.GuildId;
            Code = model.Code;
            Inviter = inviter;
            CreatedAt = model.CreatedAt.Value;
            MaxAge = model.MaxAge.Value;
            MaxUses = model.MaxUses.Value;
            Uses = model.Uses.Value;
            IsTemporary = model.Temporary;
        }

        internal void Update(string code, IGuild guild, ISocketMessageChannel channel)
        {
            Code = code;
            Guild = guild;
            GuildId = guild.Id;
            Channel = channel;
            ChannelId = channel.Id;
        }


    }
}
