namespace Discord
{
    public static class EmbedBuilderExtensions
    {
        public static EmbedBuilder WithColor(this EmbedBuilder builder, uint rawValue) =>
            builder.WithColor(new Color(rawValue));

        public static EmbedBuilder WithColor(this EmbedBuilder builder, byte r, byte g, byte b) =>
            builder.WithColor(new Color(r, g, b));

        public static EmbedBuilder WithColor(this EmbedBuilder builder, float r, float g, float b) =>
            builder.WithColor(new Color(r, g, b));

        public static EmbedBuilder WithAuthor(this EmbedBuilder builder, IUser user) =>
            WithAuthor(builder, $"{user.Username}#{user.Discriminator}", user.AvatarUrl);

        public static EmbedBuilder WithAuthor(this EmbedBuilder builder, IGuildUser user) =>
            WithAuthor(builder, $"{user.Nickname ?? user.Username}#{user.Discriminator}", user.AvatarUrl);
    }
}
