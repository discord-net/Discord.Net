using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a set of json error codes received by discord.
    /// </summary>
    public enum DiscordErrorCode
    {
        GeneralError = 0,

        #region UnknownXYZ (10XXX)
        UnknownAccount = 10001,
        UnknownApplication = 10002,
        UnknownChannel = 10003,
        UnknownGuild = 10004,
        UnknownIntegration = 10005,
        UnknownInvite = 10006,
        UnknownMember = 10007,
        UnknownMessage = 10008,
        UnknownPermissionOverwrite = 10009,
        UnknownProvider = 10010,
        UnknownRole = 10011,
        UnknownToken = 10012,
        UnknownUser = 10013,
        UnknownEmoji = 10014,
        UnknownWebhook = 10015,
        UnknownWebhookService = 10016,
        UnknownSession = 10020,
        UnknownBan = 10026,
        UnknownSKU = 10027,
        UnknownStoreListing = 10028,
        UnknownEntitlement = 10029,
        UnknownBuild = 10030,
        UnknownLobby = 10031,
        UnknownBranch = 10032,
        UnknownStoreDirectoryLayout = 10033,
        UnknownRedistributable = 10036,
        UnknownGiftCode = 10038,
        UnknownStream = 10049,
        UnknownPremiumServerSubscribeCooldown = 10050,
        UnknownGuildTemplate = 10057,
        UnknownDiscoverableServerCategory = 10059,
        UnknownSticker = 10060,
        UnknownInteraction = 10062,
        UnknownApplicationCommand = 10063,
        UnknownApplicationCommandPermissions = 10066,
        UnknownStageInstance = 10067,
        UnknownGuildMemberVerificationForm = 10068,
        UnknownGuildWelcomeScreen = 10069,
        UnknownGuildScheduledEvent = 10070,
        UnknownGuildScheduledEventUser = 10071,
        #endregion

        #region General Actions (20XXX)
        BotsCannotUse = 20001,
        OnlyBotsCanUse = 20002,
        CannotSendExplicitContent = 20009,
        ApplicationActionUnauthorized = 20012,
        ActionSlowmode = 20016,
        OnlyOwnerAction = 20018,
        AnnouncementEditRatelimit = 20022,
        ChannelWriteRatelimit = 20028,
        WriteRatelimitReached = 20029,
        WordsNotAllowed = 20031,
        GuildPremiumTooLow = 20035,
        #endregion

        #region Numeric Limits Reached (30XXX)
        MaximumGuildsReached = 30001,
        MaximumFriendsReached = 30002,
        MaximumPinsReached = 30003,
        MaximumRecipientsReached = 30004,
        MaximumGuildRolesReached = 30005,
        MaximumWebhooksReached = 30007,
        MaximumEmojisReached = 30008,
        MaximumReactionsReached = 30010,
        MaximumGuildChannelsReached = 30013,
        MaximumAttachmentsReached = 30015,
        MaximumInvitesReached = 30016,
        MaximumAnimatedEmojisReached = 30018,
        MaximumServerMembersReached = 30019,
        MaximumServerCategoriesReached = 30030,
        GuildTemplateAlreadyExists = 30031,
        MaximumThreadMembersReached = 30033,
        MaximumBansForNonGuildMembersReached = 30035,
        MaximumBanFetchesReached = 30037,
        MaximumUncompleteGuildScheduledEvents = 30038,
        MaximumStickersReached = 30039,
        MaximumPruneRequestReached = 30040,
        MaximumGuildWigitsReached = 30042,
        #endregion

        #region General Request Errors (40XXX)
        TokenUnauthorized = 40001,
        InvalidVerification = 40002,
        OpeningDMTooFast = 40003,
        RequestEntityTooLarge = 40005,
        FeatureDisabled = 40006,
        UserBanned = 40007,
        TargetUserNotInVoice = 40032,
        MessageAlreadyCrossposted = 40033,
        ApplicationNameAlreadyExists = 40041,
        #endregion

        #region Action Preconditions/Checks (50XXX)
        MissingPermissions = 50001,
        InvalidAccountType = 50002,
        CannotExecuteForDM = 50003,
        GuildWigitDisabled = 50004,
        CannotEditOtherUsersMessage = 50005,
        CannotSendEmptyMessage = 50006,
        CannotSendMessageToUser = 50007,
        CannotSendMessageToVoiceChannel = 50008,
        ChannelVerificationTooHight = 50009,
        OAuth2ApplicationDoesntHaveBot = 50010,
        OAuth2ApplicationLimitReached = 50011,
        InvalidOAuth2State = 50012,
        InsufficientPermissions = 50013,
        InvalidAuthenticationToken = 50014,
        NoteTooLong = 50015,
        ProvidedMessageDeleteCountOutOfBounds = 50016,
        InvalidPinChannel = 50019,
        InvalidInvite = 50020,
        CannotExecuteOnSystemMessage = 50021,
        CannotExecuteOnChannelType = 50024,
        InvalidOAuth2Token = 50025,
        MissingOAuth2Scope = 50026,
        InvalidWebhookToken = 50027,
        InvalidRole = 50028,
        InvalidRecipients = 50033,
        BulkDeleteMessageTooOld = 50034,
        InvalidFormBody = 50035,
        InviteAcceptedForGuildThatBotIsntIn = 50036,
        InvalidAPIVersion = 50041,
        FileUploadTooBig = 50045,
        InvalidFileUpload = 50046,
        CannotSelfRedeemGift = 50054,
        InvalidGuild = 50055,
        PaymentSourceRequiredForGift = 50070,
        CannotDeleteRequiredCommunityChannel = 50074,
        InvalidSticker = 50081,
        CannotExecuteOnArchivedThread = 50083,
        InvalidThreadNotificationSettings = 50084,
        BeforeValueEarlierThanThreadCreation = 50085,
        ServerLocaleUnavailable = 50095,
        ServerRequiresMonetization = 50097,
        ServerRequiresBoosts = 50101,

        #endregion

        #region 2FA (60XXX)
        Requires2FA = 60003,
        #endregion

        #region User Searches (80XXX)
        NoUsersWithTag = 80004,
        #endregion

        #region Reactions (90XXX)
        ReactionBlocked = 90001,
        #endregion

        #region API Status (130XXX)
        APIOverloaded = 130000,
        #endregion

        #region Stage Errors (150XXX)
        StageAlreadyOpened = 150006,
        #endregion

        #region Reply and Thread Errors (160XXX)
        CannotReplyWithoutReadMessageHistory = 160002,
        MessageAlreadyContainsThread = 160004,
        ThreadIsLocked = 160005,
        MaximumActiveThreadsReached = 160006,
        MaximumAnnouncementThreadsReached = 160007,
        #endregion

        #region Sticker Uploads (170XXX)
        InvalidJSONLottie = 170001,
        LottieCantContainRasters = 170002,
        StickerMaximumFramerateExceeded = 170003,
        StickerMaximumFrameCountExceeded = 170004,
        LottieMaximumDimentionsExceeded = 170005,
        StickerFramerateBoundsExceeed = 170006,
        StickerAnimationDurationTooLong = 170007,
        #endregion

        #region Guild Scheduled Events
        CannotUpdateFinishedEvent = 180000,
        FailedStageCreation = 180002,
        #endregion
    }
}
