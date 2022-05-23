using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.Rest
{
    internal static class EntityExtensions
    {
        public static IEmote ToIEmote(this IEmojiModel model)
        {
            if (model.Id.HasValue)
                return model.ToEntity();
            return new Emoji(model.Name);
        }

        public static GuildEmote ToEntity(this IEmojiModel model)
            => new GuildEmote(model.Id.Value,
                model.Name,
                model.IsAnimated,
                model.IsManaged,
                model.IsAvailable,
                model.RequireColons,
                ImmutableArray.Create(model.Roles),
                model.CreatorId);

        public static IEmote ToIEmote(this API.Emoji model)
        {
            if (model.Id.HasValue)
                return model.ToEntity();
            return new Emoji(model.Name);
        }

        public static GuildEmote ToEntity(this API.Emoji model)
            => new GuildEmote(model.Id.Value,
                model.Name,
                model.Animated.GetValueOrDefault(),
                model.Managed,
                model.Available.GetValueOrDefault(),
                model.RequireColons,
                ImmutableArray.Create(model.Roles),
                model.User.IsSpecified ? model.User.Value.Id : (ulong?)null);

        public static Embed ToEntity(this IEmbedModel model)
        {
            return new Embed(model.Type, model.Title, model.Description, model.Url,
                model.Timestamp.HasValue ? new DateTimeOffset(model.Timestamp.Value, TimeSpan.Zero) : null,
                model.Color.HasValue ? new Color(model.Color.Value) : (Color?)null,
                model.Image != null
                    ? new EmbedImage(model.Image.Url, model.Image.ProxyUrl, model.Image.Height, model.Image.Width) : (EmbedImage?)null,
                model.Video != null
                    ? new EmbedVideo(model.Video.Url, model.Video.Height, model.Video.Width) : (EmbedVideo?)null,
                model.AuthorIconUrl != null || model.AuthorName != null || model.AuthorProxyIconUrl != null || model.AuthorUrl != null
                    ? new EmbedAuthor(model.AuthorName, model.AuthorUrl, model.AuthorIconUrl, model.AuthorProxyIconUrl) : (EmbedAuthor?)null,
                model.FooterIconUrl != null || model.FooterProxyUrl != null || model.FooterText != null
                    ? new EmbedFooter(model.FooterText, model.FooterIconUrl, model.FooterProxyUrl) : (EmbedFooter?)null,
                model.ProviderUrl != null || model.ProviderName != null
                    ? new EmbedProvider(model.ProviderName, model.ProviderUrl) : (EmbedProvider?)null,
                model.Thumbnail != null
                    ? new EmbedThumbnail(model.Thumbnail.Url, model.Thumbnail.ProxyUrl, model.Thumbnail.Height, model.Thumbnail.Width) : (EmbedThumbnail?)null,
                model.Fields != null
                    ? model.Fields.Select(x => x.ToEntity()).ToImmutableArray() : ImmutableArray.Create<EmbedField>());
        }
        public static RoleTags ToEntity(this API.RoleTags model)
        {
            return new RoleTags(
                model.BotId.IsSpecified ? model.BotId.Value : null,
                model.IntegrationId.IsSpecified ? model.IntegrationId.Value : null,
                model.IsPremiumSubscriber.IsSpecified);
        }
        public static API.Embed ToModel(this Embed entity)
        {
            if (entity == null) return null;
            var model = new API.Embed
            {
                Type = entity.Type,
                Title = entity.Title,
                Description = entity.Description,
                Url = entity.Url,
                Timestamp = entity.Timestamp,
                Color = entity.Color?.RawValue
            };
            if (entity.Author != null)
                model.Author = entity.Author.Value.ToModel();
            model.Fields = entity.Fields.Select(x => x.ToModel()).ToArray();
            if (entity.Footer != null)
                model.Footer = entity.Footer.Value.ToModel();
            if (entity.Image != null)
                model.Image = entity.Image.Value.ToModel();
            if (entity.Provider != null)
                model.Provider = entity.Provider.Value.ToModel();
            if (entity.Thumbnail != null)
                model.Thumbnail = entity.Thumbnail.Value.ToModel();
            if (entity.Video != null)
                model.Video = entity.Video.Value.ToModel();
            return model;
        }

        public static API.AllowedMentions ToModel(this AllowedMentions entity)
        {
            if (entity == null) return null;
            return new API.AllowedMentions()
            {
                Parse = entity.AllowedTypes?.EnumerateMentionTypes().ToArray(),
                Roles = entity.RoleIds?.ToArray(),
                Users = entity.UserIds?.ToArray(),
                RepliedUser = entity.MentionRepliedUser ?? Optional.Create<bool>(),
            };
        }
        public static API.MessageReference ToModel(this MessageReference entity)
        {
            return new API.MessageReference()
            {
                ChannelId = entity.InternalChannelId,
                GuildId = entity.GuildId,
                MessageId = entity.MessageId,
            };
        }
        public static IEnumerable<string> EnumerateMentionTypes(this AllowedMentionTypes mentionTypes)
        {
            if (mentionTypes.HasFlag(AllowedMentionTypes.Everyone))
                yield return "everyone";
            if (mentionTypes.HasFlag(AllowedMentionTypes.Roles))
                yield return "roles";
            if (mentionTypes.HasFlag(AllowedMentionTypes.Users))
                yield return "users";
        }
        public static API.EmbedAuthor ToModel(this EmbedAuthor entity)
        {
            return new API.EmbedAuthor { Name = entity.Name, Url = entity.Url, IconUrl = entity.IconUrl };
        }
        public static EmbedField ToEntity(this IEmbedFieldModel model)
        {
            return new EmbedField(model.Name, model.Value, model.Inline);
        }
        public static API.EmbedField ToModel(this EmbedField entity)
        {
            return new API.EmbedField { Name = entity.Name, Value = entity.Value, Inline = entity.Inline };
        }
        public static API.EmbedFooter ToModel(this EmbedFooter entity)
        {
            return new API.EmbedFooter { Text = entity.Text, IconUrl = entity.IconUrl };
        }
        public static API.EmbedImage ToModel(this EmbedImage entity)
        {
            return new API.EmbedImage { Url = entity.Url };
        }
        public static API.EmbedProvider ToModel(this EmbedProvider entity)
        {
            return new API.EmbedProvider { Name = entity.Name, Url = entity.Url };
        }
        public static API.EmbedThumbnail ToModel(this EmbedThumbnail entity)
        {
            return new API.EmbedThumbnail { Url = entity.Url };
        }
        public static API.EmbedVideo ToModel(this EmbedVideo entity)
        {
            return new API.EmbedVideo { Url = entity.Url };
        }

        public static API.Image ToModel(this Image entity)
        {
            return new API.Image(entity.Stream);
        }

        public static Overwrite ToEntity(this API.Overwrite model)
        {
            return new Overwrite(model.TargetId, model.TargetType, new OverwritePermissions(model.Allow, model.Deny));
        }
    }
}
