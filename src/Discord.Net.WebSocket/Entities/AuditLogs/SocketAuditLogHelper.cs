using System;
using System.Collections.Generic;

using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

internal static class SocketAuditLogHelper
{
    private static readonly Dictionary<ActionType, Func<DiscordSocketClient, EntryModel, ISocketAuditLogData>> CreateMapping
        = new ()
        {
            [ActionType.GuildUpdated] = SocketGuildUpdateAuditLogData.Create,

            [ActionType.ChannelCreated] = SocketChannelCreateAuditLogData.Create,
            [ActionType.ChannelUpdated] = SocketChannelUpdateAuditLogData.Create,
            [ActionType.ChannelDeleted] = SocketChannelDeleteAuditLogData.Create,
            
            [ActionType.OverwriteCreated] = SocketOverwriteCreateAuditLogData.Create,
            [ActionType.OverwriteUpdated] = SocketOverwriteUpdateAuditLogData.Create,
            [ActionType.OverwriteDeleted] = SocketOverwriteDeleteAuditLogData.Create,
            
            [ActionType.Kick] = SocketKickAuditLogData.Create,
            [ActionType.Prune] = SocketPruneAuditLogData.Create,
            [ActionType.Ban] = SocketBanAuditLogData.Create,
            [ActionType.Unban] = SocketUnbanAuditLogData.Create,
            [ActionType.MemberUpdated] = SocketMemberUpdateAuditLogData.Create,
            [ActionType.MemberRoleUpdated] = SocketMemberRoleAuditLogData.Create,
            [ActionType.MemberMoved] = SocketMemberMoveAuditLogData.Create,
            [ActionType.MemberDisconnected] = SocketMemberDisconnectAuditLogData.Create,
            [ActionType.BotAdded] = SocketBotAddAuditLogData.Create,
            
            [ActionType.RoleCreated] = SocketRoleCreateAuditLogData.Create,
            [ActionType.RoleUpdated] = SocketRoleUpdateAuditLogData.Create,
            [ActionType.RoleDeleted] = SocketRoleDeleteAuditLogData.Create,
            
            [ActionType.InviteCreated] = SocketInviteCreateAuditLogData.Create,
            [ActionType.InviteUpdated] = SocketInviteUpdateAuditLogData.Create,
            [ActionType.InviteDeleted] = SocketInviteDeleteAuditLogData.Create,
            
            [ActionType.WebhookCreated] = SocketWebhookCreateAuditLogData.Create,
            [ActionType.WebhookUpdated] = SocketWebhookUpdateAuditLogData.Create,
            [ActionType.WebhookDeleted] = SocketWebhookDeleteAuditLogData.Create,
            
            [ActionType.EmojiCreated] = SocketEmoteCreateAuditLogData.Create,
            [ActionType.EmojiUpdated] = SocketEmoteUpdateAuditLogData.Create,
            [ActionType.EmojiDeleted] = SocketEmoteDeleteAuditLogData.Create,
            
            [ActionType.MessageDeleted] = SocketMessageDeleteAuditLogData.Create,
            [ActionType.MessageBulkDeleted] = SocketMessageBulkDeleteAuditLogData.Create,
            [ActionType.MessagePinned] = SocketMessagePinAuditLogData.Create,
            [ActionType.MessageUnpinned] = SocketMessageUnpinAuditLogData.Create,
            
            [ActionType.EventCreate] = SocketScheduledEventCreateAuditLogData.Create,
            [ActionType.EventUpdate] = SocketScheduledEventUpdateAuditLogData.Create,
            [ActionType.EventDelete] = SocketScheduledEventDeleteAuditLogData.Create,
            
            [ActionType.ThreadCreate] = SocketThreadCreateAuditLogData.Create,
            [ActionType.ThreadUpdate] = SocketThreadUpdateAuditLogData.Create,
            [ActionType.ThreadDelete] = SocketThreadDeleteAuditLogData.Create,

            [ActionType.ApplicationCommandPermissionUpdate] = SocketCommandPermissionUpdateAuditLogData.Create,

            [ActionType.IntegrationCreated] = SocketIntegrationCreatedAuditLogData.Create,
            [ActionType.IntegrationUpdated] = SocketIntegrationUpdatedAuditLogData.Create,
            [ActionType.IntegrationDeleted] = SocketIntegrationDeletedAuditLogData.Create,

            [ActionType.StageInstanceCreated] = SocketStageInstanceCreateAuditLogData.Create,
            [ActionType.StageInstanceUpdated] = SocketStageInstanceUpdatedAuditLogData.Create,
            [ActionType.StageInstanceDeleted] = SocketStageInstanceDeleteAuditLogData.Create,

            [ActionType.StickerCreated] = SocketStickerCreatedAuditLogData.Create,
            [ActionType.StickerUpdated] = SocketStickerUpdatedAuditLogData.Create,
            [ActionType.StickerDeleted] = SocketStickerDeletedAuditLogData.Create,

            [ActionType.AutoModerationRuleCreate] = SocketAutoModRuleCreatedAuditLogData.Create,
            [ActionType.AutoModerationRuleUpdate] = AutoModRuleUpdatedAuditLogData.Create,
            [ActionType.AutoModerationRuleDelete] = SocketAutoModRuleDeletedAuditLogData.Create,
            
            [ActionType.AutoModerationBlockMessage] = SocketAutoModBlockedMessageAuditLogData.Create,
            [ActionType.AutoModerationFlagToChannel] = SocketAutoModFlaggedMessageAuditLogData.Create,
            [ActionType.AutoModerationUserCommunicationDisabled] = SocketAutoModTimeoutUserAuditLogData.Create,

            [ActionType.OnboardingQuestionCreated] = SocketOnboardingPromptCreatedAuditLogData.Create,
            [ActionType.OnboardingQuestionUpdated] = SocketOnboardingPromptUpdatedAuditLogData.Create,
            [ActionType.OnboardingUpdated] = SocketOnboardingUpdatedAuditLogData.Create,
        };

    public static ISocketAuditLogData CreateData(DiscordSocketClient discord, EntryModel entry)
    {
        if (CreateMapping.TryGetValue(entry.Action, out var func))
            return func(discord, entry);

        return null;
    }
}
