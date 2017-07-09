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
    }
}
