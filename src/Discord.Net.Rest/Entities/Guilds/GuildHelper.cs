using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using EmbedModel = Discord.API.GuildEmbed;
using Model = Discord.API.Guild;
using RoleModel = Discord.API.Role;

namespace Discord.Rest
{
    internal static class GuildHelper
    {
        //General
        public static async Task<Model> ModifyAsync(IGuild guild, BaseDiscordClient client,
            Action<ModifyGuildParams> func, RequestOptions options)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildParams();
            func(args);

            if (args.Splash.IsSpecified && guild.SplashId != null)
                args.Splash = new API.Image(guild.SplashId);
            if (args.Icon.IsSpecified && guild.IconId != null)
                args.Icon = new API.Image(guild.IconId);

            return await client.ApiClient.ModifyGuildAsync(guild.Id, args, options).ConfigureAwait(false);
        }
        public static async Task<EmbedModel> ModifyEmbedAsync(IGuild guild, BaseDiscordClient client,
            Action<ModifyGuildEmbedParams> func, RequestOptions options)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildEmbedParams();
            func(args);
            return await client.ApiClient.ModifyGuildEmbedAsync(guild.Id, args, options).ConfigureAwait(false);
        }
        public static async Task ModifyChannelsAsync(IGuild guild, BaseDiscordClient client,
            IEnumerable<ModifyGuildChannelsParams> args, RequestOptions options)
        {
            await client.ApiClient.ModifyGuildChannelsAsync(guild.Id, args, options).ConfigureAwait(false);
        }
        public static async Task<IReadOnlyCollection<RoleModel>> ModifyRolesAsync(IGuild guild, BaseDiscordClient client,
            IEnumerable<ModifyGuildRolesParams> args, RequestOptions options)
        {
            return await client.ApiClient.ModifyGuildRolesAsync(guild.Id, args, options).ConfigureAwait(false);
        }
        public static async Task LeaveAsync(IGuild guild, BaseDiscordClient client, 
            RequestOptions options)
        {
            await client.ApiClient.LeaveGuildAsync(guild.Id, options).ConfigureAwait(false);
        }
        public static async Task DeleteAsync(IGuild guild, BaseDiscordClient client, 
            RequestOptions options)
        {
            await client.ApiClient.DeleteGuildAsync(guild.Id, options).ConfigureAwait(false);
        }

        //Bans
        public static async Task<IReadOnlyCollection<RestBan>> GetBansAsync(IGuild guild, BaseDiscordClient client, 
            RequestOptions options)
        {
            var models = await client.ApiClient.GetGuildBansAsync(guild.Id, options).ConfigureAwait(false);
            return models.Select(x => RestBan.Create(client, x)).ToImmutableArray();
        }
        
        public static async Task AddBanAsync(IGuild guild, BaseDiscordClient client, 
            ulong userId, int pruneDays, RequestOptions options)
        {
            var args = new CreateGuildBanParams { DeleteMessageDays = pruneDays };
            await client.ApiClient.CreateGuildBanAsync(guild.Id, userId, args, options).ConfigureAwait(false);
        }        
        public static async Task RemoveBanAsync(IGuild guild, BaseDiscordClient client, 
            ulong userId, RequestOptions options)
        {
            await client.ApiClient.RemoveGuildBanAsync(guild.Id, userId, options).ConfigureAwait(false);
        }

        //Channels
        public static async Task<RestGuildChannel> GetChannelAsync(IGuild guild, BaseDiscordClient client,
            ulong id, RequestOptions options)
        {
            var model = await client.ApiClient.GetChannelAsync(guild.Id, id, options).ConfigureAwait(false);
            if (model != null)
                return RestGuildChannel.Create(client, guild, model);
            return null;
        }
        public static async Task<IReadOnlyCollection<RestGuildChannel>> GetChannelsAsync(IGuild guild, BaseDiscordClient client, 
            RequestOptions options)
        {
            var models = await client.ApiClient.GetGuildChannelsAsync(guild.Id, options).ConfigureAwait(false);
            return models.Select(x => RestGuildChannel.Create(client, guild, x)).ToImmutableArray();
        }
        public static async Task<RestTextChannel> CreateTextChannelAsync(IGuild guild, BaseDiscordClient client,
            string name, RequestOptions options)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var args = new CreateGuildChannelParams(name, ChannelType.Text);
            var model = await client.ApiClient.CreateGuildChannelAsync(guild.Id, args, options).ConfigureAwait(false);
            return RestTextChannel.Create(client, guild, model);
        }
        public static async Task<RestVoiceChannel> CreateVoiceChannelAsync(IGuild guild, BaseDiscordClient client,
            string name, RequestOptions options)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var args = new CreateGuildChannelParams(name, ChannelType.Voice);
            var model = await client.ApiClient.CreateGuildChannelAsync(guild.Id, args, options).ConfigureAwait(false);
            return RestVoiceChannel.Create(client, guild, model);
        }

        //Integrations
        public static async Task<IReadOnlyCollection<RestGuildIntegration>> GetIntegrationsAsync(IGuild guild, BaseDiscordClient client, 
            RequestOptions options)
        {
            var models = await client.ApiClient.GetGuildIntegrationsAsync(guild.Id, options).ConfigureAwait(false);
            return models.Select(x => RestGuildIntegration.Create(client, guild, x)).ToImmutableArray();
        }
        public static async Task<RestGuildIntegration> CreateIntegrationAsync(IGuild guild, BaseDiscordClient client,
            ulong id, string type, RequestOptions options)
        {
            var args = new CreateGuildIntegrationParams(id, type);
            var model = await client.ApiClient.CreateGuildIntegrationAsync(guild.Id, args, options).ConfigureAwait(false);
            return RestGuildIntegration.Create(client, guild, model);
        }

        //Invites
        public static async Task<IReadOnlyCollection<RestInviteMetadata>> GetInvitesAsync(IGuild guild, BaseDiscordClient client, 
            RequestOptions options)
        {
            var models = await client.ApiClient.GetGuildInvitesAsync(guild.Id, options).ConfigureAwait(false);
            return models.Select(x => RestInviteMetadata.Create(client, guild, null, x)).ToImmutableArray();
        }

        //Roles
        public static async Task<RestRole> CreateRoleAsync(IGuild guild, BaseDiscordClient client,
            string name, GuildPermissions? permissions, Color? color, bool isHoisted, RequestOptions options)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var model = await client.ApiClient.CreateGuildRoleAsync(guild.Id, options).ConfigureAwait(false);
            var role = RestRole.Create(client, model);

            await role.ModifyAsync(x =>
            {
                x.Name = name;
                x.Permissions = (permissions ?? role.Permissions).RawValue;
                x.Color = (color ?? Color.Default).RawValue;
                x.Hoist = isHoisted;
            }, options).ConfigureAwait(false);

            return role;
        }

        //Users
        public static async Task<RestGuildUser> GetUserAsync(IGuild guild, BaseDiscordClient client,
            ulong id, RequestOptions options)
        {
            var model = await client.ApiClient.GetGuildMemberAsync(guild.Id, id, options).ConfigureAwait(false);
            if (model != null)
                return RestGuildUser.Create(client, guild, model);
            return null;
        }
        public static async Task<RestGuildUser> GetCurrentUserAsync(IGuild guild, BaseDiscordClient client, 
            RequestOptions options)
        {
            return await GetUserAsync(guild, client, client.CurrentUser.Id, options).ConfigureAwait(false);
        }
        public static IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> GetUsersAsync(IGuild guild, BaseDiscordClient client,
            ulong? fromUserId, int? limit, RequestOptions options)
        {
            return new PagedAsyncEnumerable<RestGuildUser>(
                DiscordConfig.MaxMessagesPerBatch,
                async (info, ct) =>
                {
                    var args = new GetGuildMembersParams
                    {
                        Limit = info.PageSize
                    };
                    if (info.Position != null)
                        args.AfterUserId = info.Position.Value;
                    var models = await client.ApiClient.GetGuildMembersAsync(guild.Id, args, options);
                    return models.Select(x => RestGuildUser.Create(client, guild, x)).ToImmutableArray();
                },
                nextPage: (info, lastPage) =>
                {
                    info.Position = lastPage.Max(x => x.Id);
                    if (lastPage.Count != DiscordConfig.MaxMessagesPerBatch)
                        info.Remaining = 0;
                },
                start: fromUserId,
                count: limit
            );
        }
        public static async Task<int> PruneUsersAsync(IGuild guild, BaseDiscordClient client,
            int days, bool simulate, RequestOptions options)
        {
            var args = new GuildPruneParams(days);
            GetGuildPruneCountResponse model;
            if (simulate)
                model = await client.ApiClient.GetGuildPruneCountAsync(guild.Id, args, options).ConfigureAwait(false);
            else
                model = await client.ApiClient.BeginGuildPruneAsync(guild.Id, args, options).ConfigureAwait(false);
            return model.Pruned;
        }
    }
}
