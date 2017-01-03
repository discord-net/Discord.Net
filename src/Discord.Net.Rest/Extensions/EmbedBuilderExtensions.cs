namespace Discord
{
    public static class EmbedBuilderExtensions
    {
        public static EmbedBuilder AddField(this EmbedBuilder builder, string title, string text, bool inline = false) =>
            builder.AddField(field =>
            {
                field.Name = title;
                field.Value = text;
                field.IsInline = inline;
            });

        public static EmbedBuilder WithFooter(this EmbedBuilder builder, string text, string iconUrl = null) =>
            builder.WithFooter(footer =>
            {
                footer.Text = text;
                footer.IconUrl = iconUrl;
            });

        public static EmbedBuilder WithColor(this EmbedBuilder builder, uint rawValue) =>
            builder.WithColor(new Color(rawValue));

        public static EmbedBuilder WithColor(this EmbedBuilder builder, byte r, byte g, byte b) =>
            builder.WithColor(new Color(r, g, b));

        public static EmbedBuilder WithColor(this EmbedBuilder builder, float r, float g, float b) =>
            builder.WithColor(new Color(r, g, b));

        public static EmbedBuilder WithAuthor(this EmbedBuilder builder, string name, string iconUrl = null, string url = null) =>
            builder.WithAuthor(author =>
            {
                author.Name = name;
                author.IconUrl = iconUrl;
                author.Url = url;
            });

        public static EmbedBuilder WithAuthor(this EmbedBuilder builder, IUser user) =>
            WithAuthor(builder, $"{user.Username}#{user.Discriminator}", user.AvatarUrl);

        public static EmbedBuilder WithAuthor(this EmbedBuilder builder, IGuildUser user) =>
            WithAuthor(builder, $"{user.Nickname ?? user.Username}#{user.Discriminator}", user.AvatarUrl);
    }
}
