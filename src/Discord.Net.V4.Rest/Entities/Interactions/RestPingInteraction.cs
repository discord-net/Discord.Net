using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Interaction;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based ping interaction.
    /// </summary>
    public class RestPingInteraction : RestInteraction, IDiscordInteraction
    {
        internal RestPingInteraction(BaseDiscordClient client, ulong id)
            : base(client, id)
        {
        }

        internal static new async Task<RestPingInteraction> CreateAsync(DiscordRestClient client, Model model, bool doApiCall)
        {
            var entity = new RestPingInteraction(client, model.Id);
            await entity.UpdateAsync(client, model, doApiCall);
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
        public override string RespondWithModal(Modal modal, RequestOptions options = null) => throw new NotSupportedException();
        public override string Respond(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null) => throw new NotSupportedException();
        public override Task<RestFollowupMessage> FollowupAsync(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null) => throw new NotSupportedException();
        public override Task<RestFollowupMessage> FollowupWithFileAsync(Stream fileStream, string fileName, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null) => throw new NotSupportedException();
        public override Task<RestFollowupMessage> FollowupWithFileAsync(string filePath, string fileName = null, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null) => throw new NotSupportedException();
        public override Task<RestFollowupMessage> FollowupWithFileAsync(FileAttachment attachment, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null) => throw new NotSupportedException();
        public override Task<RestFollowupMessage> FollowupWithFilesAsync(IEnumerable<FileAttachment> attachments, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null) => throw new NotSupportedException();
    }
}
