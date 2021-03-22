using Discord.Rest;
using Discord;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _04_simple_blazor_discord_login.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        private DiscordRestClient DiscordRestClient { get; set; }
        [Inject]
        private NavigationManager navigationManager { get; set; }
        public RestSelfUser User { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
            var uri_query = uri.Query;
            //?access_token = RTfP0OK99U3kbRtHOoKLmJbOn45PjL & token_type = Bearer & expires_in = 604800 & scope = identify & state = 15773059ghq9183habn
            if (QueryHelpers.ParseQuery(uri.Fragment.TrimStart('#')).TryGetValue("access_token", out var token))
            {
                
                await DiscordRestClient.LoginAsync(TokenType.Bearer, token);
                User = DiscordRestClient.CurrentUser;
                StateHasChanged();
            }
        }


    }

}
