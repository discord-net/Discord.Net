// RestUser entities expose the accent color and banner of a user.
// This being one of the few use-cases for requesting a RestUser instead of depending on the Socket counterpart.
public static EmbedBuilder WithUserColor(this EmbedBuilder builder, IUser user)
{
    var restUser = await _client.Rest.GetUserAsync(user.Id);
    return builder.WithColor(restUser.AccentColor ?? Color.Blue);
    // The accent color can still be null, so a check for this needs to be done to prevent an exception to be thrown.
}
