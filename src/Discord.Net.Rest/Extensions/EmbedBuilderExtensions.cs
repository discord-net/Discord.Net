using System;

namespace Discord
{
    public static class EmbedBuilderExtensions
    {
        public static EmbedBuilder WithColor(this EmbedBuilder builder, uint rawValue) =>
            builder.WithColor(new Color(rawValue));

        public static EmbedBuilder WithColor(this EmbedBuilder builder, byte r, byte g, byte b) =>
            builder.WithColor(new Color(r, g, b));

        public static EmbedBuilder WithColor(this EmbedBuilder builder, int r, int g, int b) =>
            builder.WithColor(new Color(r, g, b));

        public static EmbedBuilder WithColor(this EmbedBuilder builder, float r, float g, float b) =>
            builder.WithColor(new Color(r, g, b));

        public static EmbedBuilder WithAuthor(this EmbedBuilder builder, IUser user) =>
            builder.WithAuthor($"{user.Username}#{user.Discriminator}", user.GetAvatarUrl());

        public static EmbedBuilder WithAuthor(this EmbedBuilder builder, IGuildUser user) =>
            builder.WithAuthor($"{user.Nickname ?? user.Username}#{user.Discriminator}", user.GetAvatarUrl());

        public static EmbedBuilder ToEmbedBuilder(this IEmbed embed)
        {
            if (embed.Type != EmbedType.Rich)
                throw new InvalidOperationException($"Only {nameof(EmbedType.Rich)} embeds may be built.");

            var eb = new EmbedBuilder
            {
                Title = embed.Title,
                Description = embed.Description,
                Url = embed.Url,
                ImageUrl = embed.Image?.Url,
                ThumbnailUrl = embed.Thumbnail?.Url,
                Timestamp = embed.Timestamp
            }
            .WithAuthor(embed.Author?.Name, embed.Author?.IconUrl, embed.Author?.Url)
            .WithFooter(embed.Footer?.Text, embed.Footer?.IconUrl);

            if (embed.Color.HasValue)
                eb.WithColor(embed.Color.Value);

            foreach (var field in embed.Fields)
                eb.AddField(field.Name, field.Value, field.Inline);

            return eb;
        }
    }
}
