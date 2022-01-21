IUser user;

// Here we check if the user is an IGuildUser, if not, let it pass. This ensures its not null.
if (user is IGuildUser)
{
    Console.WriteLine("This user is in a guild!");
}
// Check failed.
