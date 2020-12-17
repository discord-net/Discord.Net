using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommand;

namespace Discord.Rest
{
    internal static class ApplicationCommandHelper
    {
        public static async Task<Model> ModifyAsync(IApplicationCommand command, BaseDiscordClient client,
            Action<ApplicationCommandProperties> func, RequestOptions options)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var args = new ApplicationCommandProperties();
            func(args);

            var apiArgs = new Discord.API.Rest.ApplicationCommandParams()
            {
                Description = args.Description,
                Name = args.Name,
                Options = args.Options.IsSpecified
                    ? args.Options.Value.Select(x => new API.ApplicationCommandOption(x)).ToArray()
                    : Optional<API.ApplicationCommandOption[]>.Unspecified,
            };

            return await client.ApiClient.ModifyGlobalApplicationCommandAsync(apiArgs, command.Id, options);
        }
    }
}
