IUser user;

// Here we check if the user is an IGuildUser, if not, let it pass. This ensures its not null.
if (user is IGuildUser)
{
    Console.WriteLine("This user is in a guild!");
}
// Check failed.

----------------------------
// Another situation, where we want to get the actual data of said IGuildUser.

----------------------------
// A final situation, where we dont actually need to do anything code-wise when the check does not pass, so we want to simplify it.
