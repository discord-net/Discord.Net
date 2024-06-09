namespace Discord;

public static class UserExtensions
{
    public static string? GetAvatarUrl(this IUser user, ImageFormat format = ImageFormat.Auto, ushort size = 128)
        => CDN.GetUserAvatarUrl(user.Client.Config, user.Id, user.AvatarId, size, format);

    public static string GetDefaultAvatarUrl(this IUser user)
        => user.Discriminator is not 0
            ? CDN.GetDefaultUserAvatarUrl(user.Client.Config, user.Discriminator)
            : CDN.GetDefaultUserAvatarUrl(user.Client.Config, user.Id);
}
