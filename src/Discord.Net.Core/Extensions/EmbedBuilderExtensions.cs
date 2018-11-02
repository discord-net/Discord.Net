using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
    /// <summary> An extension class for building an embed. </summary>
    public static class EmbedBuilderExtensions
    {
        /// <summary> Adds embed color based on the provided raw value. </summary>
        public static EmbedBuilder WithColor(this EmbedBuilder builder, uint rawValue) =>
            builder.WithColor(new Color(rawValue));

        /// <summary> Adds embed color based on the provided RGB <see cref="byte"/> value. </summary>
        public static EmbedBuilder WithColor(this EmbedBuilder builder, byte r, byte g, byte b) =>
            builder.WithColor(new Color(r, g, b));

        /// <summary> Adds embed color based on the provided RGB <see cref="int"/> value. </summary>
        /// <exception cref="ArgumentOutOfRangeException">The argument value is not between 0 to 255.</exception>
        public static EmbedBuilder WithColor(this EmbedBuilder builder, int r, int g, int b) =>
            builder.WithColor(new Color(r, g, b));

        /// <summary> Adds embed color based on the provided RGB <see cref="float"/> value. </summary>
        /// <exception cref="ArgumentOutOfRangeException">The argument value is not between 0 to 1.</exception>
        public static EmbedBuilder WithColor(this EmbedBuilder builder, float r, float g, float b) =>
            builder.WithColor(new Color(r, g, b));

        /// <summary> Fills the embed author field with the provided user's full username and avatar URL. </summary>
        public static EmbedBuilder WithAuthor(this EmbedBuilder builder, IUser user) =>
            builder.WithAuthor($"{user.Username}#{user.Discriminator}", user.GetAvatarUrl());

        /// <summary> Fills the embed author field with the provided user's nickname and avatar URL; username is used if nickname is not set. </summary>
        public static EmbedBuilder WithAuthor(this EmbedBuilder builder, IGuildUser user) =>
            builder.WithAuthor($"{user.Nickname ?? user.Username}#{user.Discriminator}", user.GetAvatarUrl());

        /// <summary> Converts a <see cref="EmbedType.Rich"/> <see cref="IEmbed"/> object to a <see cref="EmbedBuilder"/>. </summary>
        /// <exception cref="InvalidOperationException">The embed type is not <see cref="EmbedType.Rich"/>.</exception>
        public static EmbedBuilder ToEmbedBuilder(this IEmbed embed)
        {
            if (embed.Type != EmbedType.Rich)
                throw new InvalidOperationException($"Only {nameof(EmbedType.Rich)} embeds may be built.");

            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = embed.Author?.Name,
                    IconUrl = embed.Author?.IconUrl,
                    Url = embed.Author?.Url
                },
                Color = embed.Color ?? Color.Default,
                Description = embed.Description,
                Footer = new EmbedFooterBuilder
                {
                    Text = embed.Footer?.Text,
                    IconUrl = embed.Footer?.IconUrl
                },
                ImageUrl = embed.Image?.Url,
                ThumbnailUrl = embed.Thumbnail?.Url,
                Timestamp = embed.Timestamp,
                Title = embed.Title,
                Url = embed.Url
            };

            foreach (var field in embed.Fields)
                builder.AddField(field.Name, field.Value, field.Inline);

            return builder;
        }

        public static EmbedBuilder WithFields(this EmbedBuilder builder, IEnumerable<EmbedFieldBuilder> fields)
        {
            foreach (var field in fields)
                builder.AddField(field);

            return builder;
        }
        public static EmbedBuilder WithFields(this EmbedBuilder builder, params EmbedFieldBuilder[] fields)
            => WithFields(builder, fields.AsEnumerable());
    }
}
