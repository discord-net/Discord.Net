using Discord.API.Rest;
using Discord.Net;
using System;
using System.Net;
using System.Threading.Tasks;
using Model = Discord.API.InviteMetadata;

namespace Discord
{
    public class GuildInvite : IInvite, IEntity<string>
    {
        /// <inheritdoc />
        public string Code { get; }
        /// <summary> Gets the channel this invite is attached to. </summary>
        public GuildChannel Channel { get; }

        /// <inheritdoc />
        public string XkcdCode { get; private set; }
        /// <summary> Gets the time (in seconds) until the invite expires, or null if it never expires. </summary>
        public int? MaxAge { get; private set; }
        /// <summary> Gets the amount of times this invite has been used. </summary>
        public int Uses { get; private set; }
        /// <summary> Gets the max amount of times this invite may be used, or null if there is no limit. </summary>
        public int? MaxUses { get; private set; }
        /// <summary> Returns true if this invite has expired or been deleted. </summary>
        public bool IsRevoked { get; private set; }
        /// <summary> Returns true if a user accepting this invite will be kicked from the guild after closing their client. </summary>
        public bool IsTempMembership { get; private set; }

        /// <summary> Gets the guild this invite is attached to. </summary>
        public Guild Guild => Channel.Guild;
        /// <inheritdoc />
        public DiscordClient Discord => Guild.Discord;
        /// <inheritdoc />
        public string Url => $"{DiscordConfig.InviteUrl}/{Code}";
        /// <inheritdoc />
        public string XkcdUrl => XkcdCode != null ? $"{DiscordConfig.InviteUrl}/{XkcdCode}" : null;
        /// <inheritdoc />
        string IEntity<string>.Id => Code;
        /// <inheritdoc />
        InviteChannel IInvite.Channel => new InviteChannel(Channel.Id, Channel.Name);
        /// <inheritdoc />
        InviteGuild IInvite.Guild => new InviteGuild(Guild.Id, Guild.Name);

        internal GuildInvite(string code, GuildChannel channel)
        {
            Code = code;
            Channel = channel;
        }
        
        internal void Update(Model model)
        {
            XkcdCode = model.XkcdPass;
            IsRevoked = model.Revoked;
            IsTempMembership = model.Temporary;
            MaxAge = model.MaxAge != 0 ? model.MaxAge : (int?)null;
            MaxUses = model.MaxUses;
            Uses = model.Uses;
        }

        /// <inheritdoc />
        public Task Update() { throw new NotSupportedException(); } //TODO: Not supported yet

        /// <summary> Deletes this invite. </summary>
        public async Task Delete()
        {
            try { await Discord.RestClient.Send(new DeleteInviteRequest(Code)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }
    }
}
