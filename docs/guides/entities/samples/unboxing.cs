IUser user;

// Here we use inline unboxing to make a call to its member (if available) only once.

// Note that if the entity we're trying to cast to is null, this will throw a NullReferenceException.
Console.WriteLine(((IGuildUser)user).Nickname);

// In case you are certain the entity IS said member, you can also use unboxing to declare variables.
IGuildUser guildUser = (IGuildUser)user;
