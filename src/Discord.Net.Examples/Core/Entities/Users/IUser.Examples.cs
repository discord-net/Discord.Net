using JetBrains.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Discord.Net.Examples.Core.Entities.Users
{
    [PublicAPI]
    internal class UserExamples
    {
        #region CreateDMChannelAsync

        public async Task MessageUserAsync(IUser user)
        {
            var channel = await user.CreateDMChannelAsync();
            try
            {
                await channel.SendMessageAsync("Awesome stuff!");
            }
            catch (Discord.Net.HttpException ex) when (ex.HttpCode == HttpStatusCode.Forbidden)
            {
                Console.WriteLine($"Boo, I cannot message {user}.");
            }
        }

        #endregion
    }
}
