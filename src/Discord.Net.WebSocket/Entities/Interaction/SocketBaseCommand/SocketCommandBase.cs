using Discord.Net.Rest;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel = Discord.API.ApplicationCommandInteractionData;
using Model = Discord.API.Interaction;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Base class for User, Message, and Slash command interactions.
    /// </summary>
    public class SocketCommandBase : SocketInteraction
    {
        /// <summary>
        ///     Gets the name of the invoked command.
        /// </summary>
        public string CommandName
            => Data.Name;

        /// <summary>
        ///     Gets the id of the invoked command.
        /// </summary>
        public ulong CommandId
            => Data.Id;

        /// <summary>
        ///     The data associated with this interaction.
        /// </summary>
        new internal SocketCommandBaseData Data { get; }

        internal SocketCommandBase(DiscordSocketClient client, Model model, ISocketMessageChannel channel)
            : base(client, model.Id, channel)
        {
            var dataModel = model.Data.IsSpecified ?
                (DataModel)model.Data.Value
                : null;

            ulong? guildId = null;
            if (Channel is SocketGuildChannel guildChannel)
                guildId = guildChannel.Guild.Id;

            Data = SocketCommandBaseData.Create(client, dataModel, model.Id, guildId);
        }

        new internal static SocketInteraction Create(DiscordSocketClient client, Model model, ISocketMessageChannel channel)
        {
            var entity = new SocketCommandBase(client, model, channel);
            entity.Update(model);
            return entity;
        }

        internal override void Update(Model model)
        {
            var data = model.Data.IsSpecified ?
                (DataModel)model.Data.Value
                : null;

            Data.Update(data);

            base.Update(model);
        }

        /// <inheritdoc/>
        public override async Task RespondAsync(
            string text = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            RequestOptions options = null,
            MessageComponent component = null,
            Embed embed = null)
        {
            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            embeds ??= Array.Empty<Embed>();
            if (embed != null)
                embeds = new[] { embed }.Concat(embeds).ToArray();

            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");
            Preconditions.AtMost(embeds?.Length ?? 0, 10, nameof(embeds), "A max of 10 embeds are allowed.");

            // check that user flag and user Id list are exclusive, same with role flag and role Id list
            if (allowedMentions != null && allowedMentions.AllowedTypes.HasValue)
            {
                if (allowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.Users) &&
                    allowedMentions.UserIds != null && allowedMentions.UserIds.Count > 0)
                {
                    throw new ArgumentException("The Users flag is mutually exclusive with the list of User Ids.", nameof(allowedMentions));
                }

                if (allowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.Roles) &&
                    allowedMentions.RoleIds != null && allowedMentions.RoleIds.Count > 0)
                {
                    throw new ArgumentException("The Roles flag is mutually exclusive with the list of Role Ids.", nameof(allowedMentions));
                }
            }

            var response = new API.InteractionResponse
            {
                Type = InteractionResponseType.ChannelMessageWithSource,
                Data = new API.InteractionCallbackData
                {
                    Content = text,
                    AllowedMentions = allowedMentions?.ToModel() ?? Optional<API.AllowedMentions>.Unspecified,
                    Embeds = embeds?.Select(x => x.ToModel()).ToArray() ?? Optional<API.Embed[]>.Unspecified,
                    TTS = isTTS ? true : Optional<bool>.Unspecified,
                    Components = component?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified,
                    Flags = ephemeral ? MessageFlags.Ephemeral : Optional<MessageFlags>.Unspecified
                }
            };

            await InteractionHelper.SendInteractionResponse(Discord, response, Id, Token, options);
        }

        /// <inheritdoc/>
        public override async Task<RestFollowupMessage> FollowupAsync(
            string text = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            RequestOptions options = null,
            MessageComponent component = null,
            Embed embed = null)
        {
            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            embeds ??= Array.Empty<Embed>();
            if (embed != null)
                embeds = new[] { embed }.Concat(embeds).ToArray();

            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");
            Preconditions.AtMost(embeds?.Length ?? 0, 10, nameof(embeds), "A max of 10 embeds are allowed.");

            var args = new API.Rest.CreateWebhookMessageParams
            {
                Content = text,
                AllowedMentions = allowedMentions?.ToModel() ?? Optional<API.AllowedMentions>.Unspecified,
                IsTTS = isTTS,
                Embeds = embeds?.Select(x => x.ToModel()).ToArray() ?? Optional<API.Embed[]>.Unspecified,
                Components = component?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified
            };

            if (ephemeral)
                args.Flags = MessageFlags.Ephemeral;

            return await InteractionHelper.SendFollowupAsync(Discord.Rest, args, Token, Channel, options);
        }

        /// <inheritdoc/>
        public override async Task<RestFollowupMessage> FollowupWithFileAsync(
            Stream fileStream,
            string text = null,
            string fileName = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            RequestOptions options = null,
            MessageComponent component = null,
            Embed embed = null)
        {
            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            embeds ??= Array.Empty<Embed>();
            if (embed != null)
                embeds = new[] { embed }.Concat(embeds).ToArray();

            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");
            Preconditions.AtMost(embeds?.Length ?? 0, 10, nameof(embeds), "A max of 10 embeds are allowed.");
            Preconditions.NotNull(fileStream, nameof(fileStream), "File Stream must have data");
            Preconditions.NotNullOrEmpty(fileName, nameof(fileName), "File Name must not be empty or null");

            var args = new API.Rest.CreateWebhookMessageParams
            {
                Content = text,
                AllowedMentions = allowedMentions?.ToModel() ?? Optional<API.AllowedMentions>.Unspecified,
                IsTTS = isTTS,
                Embeds = embeds?.Select(x => x.ToModel()).ToArray() ?? Optional<API.Embed[]>.Unspecified,
                Components = component?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified,
                File = fileStream is not null ? new MultipartFile(fileStream, fileName) : Optional<MultipartFile>.Unspecified
            };

            if (ephemeral)
                args.Flags = MessageFlags.Ephemeral;

            return await InteractionHelper.SendFollowupAsync(Discord.Rest, args, Token, Channel, options);
        }

        /// <inheritdoc/>
        public override async Task<RestFollowupMessage> FollowupWithFileAsync(
            string filePath,
            string text = null,
            string fileName = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            RequestOptions options = null,
            MessageComponent component = null,
            Embed embed = null)
        {
            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            embeds ??= Array.Empty<Embed>();
            if (embed != null)
                embeds = new[] { embed }.Concat(embeds).ToArray();

            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");
            Preconditions.AtMost(embeds?.Length ?? 0, 10, nameof(embeds), "A max of 10 embeds are allowed.");
            Preconditions.NotNullOrEmpty(filePath, nameof(filePath), "Path must exist");

            fileName ??= Path.GetFileName(filePath);
            Preconditions.NotNullOrEmpty(fileName, nameof(fileName), "File Name must not be empty or null");

            var args = new API.Rest.CreateWebhookMessageParams
            {
                Content = text,
                AllowedMentions = allowedMentions?.ToModel() ?? Optional<API.AllowedMentions>.Unspecified,
                IsTTS = isTTS,
                Embeds = embeds?.Select(x => x.ToModel()).ToArray() ?? Optional<API.Embed[]>.Unspecified,
                Components = component?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified,
                File = !string.IsNullOrEmpty(filePath) ? new MultipartFile(new MemoryStream(File.ReadAllBytes(filePath), false), fileName) : Optional<MultipartFile>.Unspecified
            };

            if (ephemeral)
                args.Flags = MessageFlags.Ephemeral;

            return await InteractionHelper.SendFollowupAsync(Discord.Rest, args, Token, Channel, options);
        }

        /// <summary>
        ///     Acknowledges this interaction with the <see cref="InteractionResponseType.DeferredChannelMessageWithSource"/>.
        /// </summary>
        /// <returns>
        ///     A task that represents the asynchronous operation of acknowledging the interaction.
        /// </returns>
        public override Task DeferAsync(bool ephemeral = false, RequestOptions options = null)
        {
            var response = new API.InteractionResponse
            {
                Type = InteractionResponseType.DeferredChannelMessageWithSource,
                Data = new API.InteractionCallbackData
                {
                    Flags = ephemeral ? MessageFlags.Ephemeral : Optional<MessageFlags>.Unspecified
                }
            };

            return Discord.Rest.ApiClient.CreateInteractionResponseAsync(response, Id, Token, options);
        }
    }
}
