using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    internal class UserTypeReader<T> : TypeReader
        where T : class, IUser
    {
        public override async Task<TypeReaderResult> Read(IMessage context, string input)
        {
            IUser result = null;
            
            //By Id
            ulong id;
            if (MentionUtils.TryParseUser(input, out id) || ulong.TryParse(input, out id))
            {
                var user = await context.Channel.GetUserAsync(id).ConfigureAwait(false);
                if (user != null)
                    result = user;
            }

            //By Username + Discriminator
            if (result == null)
            {
                int index = input.LastIndexOf('#');
                if (index >= 0)
                {
                    string username = input.Substring(0, index);
                    ushort discriminator;
                    if (ushort.TryParse(input.Substring(index + 1), out discriminator))
                    {
                        var users = await context.Channel.GetUsersAsync().ConfigureAwait(false);
                        result = users.Where(x =>
                            x.DiscriminatorValue == discriminator &&
                            string.Equals(username, x.Username, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    }
                }
            }

            //By Username
            if (result == null)
            {
                var users = await context.Channel.GetUsersAsync().ConfigureAwait(false);
                var filteredUsers = users.Where(x => string.Equals(input, x.Username, StringComparison.OrdinalIgnoreCase)).ToArray();
                if (filteredUsers.Length > 1)
                    return TypeReaderResult.FromError(CommandError.MultipleMatches, "Multiple users found.");
                else if (filteredUsers.Length == 1)
                    result = filteredUsers[0];
            }

            if (result == null)
                return TypeReaderResult.FromError(CommandError.ObjectNotFound, "User not found.");

            T castResult = result as T;
            if (castResult == null)
                return TypeReaderResult.FromError(CommandError.CastFailed, $"User is not a {typeof(T).Name}.");
            else
                return TypeReaderResult.FromSuccess(castResult);
        }
    }
}
