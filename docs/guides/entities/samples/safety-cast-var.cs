IUser user;

// Here we can pre-define the actual declaration of said IGuildUser object,
// so we don't need to cast additionally inside of the statement.
if (user is IGuildUser guildUser)
{
    Console.WriteLine(guildUser.JoinedAt);
}
// Check failed.
