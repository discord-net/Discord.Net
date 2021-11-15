using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Interaction;

namespace Discord.Rest
{
    public class RestPingInteraction : RestInteraction, IDiscordInteraction
    {
        internal override bool _hasResponded { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public RestPingInteraction(BaseDiscordClient client, ulong id)
            : base(client, id)
        {
        }

        internal static new async Task<RestPingInteraction> CreateAsync(DiscordRestClient client, Model model)
        {
            var entity = new RestPingInteraction(client, model.Id);
            await entity.UpdateAsync(client, model);
            return entity;
        }

        public string AcknowledgePing()
        {
            var model = new API.InteractionResponse()
            {
                Type = InteractionResponseType.Pong
            };

            return SerializePayload(model);
        }

        public override string Defer(bool ephemeral = false, RequestOptions options = null) => throw new NotSupportedException();
        public override Task<RestFollowupMessage> FollowupAsync(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null) => throw new NotSupportedException();
        public override Task<RestFollowupMessage> FollowupWithFileAsync(Stream fileStream, string fileName, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null) => throw new NotSupportedException();
        public override Task<RestFollowupMessage> FollowupWithFileAsync(string filePath, string text = null, string fileName = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null) => throw new NotSupportedException();
        public override string Respond(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null) => throw new NotSupportedException();
    }
}
