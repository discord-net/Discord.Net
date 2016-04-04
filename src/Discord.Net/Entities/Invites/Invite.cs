using Discord.API.Rest;
using System.Threading.Tasks;
using Model = Discord.API.Invite;

namespace Discord
{
    public class PublicInvite : IInvite, IEntity<string>
    {
        /// <inheritdoc />
        public string Code { get; }
        /// <inheritdoc />
        string IEntity<string>.Id => Code;
        /// <inheritdoc />
        public DiscordClient Discord { get; }

        /// <inheritdoc />
        public InviteGuild Guild { get; private set; }
        /// <inheritdoc />
        public InviteChannel Channel { get; private set; }
        /// <inheritdoc />
        public string XkcdCode { get; private set; }

        /// <inheritdoc />
        public string Url => $"{DiscordConfig.InviteUrl}/{XkcdCode ?? Code}";
        /// <inheritdoc />
        public string XkcdUrl => XkcdCode != null ? $"{DiscordConfig.InviteUrl}/{XkcdCode}" : null;

        internal PublicInvite(string code, DiscordClient client)
        {
            Code = code;
            Discord = client;
        }

        internal void Update(Model model)
        {
            XkcdCode = model.XkcdPass;
            Guild = new InviteGuild(model.Guild.Id, model.Guild.Name);
            Channel = new InviteChannel(model.Channel.Id, model.Channel.Name);
        }

        /// <inheritdoc />
        public async Task Update()
            => Update(await Discord.RestClient.Send(new GetInviteRequest(Code)).ConfigureAwait(false));

        /// <inheritdoc />
        public override string ToString() => Url;
    }
}
