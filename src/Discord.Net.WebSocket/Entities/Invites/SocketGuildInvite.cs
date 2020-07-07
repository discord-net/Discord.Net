using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using InviteUpdate = Discord.API.Gateway.InviteCreatedEvent;

namespace Discord.WebSocket
{
    /// <summary>
    /// Represents a guild invite
    /// </summary>
    public class SocketGuildInvite : SocketEntity<string>, ISocketInvite
    {
        public string Code { get; private set; }
        public string Url => $"{DiscordConfig.InviteUrl}{Code}";
        public SocketGuildChannel Channel { get; private set; }
        public SocketGuild Guild { get; private set; }
        /// <summary>
        /// Gets the unique invite code
        /// <returns>
        ///     Returns the unique invite code
        /// </returns>
        /// </summary>
        public string Id => Code;
        /// <summary>
        /// Gets the user who created the invite
        /// <returns>
        ///     Returns the user who created the invite
        /// </returns>
        /// </summary>
        public SocketGuildUser Inviter { get; private set; }
        /// <summary>
        /// Gets the maximum number of times the invite can be used, if there is no limit then the value will be 0
        /// <returns>
        ///     Returns the maximum number of times the invite can be used,  if there is no limit then the value will be 0
        /// </returns>
        /// </summary>
        public int? MaxUses { get; private set; }
        /// <summary>
        /// Gets whether or not the invite is temporary (invited users will be kicked on disconnect unless they're assigned a role)
        /// <returns>
        ///     Returns whether or not the invite is temporary (invited users will be kicked on disconnect unless they're assigned a role)
        /// </returns>
        /// </summary>
        public bool Temporary { get; private set; }
        /// <summary>
        /// Gets the time at which the invite was created
        /// <returns>
        ///     Returns the time at which the invite was created
        /// </returns>
        /// </summary>
        public DateTimeOffset? CreatedAt { get; private set; }
        /// <summary>
        /// Gets how long the invite is valid for 
        /// <returns>
        ///     Returns how long the invite is valid for (in seconds)
        /// </returns>
        /// </summary>
        public TimeSpan? MaxAge { get; private set; }

        internal SocketGuildInvite(DiscordSocketClient _client, SocketGuild guild, SocketGuildChannel channel, string inviteCode, RestInviteMetadata rest) : base(_client, inviteCode)
        {
            Code = inviteCode;
            Guild = guild;
            Channel = channel;
            CreatedAt = rest.CreatedAt;
            Temporary = rest.IsTemporary;
            MaxUses = rest.MaxUses;
            Inviter = guild.GetUser(rest.Inviter.Id);
            if (rest.MaxAge.HasValue)
                MaxAge = TimeSpan.FromSeconds(rest.MaxAge.Value);
        }
        internal SocketGuildInvite(DiscordSocketClient _client, SocketGuild guild, SocketGuildChannel channel, string inviteCode, InviteUpdate Update) : base(_client, inviteCode)
        {
            Code = inviteCode;
            Guild = guild;
            Channel = channel;

            if (Update.RawTimestamp.IsSpecified)
                CreatedAt = Update.RawTimestamp.Value;
            else
                CreatedAt = DateTimeOffset.Now;

            if (Update.inviter.IsSpecified)
                Inviter = guild.GetUser(Update.inviter.Value.Id);

            Temporary = Update.TempInvite;
            MaxUses = Update.MaxUsers;
            MaxAge = TimeSpan.FromSeconds(Update.RawAge);
        }
        internal static SocketGuildInvite Create(DiscordSocketClient _client, SocketGuild guild, SocketGuildChannel channel, string inviteCode, InviteUpdate Update)
        {
            var invite = new SocketGuildInvite(_client, guild, channel, inviteCode, Update);
            return invite;
        }
        internal static SocketGuildInvite CreateFromRest(DiscordSocketClient _client, SocketGuild guild, SocketGuildChannel channel, string inviteCode, RestInviteMetadata rest)
        {
            var invite = new SocketGuildInvite(_client, guild, channel, inviteCode, rest);
            return invite;
        }
        /// <summary>
        /// Deletes the invite
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task DeleteAsync(RequestOptions options = null)
            => SocketInviteHelper.DeleteAsync(this, Discord, options);
    }
}
