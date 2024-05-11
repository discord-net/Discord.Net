# Changelog

## [3.15.0] - 2024-05-12
### Added
- #2902 Polls (476ec06)
- #2898 Guild bans with seconds prune period (d1da2a0)
- #2903 Create `RequireTeamAttribute` (753724d)
- #2906 Create `DoHierarchyCheckAttribute` (1a5cba8)
- #2923 Create `RequireRoleAttribute` (5dadcb2)
- #2919 More store stuff (f9086d3)
- #2920 Add `SocketInteraction.InteractionChannel` (3054505)

### Fixed
- #2910 RestDMChannel relies on recipient (13ea566)
- #2911 Don't rely on Guild for id (397a887)
- #2912 Don't depend on WebSocket for Interaction service (a2f624e)
- #2922 NRE with rest interactions & no api call (again) (83fdc89) 
- #2924 Fix null error when using a `SelectMenuBuilder` constructor (93cb71a)

### Misc
- #2905 Remove analyzer project & resolve some build warnings (7aab366)
- #2913 Replace generic exceptions. (54d2fe5)
- #2908 Support options in Rest's AddRoleAsync (b70e21b)
- #2914 `.Distinct()` roles(ids) in `IGuildUser.ModifyAsync` (27a0785)
- #2915 Improve bulk bans (f85bf9a)


## [3.14.1] - 2024-03-19
### Fixed
- Version number contains commit hash

## [3.14.0] - 2024-03-19

### Added
- #2849 Allow setting channel topic to null when creating channels (7c96844)
- #2832 Add Icon size parameter to `GetGuildIconUrl` CDN method (eb5678a)
- #2883 Initial user apps support (24a6978)
- #2881 Bulk ban support (03402cd)
- #2880 More banner support (4efe3be)
- #2858 Allow setting custom status with `SetGameAsync` (ee386b1)
- #2870 SentRequest event (fc2fc87)
- #2873 Voice reconnection and resuming (09680c5)
- #2879 Support setting banner on bot profiles (d0c0a56)

### Fixed
- #2866 Allow switching voice channels without disconnecting (external clients only) (5a62ba1)
- #2857 Receiving voice packets (use system-assigned port) (1a16f4d)
- #2850 NRE in automod audit logs (1eb7a53)
- #2862 Fixed a bug when creating a `SelectMenuBuilder` from a `SelectMenuComponent` incorrectly set the `CustomId` (6870ac5)
- Incorrect ConnectionState of a DiscordSocketClient casted to IDiscordClient/BaseSocketClient (955fae0)
- #2867 `InteractionService.AddModulesXAsync` command duplication (2787db3)

### Misc
- #2806 Update library dependencies (bfc8dc2)
- #2847 Remove some redundant notes & small updates (d0e9c8b)
- #2854 Refactor `MaxBitrate` and `UploadLimit` in `GuildHelper` (6256de4)
- #2869 Fix optional param being non-optional (fa51f0a)


## [3.13.1] - 2024-02-11

### Added
- #2804 Add Incident actions message types (178ff58)
- #2805 Add `AppliedTags` to `DiscordWebhookClient.SendXAsync` (d382e5c)
- #2700 Optional Aliasses, Summary and Remarks Properties to CommandAttribute (5a8582c)
- #2584 Log InteractionCommand execution exceptions in Wrapped Exception (0f0f3f7)
- #2827 `44` (Add message type 44) (541571c)
- #2837 Allow creating announcement channels (f1777de)

### Fixed
- #2801 Add null check in `SelectMenuBuilder` (b18cc31)
- #2813 Incorrect json model for Guild Scheduled Events (de4b6b9)
- #2800 Warning about missing intents not being logged (8227d70)
- #2818 `GetEventCoverImageUrl` throwing NRE in case event cover image is null (138e18c)

### Misc
- #2807 Incorrect xmldoc (d234db7)
- #2823 Update sample projects & samples in docs (e2e8c0f)
- #2819 [2024] Very important changes (update licence & docs) (ff9b9b4)
- #2831 Add checks for whitespace-only strings in button builder (89efe0f)
- #2835 Additional select menu option validations (4d7e384)


## [3.13.0] - 2023-11-18

### Added
- #2707 Added Super reactions support (9fd5c6c)
- #2725 Added Media channel support (e3cd340)
- #2753 Added additional Update/Modify Async Attachment's (ae275ab)
- #2769 Added missing parameters to `RespondWithModalAsync` methods and implement missing overloads (4e78aec)
- #2773 Added useful utility methods to `ModalBuilder` (8591de7)
- #2776 Added Select menu default values (ac274d4)
- #2777 Added Voice channel status support (8060dcf)
- #2781 Added premium subscriptions support (7723f61)
- #2782 Added Avatar decorations support (f64d9d6)
- #2784 Added App subscriptions additional support (a9a8da4)
- #2783 Added Guild incident actions (9b36df3)
- #2787 Added Embed `ToJsonString` & `(Try)Parse` (7b5c40a)
- #2791 Added Automod member profile moderation, new application fields, use clyde ai & guild invenory settings (699554a)
- #2792 Added create roles with icons (b45b774)
- #2796 Added clips support (poggers) (89bebc3)

### Fixed
- #2758 Fixed Slash Command Localizations lost after AddCommandsToGuildAsync and deleteMissing = false (8d5022a)
- #2759 Fixed missing null checks in `RoleConnectionProperties` (ab3b30d)
- #2766 Fixed UpdateAsync Remove Attachment Bug (a668757)
- #2763 Fixed SocketKickAuditLogData User never having value (5cfec05)
- #2770 Fixed URL encode timestamps sent as query parameters (589c58a)
- #2774 Fixed some IDiscordClient methods are not properly implemented in clients (8baf913)
- #2789 Fixed multiline values in short TextInputs (33e8340)


### Misc
- #2739 Remove some unnecessary async/await (86655a8)
- #2754 Replace Moq with NSubstitute for Mocking in GuildHelperTests (b988a18)
- #2765 Use Discord heartbeat interval in audio. (9cedfbc)
- #2771 Extract `GetDisplayAvatarUrl` in `IUser` (b1787d8)


## [3.12.0] - 2023-08-10

### Added
- #2644 Added `ComponentBuilder` component removal methods. (d5d7378)
- #2668 Added a method for sending silent audio frames on `OpusEncodeStream`. (59094d2)
- #2675 Added missing `remove` property to `MemberRoleAudit`. (5fb9152)
- #2720 Added role & attachment flags (a421715)
- #2723 Added missing properties to webhook implementation (2b8584d)
- #2729 Added new onboarding methods/properties (8cd4c1c)
- #2730 Added new `ModifyCurrentApplication` features (166d40f)
- #2749 Added support for setting custom status (b2820d5)

### Fixed
- #2711 Fixed modifying guild resets preferred locale (12c50f2)
- #2713 Fixed some `ReplyAsync` methods missing `MessageFlags` parameter. (56769cb)
- #2714 Fixed disconnecting socket client not sending close code. (6bb3777)
- #2722 Fixed modal's `DeferAsync` implementation (5f580f6)
- #2726 Fixed global name not being passed to `UserUpdated` event (9dfd4d7)
- #2742 Fixed `ModifyAsync` attachments #2236 (0c12079)
- #2745 Fixed `GetMessagesAsync` with `Direction.After` not working as expected (c922497)


## [3.11.0] - 2023-6-27

### Added
- #2616 Added guild Onboarding support (3a8f76c)
- #2627 Added `Audit Log Created` gateway event support (dff6a57)
- #2645 Added `Message` property and `UpdateAsync()` to `IModalInteraction` (83dfa0c)
- #2648 Added `MaxStageVideoChannelUsers` property to `IGuild` (bdd755b)
- #2665 Added voice messages support (f171684)
- #2619 Added get current bot application information (9ef5a76)
- #2649 Added `SafetyAlertsChannelId` to `IGuild` (2112e1f)
- #2696 Added support for the new username system (df717e6)
- #2620 Added support for filtering audit log entries by after id (529fe3d)
- #2622 Added default thread ratelimit in guild text channels (f9c8530)

### Fixed
- #2628 Allow creating stickers with no description (c950106)
- #2682 Fixed missing `JsonProperty` attributes in params for modifying automod rules (9df4438)
- #2702 Fixed `TypingStartedEvent` model to support Clyde (547bc92)
- #2652 Fixed streams being incorrectly disposed in `DefaultRestClient` (84431de)
- #2708 Fixed threads not getting removed from cache on `THREAD_DELETE` (fe4130d)
- #2688 Fixed some internal classes being public (9ddd922)
- #2687 Allow more than 1 action to be registered per automod rule (200bed7)
- #2683 Fixed mention limit, regex patterns & allowlist for automod rules (a41fa62)
- #2663 Fixed T3 file upload size to be 25MiB (98f974d)
- #2662 Fixed duplicate `SlashCommandExecuted` event on failed result (fe9657a)
- #2651 Fixed name translations (`NameLocalizations`) for `ChoiceAttribute` (75ae488)
- #2647 Fixed `ChannelPermissions.All()` exception on forums (6265dd5)
- #2642 Enforce rate limits on 429s (69cce5b)

### Misc
- #2601 Add missing json error (a8ae1e9)
- #2609 Refactor `SendMessageAsync` & `SendFile(s)Async` methods & modify webhook message attachments (898ee56)
- #2646 Create forum channels with default layout (0720049)
- #2666 Switch `channel` to `channel_id` in interactions (d5ba7d2)
- #2670 Disable FEC in OPUS (2be9b00)

## [3.10.0] - 2023-03-27

### Added
- #2507 Added selects v2 support (48fb1b5)
- #2509 Added `SendFileAsync` to `UserExtensions` (25cfb88)
- #2510 Added missing invite guild properties & welcome screen support (7c535b9)
- #2520 Added `ClientDisconnect` event for audio client. (4cad546)
- #2521 Added `OwnerId` to `IThreadChannel` and `RestThreadChannel`
- #2521 Added `ActiveDeveloper` to `UserProperies`
- #2523 Added emojis from Emoji 13.0, Emoji 13.1 and Emoji 14.0 (23b2822)
- #2528 Added wildcard length quantifiers and `TreatAsRegex` property
- #2530 Added application Role Connections Metadata support (37d868f)
- #2531 Added age restricted (NSFW) application commands support (56b1a93)
- #2537 Added `RemoveModulesFromGuildAsync` (7acbcfd)
- #2560 Added app Command channel target permission (2616d35)
- #2562 Added new message types & error codes (a3ee855)
- #2564 Added `RespondWithModal()` which accepts an `IModal` instance as template (e7bda0f)
- #2571 Added guild member flags (7d8d6ec)
- #2573 Added missing VoiceChannel properties (1e21a6e)
- #2574 Added `GetCurrentUser` method to `DiscordRestClient` (e3da96f)
- #2574 Added `GetCurrentUserGuildMemberAsync` method to `DiscordRestClient` and `RestGuild` (e3da96f)
- #2574 Added `GuildFeatures` property to `IUserGuild` (e3da96f)
- #2575 Added `Thread` property to `RestMessage` (91e2084)
- #2578 Added AutoMod support (673b02d)
- #2579 Added `Thread` property to `IUserMessage` (0998285)
- #2580 Added thread user member & thread members pagination (b085621)
- #2582 Added `IIntegrationChannel` (75f3c91)
- #2582 Added support for creating webhooks in forums (75f3c91)
- #2590 Added `FollowAnnouncementChannelAsync` to `INewsChannel` (bb056df)
- #2591 Added support for silent mentions (2ea8af1)
- #2599 Added `AddModalCommand` to `ModalBuilder` with option to specify callback (50e0e59)
- #2605 Added `GuildFeatures` to `GuildProperties` (177df2c)
- #2603 Added support for creating forum posts with `DiscordWebhookClient` (e69e27a)
- #2610 Added Text-In-Stage support & missing `IVoiceChannel` properties (76bb901)
- #2613 Added AutoMod custom block message (709364a)
- #2615 Added missing invite properties (abfa8d1)

### Fixed
- #2483 Fixed an NRE when event was changed from in channel to external or vice versa (493f604)
- #2526 Fixed `GetActiveThreadsAsync` & add it to `ITextChannel` (bd2f719)
- #2528 Fixed catastrophic backtracking (3b107c2)
- #2532 Fixed permissions on deleteMissing=false (274be5d)
- #2540 Fixed `reason` not being included in audit logs when calling `AddBanAsync` and `BanAsync` (e71faf7)
- #2541 Fixed `CreatePost` methods for `IForumChannel` (a53f1df)
- #2549 Fixed `NullReferenceException` caused by Options being undefined. (02d3ce6)
- #2557 Fixed `customId` template generator to escape regex metachars used in â€¦ (1602437)
- #2577 Fixed `GetEventCoverImageUrl` (0af835a)
- #2600 Fixed gateway deadlock (0c27395)
- #2606 Fixed AutoMod action executed event message being null (7b321af)

### Misc
- #2542 Resolve inconsistencies with components (f224eb0)
- #2546 Argument handling with thread tags(c67642a)
- #2551 Format the project with 'dotnet format' (257f246)
- #2561 Remove outdated checks (94996e7)
- #2572 Reduce `DateTime` Boilerplate from `LogMessage` (14b2901)
- #2614 Optimize `Add/RemoveRolesAsync` methods (11481a1)
- #2617 Update IP discovery to send 74 bytes (66e6329)

## [3.9.0] - 2022-12-23
### Added

- #2469 Add missing properties in forum & thread channels (01ae904)
- #2501 Add new discord stuff (ed38635)
- #2521 Add missing property & new stuff (82b772a)
- #2520 Implemented ClientDisconnect event for audio client. (4cad546)
- #2509 Add SendFiles to UserExtensions ( 4cad546) 
- #2528 Implement wildcard lenght quantifiers, TreatAsRegex property and solve catastrpohic backtracking (25cfb88)
- #2531 Add Age restricted (NSFW) application commands support (60956c7)

### Fixed

- #2500 Fix duplicated members of DiscordErrorCode (6712ef4)
- #2468 Fix TimestampTag being sadge (bc89d3c)
- #2497 Avoid throwing on missing Application (7077c44)
- #2485 Fixed an oversight clearing session data upon any disconnect. (c7ac59d)
- #2526 Fix `GetActiveThreadsAsync` & add it to `ITextChannel` (bd2f719)
- #2535 Fix deploy.yml (20d8fdf) 

### Misc

- #2471 Update samples to use `MessageContent` intent & update `v2 => v3 guide` (a4d34f6)
- #2505 Update events.cs (ea039b8)
- #2467 Update license and icon nuspec props (11ed0ff)
- #2306 Command execution code rework & TypeConverters auto-scope fix (6869817)
- #2534 Fully qualify SlashCommandBuilder namespace ( 3b107c2)

## [3.8.1] - 2022-09-12
### Added

- #2437 Added scheduled event types to AuditLog ActionTypes (fca9c6b)
- #2423 Added support for resume gateway url (d4c533a)

### Fixed

- #2443 Fixed typos of word length (adf012d)
- #2438 Fixed http query symbol in ModifyWebhookMessageAsync (0aa381d)
- #2444 Fixed BulkOverwriteCommands NRE (9feb703)
- #2417 Fixed CommandService RemoveModuleMethod not removing modules (fca9c6b)
- #2345 Fixed EmbedBuilder.Length NRE (11ece4b)
- #2453 Fixed NRE on SlashCommandBuilder.Build method (5073afa)
- #2457 Fixed typo in SlashCommandBuilder.AddNameLocalizationMethod (1b01fed)

### Misc

- #2462 Add additional checks for gateway event warnings (b45b152)
- #2448 Bump to Discord API V10 (fbc5ad4)
- #2451 Return a list instead of an array in GetModulePath and GetChoicePath methods (370bdfa)
- #2453 Update app commands regex and fix localization on app context commands (3dec99f)
- #2333 Update package logo (2b86a79)

## [3.8.0] - 2022-08-27
### Added
- #2384 Added support for the WEBHOOKS_UPDATED event (010e8e8)
- #2370 Add async callbacks for IModuleBase (503fa75)
- #2367 Added DeleteMessagesAsync for TIV and added remaining rate limit in client log (f178660)
- #2379 Added Max/Min length fields for ApplicationCommandOption (e551431)
- #2369 Added support for using `RespondWithModalAsync<IModal>()` without prior IModal declaration (500e7b4)
- #2347 Added Embed field comparison operators (89a8ea1)
- #2359 Added support for creating lottie stickers (32b03c8)
- #2395 Added App Command localization support and `ILocalizationManager` to IF (39bbd29)

### Fixed
- #2425 Fix missing Fact attribute in ColorTests (92215b1)
- #2424 Fix IGuild.GetBansAsync() (b7b7964)
- #2416 Fix role icon & emoji assignment (b6b5e95)
- #2414 Fix NRE on RestCommandBase Data (02bc3b7)
- #2421 Fix placeholder length being hardcoded (8dfe19f)
- #2352 Fix issues related to the absence of bot scope (1eb42c6)
- #2346 Fix IGuild.DisconnectAsync(IUser) not disconnecting users (ba02416)
- #2404 Fix range of issues presented by 3rd party analyzer (902326d)
- #2409 Removes GroupContext from requirecontext (b0b8167)

### Misc
- #2366 Fixed typo in ChannelUpdatedEvent's documentation (cfd2662)
- #2408 Fix sharding sample throwing at appcommand registration (519deda)
- #2420 Fix broken code snippet in dependency injection docs (ddcf68a)
- #2430 Add a note about DontAutoRegisterAttribute (917118d)
- #2418 Update xmldocs to reflect the ConnectedUsers split (65b98f8)
- #2415 Adds missing DI entries in TOC (c49d483)
- #2407 Introduces high quality dependency injection documentation (6fdcf98)
- #2348 Added  `RequiredInput`  attribute to example in int.framework intro (ee6e0ad)
- #2385 Add ServerStarter.Host to deployment.md (06ed995)
- #2405 Add a note about  `IgnoreGroupNames`  to IF docs (cf25acd)
- #2356 Makes voice section about precompiled binaries more visible (e0d68d4 )
- #2405 IF intro docs improvements (246282d)
- #2406 Labs deprecation & readme/docs edits (bf493ea)

## [3.7.2] - 2022-06-02
### Added
- #2328 Add method overloads to InteractionService (0fad3e8)
- #2336 Add support for attachments on interaction response type 7 (35db22e)
- #2338 AddOptions no longer has an uneeded restriction, added AddOptions to SlashCommandOptionBuilder (3a37f89)

### Fixed
- #2342 Disable TIV restrictions for rollout of TIV (7adf516)

## [3.7.1] - 2022-05-27
### Added
- #2325 Add missing interaction properties (d3a693a)
- #2330 Add better call control in ParseHttpInteraction (a890de9)

### Fixed
- #2329 Voice perms not retaining text perms. (712a4ae)
- #2331 NRE with Cacheable.DownloadAsync() (e1f9b76)

## [3.7.0] - 2022-05-24
### Added
- #2269 Text-In-Voice (23656e8)
- #2281 Optional API calling to RestInteraction (a24dde4)
- #2283 Support FailIfNotExists on MessageReference (0ec8938)
- #2284 Add Parse & TryParse to EmbedBuilder & Add ToJsonString extension (cea59b5)
- #2289 Add UpdateAsync to SocketModal (b333de2)
- #2291 Webhook support for threads (b0a3b65)
- #2295 Add DefaultArchiveDuration to ITextChannel (1f01881)
- #2296 Add `.With` methods to ActionRowBuilder (13ccc7c)
- #2307 Add Nullable ComponentTypeConverter and TypeReader (6fbd396)
- #2316 Forum channels (7a07fd6)

### Fixed
- #2290 Possible NRE in Sanitize (20ffa64)
- #2293 Application commands are disabled to everyone except admins by default (b465d60)
- #2299 Close-stage bucketId being null (725d255)
- #2313 Upload file size limit being incorrectly calculated (54a5af7)
- #2319 Use `IDiscordClient.GetUserAsync` impl in `DiscordSocketClient` (f47f319)
- #2320 NRE with bot scope and user parameters (88f6168)

## [3.6.1] - 2022-04-30
### Added
- #2272 add 50080 Error code (503e720)

### Fixed
- #2267 Permissions v2 Invalid Operation Exception (a8f6075)
- #2271 null user on interaction without bot scope (f2bb55e)
- #2274 Implement fix for Custom Id Segments NRE (0d74c5c)

### Misc
- 3.6.0 (27226f0)


## [3.6.0] - 2022-04-28
### Added
- #2136 Passing CustomId matches into contexts (4ce1801)
- #2222 V2 Permissions (d98b3cc)

### Fixed
- #2260 Guarding against empty descriptions in `SlashCommandBuilder`/`SlashCommandOptionBuilder` (0554ac2)
- #2248 Fix SocketGuild not returning the AudioClient (daba58c)
- #2254 Fix browser property (275b833)

## [3.5.0] - 2022-04-05

### Added
- #2204 Added config option for bidirectional formatting of usernames (e38104b)
- #2210 Add a way to remove type readers from the interaction/command service. (7339945)
- #2213 Add global interaction post execution event. (a744948)
- #2223 Add ban pagination support (d8757a5)
- #2201 Add missing interface methods to IComponentInteraction (741ed80)
- #2226 Add an action delegate parameter to `RespondWithModalAsync<T>()` for modifying the modal (d2118f0)
- #2227 Add RespondWithModal methods to RestInteractinModuleBase (1c680db)

### Fixed
- #2168 Fix Integration model from GuildIntegration and added INTEGRATION gateway events (305d7f9)
- #2187 Fix modal response failing (d656722)
- #2188 Fix serialization error on thread creation timestamp. (d48a7bd)
- #2209 Fix GuildPermissions.All not including newer permissions (91d8fab)
- #2219 Fix ShardedClients not pushing PresenceUpdates (c4131cf)
- #2225 Fix GuildMemberUpdated cacheable `before` entity being incorrect (bfd0d9b)
- #2217 Fix gateway interactions not running without bot scope. (8522447)

### Misc
- #2193 Update GuildMemberUpdated comment regarding presence (82473bc)
- #2206 Fixed typo (c286b99)
- #2216 Fix small typo in modal example (0439437)
- #2228 Correct minor typo (d1cf1bf)

## [3.4.1] - 2022-03-9

### Added
- #2169 Component TypeConverters and CustomID TypeReaders (fb4250b)
- #2180 Attachment description and content type (765c0c5)
- #2162 Add configuration toggle to suppress Unknown dispatch warnings (1ba96d6)
- #2178 Add 10065 Error code (cc6918d)

### Fixed
- #2179 Logging out sharded client throws (24b7bb5)
- #2182 Thread owner always returns null (25aaa49)
- #2165 Fix error with flag params when uploading files. (a5d3add)
- #2181 Fix ambiguous reference for creating roles (f8ec3c7)

## [3.4.0] - 2022-3-2

### Added
- #2146 Add FromDateTimeOffset in TimestampTag (553055b)
- #2062 Add return statement to precondition handling (3e52fab)
- #2131 Add support for sending Message Flags (1fb62de)
- #2137 Add self_video to VoiceState (8bcd3da)
- #2151 Add Image property to Guild Scheduled Events (1dc473c)
- #2152 Add missing json error codes (202554f)
- #2153 Add IsInvitable and CreatedAt to threads (6bf5818)
- #2155 Add Interaction Service Complex Parameters (9ba64f6)
- #2156 Add Display name support for enum type converter (c800674)

### Fixed
- #2117 Fix stream access exception when ratelimited (a1cfa41)
- #2128 Fix context menu comand message type (f601e9b)
- #2135 Fix NRE when ratelimmited requests don't return a body (b95b942)
- #2154 Fix usage of CacheMode.AllowDownload in channels (b3370c3)

### Misc
- #2149 Clarify Users property on SocketGuildChannel (5594739)
- #2157 Enforce valid button styles (507a18d)

## [3.3.2] - 2022-02-16

### Fixed

- #2116 Fix null rest client in shards

## [3.3.1] - 2022-02-16

### Added

- #2107 Add DisplayName property to IGuildUser. (abfba3c)

### Fixed

- #2110 Fix incorrect ratelimit handles for 429's (b2598d3)
- #2094 Fix ToString() on CommandInfo (01735c8)
- #2098 Fix channel being null in DMs on Interactions (7e1b8c9)
- #2100 Fix crosspost ratelimits (fad217e)
- #2108 Fix being unable to modify AllowedMentions with no embeds set. (169d54f)
- #2109 Fix unused creation of REST clients for DiscordShardedClient shards. (6039378)

### Misc

- #2099 Update interaction summaries (503d32a)

## [3.3.0] - 2022-02-09

### Added

- #2087 Add modals (c8f175e)
- #2088 Add support for slash command attachment type (33efd89)

### Fixed

- #2091 Fix modifying attachments on interactions and extend the module base (97e54e1)
- #2076 Fix rest message components cannot pass through execute method (b45aebf)
- #2077 Fix clone being created on updated entity (7d6f4f3)
- #2092 Fix Current user null on reconnect (b424bb2)
- #2089 Fix guild feature enum (d142710)
- #2090 Fix attempts to fetch channels in interactions (6290f75)

### Misc

- #1713 Simplify code in DiscordComparers (43b20bc)
- #2079 Add IEnumerable collection parameters instead of arrays for MessageExtensions (75e94fe)

## [3.2.1] - 2022-01-30

### Added

- #2055 Add IThreadUser interface (3475bd8)

### Fixed

- #2030 Fix logging a TaskCanceledException out of users control (82f3879)
- #2064 Fix markdown formatting in Glossary (f525131)
- #2066 Fix Channel Types Attribute (1917961)
- #2071 Fix attempt to get application info for non-bot tokens (37ada59)
- #2072 Fix thread member download on create (09eb9fa)
- #2063 Fix stream position not being zero when uploading files (6dada66)

### Misc

- #2059 Update HttpException to display inner-errors on the HttpException.Message for better debugging (5773b8e)

## [3.2.0] - 2022-01-15

### Added

- #2015 Add user-built AddCommand overloads to ModuleBuilder (68e3bed)
- #2016 Add Construct Method to InteractionModuleBase and Fix NRE on User-Built Module Creation (4ed4718)
- #2035 Add GetChannelType extension method (64423a0)
- #2036 Add missing guild features (7075d4e)
- #2037 Add default ratelimit callback (4d9389b)
- #2038 Add AddRow and WithRows to ComponentBuilder (3429cf9)
- #2040 Add user locale & guild locale in interactions (2731e20)
- #2041 Add search methods to InteractionService (9a52d05)

### Fixed

- #1562 Fix OperationCancelledException and add IAsyncEnumerable to wait without thread blocking (cd36bb8)
- #2014 Fix InteractionContext.Guild (944a0de)
- #2023 Fix ModifyAsync when only modifying a message's flags (e3eb0a9)
- #2025 Fix IsTopLevelCommand returning the wrong value (4b7cda0)
- #2031 Fix DefaultChannel to exclude threads (a06ba9f)
- #2033 Fix Interaction delete original response throwing 404 (291d1e4)
- #2034 Fix exception when uploading files with non seekable streams (7f4feea)
- #2044 Fix ApplicationId not being used for interactions (c682564)
- #2045 Fix argument null exception on Message Create (a07531f)
- #2046 Fix ThreadMember null ref in constructor (bcd5fa4)

### Misc

- #2018 Match argument defaults with IDiscordInteraction on SocketInteraction (093e548)
- #2024 Remove .ToLower from group name registration (9594ccc)
- #2042 Move gateway intents warning to ready event (3633929)

## [3.1.0] - 2021-12-24

### Added

- #1996 Add nullable type converter to Interaction service (ccc365e)
- #1998 Add before and after execute async (9f124b2)
- #2001 Add MaxUploadLimit to guilds (7745558)
- #2002 Add RTCRegion to voice channels (2a416a3)
- #2003 Add Guilduser timeouts and MODERATE_MEMBERS permission (144741e)

### Fixed

- #1976 fix guild scheduled events update (8daa0b6)
- #1977 fix thread member nre (5d43fe6)
- #1980 fix requireRole attribute of interaction service (a2f57f8)
- #1990 Fix images path for select menu section (a8b5506)
- #1992 fix images; fix closing brace on cs ref (fb52525)
- #1993 Fix CommandExecuted not invoked on failed parse (82bb3e4)
- #1995 Fixed file being disposed on upload (ad20e03)
- #1999 Fix SocketGuildUser being changed to SocketGlobalUser in UserLeft (5446bfe)
- Fix voice codes namespace (768a0a9)

### Misc

- #1994 Make HasResponded public and add it to IDiscordInteraction (1fbcbb8)
- #1997 Make module service scopes optional (cb1aad3)

## [3.0.0] - 2021-12-13

### Added

- #1152 Add characters commonly use in links to Sanitize (b9274d1)
- #1518 Add default nullable enum typereader (f7a07ae)
- #1700 Added Implementation of ValidateAndGetBestMatch (3cd9f39)
- #1767 Add method to clear guild user cache (19a66bf)
- #1847 Bump API version to 9 (06a64b7)
- #1848 Remove obsolete sync voice regions methods and properties (ed8e573)
- #1851 Remove DM cache and fix references (7a201e9)
- #1860 Remove /users/@me call for socket and rework sharded client a bit (384ad85)
- #1863 Change GuildMemberUpdate before state to cacheable (c2e87f5)
- #1666 Added negative TimeSpan handling (6abdfcb)
- #1861 Add MaxBitrate to the interface (e0dbe7c)
- #1865 Add null check to AllowedMentions.ToModel() (3cb662f)
- #1879 Add Name property to Teams (c5b4b64)
- #1890 Add default avatar to WithAuthor extension (c200861)
- #1896 IVoiceChannel implements IMentionable (3395700)
- #1923 Add Interaction Support (933ea42)
- #1923 Add Application commands (933ea42)
- #1923 Add Message Components (933ea42)
- #1923 Add Thread Channels (933ea42)
- #1923 Add Stage Channels (933ea42)
- #1923 Add Guild Events (933ea42)
- #1923 Revamped Stickers (933ea42)
- #1923 Add TimestampTag (933ea42)
- #1923 Add name property to teams (933ea42)
- #1923 Add url validation on embeds (933ea42)
- #1923 Add NsfwLevel to Guilds (933ea42)
- #1923 Add helpers to Emoji for parsing (933ea42)
- #1923 Add banner and accent color to guild users (933ea42)
- #1923 Add RatelimitCallback to RequestOptions (933ea42)
- #1923 Add Emoji to roles (933ea42)
- #1923 Add UseInteractionSnowflakeDate to config (933ea42)
- #1923 Add checks for gateway intent in some methods (933ea42)
- #1923 Add SendFilesAsync to channels (933ea42)
- #1923 Add Attachments property to MessageProperties (933ea42)
- #1942 Add multi-file upload to webhooks (bc440ab)
- #1943 Handle bidirectional usernames (10afd96)
- #1945 Updated socket presence and add new presence event (9d6dc62)
- #1948 Added warnings on invalid gateway intents (51e06e9)
- #1949 Add default application games (82276e3)
- #1950 Add custom setter to Group property of ModuleBuilder to automatically invoke AddAliases (ba656e9)
- #1958 Add Discord.Interactions framework (aa6bb5e)

### Fixed

- #1832 Grab correct Uses value for vanity urls (8ed8714)
- #1849 Remove obsolete methods and properties (70aab6c)
- #1850 Create DM channel with id and author alone (95bae78)
- #1853 Fire GuildMemberUpdated without cached user (d176fef)
- #1854 Gateway events for DMs (a7ff6ce)
- #1858 MessageUpdated without author (8b29e0f)
- #1859 Fix missing AddRef and related (de7f9b5)
- #1862 Message update without author (fabe034)
- #1864 ApiClient.CurrentUser being null (08507c0)
- #1871 Fix empty role list if not present (f47001a)
- #1872 Connection deadlock when trying to Send and Disconnect (97d90b9)
- #1873 Remove OperationCanceledException handling in connecting logic (7cf8499)
- #1876 Fix SocketMessage type always being default (ac52a11)
- #1877 Fix RestMessage type always being default (22bb1b0)
- #1886 Change embed description max length to 4096 (8349cd7)
- #1923 Fix ReactionAdded cached parameters (933ea42)
- #1923 Fixed GuildMemberUpdated cached parameters (933ea42)
- #1923 Fixed UserIsTypeing cached parameters (933ea42)
- #1941 Fix Emote.TryParse (900c1f4)
- #1946 Fix NRE when adding parameters in ModuleBuilders (143ca6d)
- #1947 ShardedClient's CurrentUser interface property being null (d5f5ae1)

### Misc

- #1852 Internal change to GetOrCreateUser (dfaaa21)
- #1923 Make Hierarchy a IGuildUser property (933ea42)
- #1923 Fixed gateway serialization to include nulls for API v9 (933ea42)
- #1923 Removed error log for gateway reconnects (933ea42)

## [2.4.0] - 2021-05-22

### Added

- #1726 Add stickers (91a9063)
- #1753 Webhook message edit & delete functionality (f67cd8e)
- #1757 Add ability to add/remove roles by id (4c9910c)
- #1781 Add GetEmotesAsync to IGuild (df23d57)
- #1801 Add missing property to MESSAGE_REACTION_ADD event (0715d7d)
- #1828 Add methods to interact with reactions without a message object (5b244f2)
- #1830 Add ModifyMessageAsync to IMessageChannel (365a848)
- #1844 Add Discord Certified Moderator user flag (4b8d444)

### Fixed

- #1486 Add type reader when entity type reader exists (c46daaa)
- #1835 Cached message emoji cleanup at MESSAGE_REACTION_REMOVE_EMOJI (8afef82)

### Misc

- #1778 Remove URI check from EmbedBuilder (25b04c4)
- #1800 Fix spelling in SnowflakeUtils.FromSnowflake (6aff419)

## [2.3.1] - 2021-03-10

### Fixed

- #1761 Deadlock in DiscordShardedClient when Ready is never received (73e5cc2)
- #1773 Private methods aren't added as commands (0fc713a)
- #1780 NullReferenceException in pin/unpin audit logs (f794163)
- #1786 Add ChannelType property to ChannelInfo audit log (6ac5ea1)
- #1791 Update Webhook ChannelId from model change (d2518db)
- #1794 Audit log UserId can be null (d41aeee)

### Misc

- #1774 Add remark regarding CustomStatus as the activity (51b7afe)

## [2.3.0] - 2021-01-28

### Added

- #1491 Add INVITE_CREATE and INVITE_DELETE events (1ab670b)
- #1520 Support reading multiple activities (421a0c1)
- #1521 Allow for inherited commands in modules (a51cdf6)
- #1526 Add Direction.Around to GetMessagesAsync (f2130f8)
- #1537 Implement gateway ratelimit (ec673e1)
- #1544 Add MESSAGE_REACTION_REMOVE_EMOJI and RemoveAllReactionsForEmoteAsync (a89f076)
- #1549 Add GetUsersAsync to SocketGuild (30b5a83)
- #1566 Support Gateway Intents (d5d10d3)
- #1573 Add missing properties to Guild and deprecate GuildEmbed (ec212b1)
- #1581 Add includeRoleIds to PruneUsersAsync (a80e5ff)
- #1588 Add GetStreams to AudioClient (1e012ac)
- #1596 Add missing channel properties (2d80037)
- #1604 Add missing application properties (including Teams) (10fcde0)
- #1619 Add "View Guild Insights" to GuildPermission (2592264)
- #1637 Added CultureInvariant RegexOption to WebhookUrlRegex (e3925a7)
- #1659 Add inline replies (e3850e1)
- #1688 Send presence on Identify payload (25d5d36)
- #1721 Add role tags (6a62c47)
- #1722 Add user public flags (c683b29)
- #1724 Add MessageFlags and AllowedMentions to message modify (225550d)
- #1731 Add GuildUser IsPending property (8b25c9b)
- #1690 Add max bitrate value to SocketGuild (aacfea0)

### Fixed

- #1244 Missing AddReactions permission for DM channels. (e40ca4a)
- #1469 unsupported property causes an exception (468f826)
- #1525 AllowedMentions and AllowedMentionTypes (3325031)
- #1531 Add AllowedMentions to SendFileAsync (ab32607)
- #1532 GuildEmbed.ChannelId as nullable per API documentation (971d519)
- #1546 Different ratelimits for the same route (implement discord buckets) (2f6c017)
- #1548 Incomplete Ready, DownloadUsersAsync, and optimize AlwaysDownloadUsers (dc8c959)
- #1555 InvalidOperationException at MESSAGE_CREATE (bd4672a)
- #1557 Sending 2 requests instead of 1 to create a Guild role. (5430cc8)
- #1571 Not using the new domain name. (df8a0f7)
- #1578 Trim token before passing it to the authorization header (42ba372)
- #1580 Stop TaskCanceledException from bubbling up (b8fa464)
- #1599 Invite audit log without inviter (b95b95b)
- #1602 Add AllowedMentions to webhooks (bd4516b)
- #1603 Cancel reconnection when 4014 (f396cd9)
- #1608 Voice overwrites and CategoryId remarks (43c8fc0)
- #1614 Check error 404 and return null for GetBanAsync (ae9fff6)
- #1621 Parse mentions from message payload (366ca9a)
- #1622 Do not update overwrite cache locally (3860da0)
- #1623 Invoke UserUpdated from GuildMemberUpdated if needed (3085e88)
- #1624 Handle null PreferredLocale in rare cases (c1d04b4)
- #1639 Invite and InviteMetadata properties (dd2e524)
- #1642 Add missing permissions (4b389f3)
- #1647 handicap member downloading for verified bots (fa5ef5e)
- #1652 Update README.MD to reflect new discord domain (03b831e)
- #1667 Audio stream dispose (a2af985)
- #1671 Crosspost throwing InvalidOperationException (9134443)
- #1672 Team is nullable, not optional (be60d81)
- #1681 Emoji url encode (04389a4)
- #1683 SocketGuild.HasAllMembers is false if a user left a guild (47f571e)
- #1686 Revert PremiumSubscriptionCount type (97e71cd)
- #1695 Possible NullReferenceException when receiving InvalidSession (5213916)
- #1702 Rollback Activities to Game (9d7cb39)
- #1727 Move and fix internal AllowedMentions object (4a7f8fe)
- limit request members batch size (084db25)
- UserMentions throwing NullRef (5ed01a3)
- Wrong author for SocketUserMessage.ReferencedMessage (1e9b252)
- Discord sends null when there's no team (05a1f0a)
- IMessage.Embeds docs remarks (a4d32d3)
- Missing MessageReference when sending files (2095701)

### Misc

- #1545 MutualGuilds optimization (323a677)
- #1551 Update webhook regex to support discord.com (7585789)
- #1556 Add SearchUsersAsync (57880de)
- #1561 Minor refactor to switch expression (42826df)
- #1576 Updating comments for privileged intents (c42bfa6)
- #1678 Change ratelimit messages (47ed806)
- #1714 Update summary of SocketVoiceChannel.Users (e385c40)
- #1720 VoiceRegions and related changes (5934c79)
- Add updated libraries for LastModified (d761846)
- Add alternative documentation link (accd351)
- Temporarily disable StyleCops until all the fixes are impl'd (36de7b2)
- Remove redundant CreateGuildRoleParams (3df0539)
- Add minor tweaks to DiscordSocketConfig docs strings (2cd1880)
- Fix MaxWaitBetweenGuildAvailablesBeforeReady docs string (e31cdc7)
- Missing summary tag for GatewayIntents (3a10018)
- Add new method of role ID copy (857ef77)
- Resolve inheritdocs for IAttachment (9ea3291)
- Mark null as a specific langword in summary (13a41f8)
- Cleanup GatewayReconnectException docs (833ee42)
- Update Docfx.Plugins.LastModified to v1.2.4 (28a6f97)
- Update framework version for tests to Core 3.1 to comply with LTS (4988a07)
- Move bulk deletes remarks from <summary> to <remarks> (62539f0)

## [2.2.0] - 2020-04-16

### Added

- #1247 Implement Client Status Support (9da11b4)
- #1310 id overload for RemoveReactionAsync (c88b1da)
- #1319 BOOST (faf23de)
- #1326 Added a Rest property to DiscordShardedClient (9fede34)
- #1348 Add Quote Formatting (265da99)
- #1354 Add support for setting X-RateLimit-Precision (9482204)
- #1355 Provide ParameterInfo with error ParseResult (3755a02)
- #1357 add the "Stream" permission. (b00da3d)
- #1358 Add ChannelFollowAdd MessageType (794eba5)
- #1369 Add SelfStream voice state property (9bb08c9)
- #1372 support X-RateLimit-Reset-After (7b9029d)
- #1373 update audit log models (c54867f)
- #1377 Support filtering audit log entries on user, action type, and before entry id (68eb71c)
- #1386 support guild subscription opt-out (0d54207)
- #1387 #1381 Guild PreferredLocale support (a61adb0)
- #1406 CustomStatusGame Activity (79a0ea9)
- #1413 Implemented Message Reference Property (f86c39d)
- #1414 add StartedAt, EndsAt, Elapsed and Remaining to SpotifyGame. (2bba324)
- #1432 Add ability to modify the banner for guilds (d734ce0)
- suppress messages (cd28892)

### Fixed

- #1318 #1314 Don't parse tags within code blocks (c977f2e)
- #1333 Remove null coalescing on ToEmbedBuilder Color (120c0f7)
- #1337 Fixed attempting to access a non-present optional value (4edda5b)
- #1346 CommandExecuted event will fire when a parameter precondition fails like what happens when standard precondition fails. (e8cb031)
- #1371 Fix keys of guild update audit (b0a595b)
- #1375 Use double precision for X-Reset-After, set CultureInfo when parsing numeric types (606dac3)
- #1392 patch todo in NamedTypeReader (0bda8a4)
- #1405 add .NET Standard 2.1 support for Color (7f0c0c9)
- #1412 GetUsersAsync to use MaxUsersPerBatch const as limit instead of MaxMessagesPerBatch. (5439cba)
- #1416 false-positive detection of CustomStatusGame based on Id property (a484651)
- #1418 #1335 Add isMentionable parameter to CreateRoleAsync in non-breaking manner (1c63fd4)
- #1421 (3ff4e3d)
- include MessageFlags and SuppressEmbedParams (d6d4429)

### Changed

- #1368 Update ISystemMessage interface to allow reactions (07f4d5f)
- #1417 fix #1415 Re-add support for overwrite permissions for news channels (e627f07)
- use millisecond precision by default (bcb3534)

### Misc

- #1290 Split Unit and Integration tests into separate projects (a797be9)
- #1328 Fix #1327 Color.ToString returns wrong value (1e8aa08)
- #1329 Fix invalid cref values in docs (363d1c6)
- #1330 Fix spelling mistake in ExclusiveBulkDelete warning (c864f48)
- #1331 Change token explanation (0484fe8)
- #1349 Fixed a spelling error. (af79ed5)
- #1353 [ci skip] Removed duplicate "any" from the readme (15b2a36)
- #1359 Fixing GatewayEncoding comment (52565ed)
- #1379 September 2019 Documentation Update (fd3810e)
- #1382 Fix .NET Core 3.0 compatibility + Drop NS1.3 (d199d93)
- #1388 fix coercion error with DateTime/Offset (3d39704)
- #1393 Utilize ValueTuples (99d7135)
- #1400 Fix #1394 Misworded doc for command params args (1c6ee72)
- #1401 Fix package publishing in azure pipelines (a08d529)
- #1402 Fix packaging (65223a6)
- #1403 Cache regex instances in MessageHelper (007b011)
- #1424 Fix the Comparer descriptions not linking the type (911523d)
- #1426 Fix incorrect and missing colour values for Color fields (9ede6b9)
- #1470 Added System.Linq reference (adf823c)
- temporary sanity checking in SocketGuild (c870e67)
- build and deploy docs automatically (2981d6b)
- 2.2.0 (4b602b4)
- target the Process env-var scope (3c6b376)
- fix metapackage build (1794f95)
- copy only \_site to docs-static (a8cdadc)
- do not exit on failed robocopy (fd204ee)
- add idn debugger (91aec9f)
- rename IsStream to IsStreaming (dcd9cdd)
- feature (40844b9)

## [2.1.1] - 2019-06-08

### Fixed

- #994: Remainder parameters now ignore character escaping, as there is no reason to escape characters here (2e95c49)
- #1316: `Emote.Equals` now pays no respect to the Name property, since Discord's API does not care about an emote's name (abf3e90)
- #1317: `Emote.GetHashCode` now pays no respect to the Name property, see above (1b54883)
- #1323: Optionals will no longer claim to be specified when a reaction message was not cached (1cc5d73)
- Log messages sourcing from REST events will no longer be raised twice (c78a679)
- News embeds will be processed as `EmbedType.Unknown`, rather than throwing an error and dropping the message (d287ed1)

### Changed

- #1311: Members may now be disconnected from voice channels by passing `null` as `GuildUserProperties.Channel` (fc48c66)
- #1313: `IMessage.Tags` now includes the EveryoneRole on @everyone and @here mentions (1f55f01)
- #1320: The maximum value for setting slow-mode has been updated to 6 hours, per the new API limit (4433ca7)

### Misc

- This library's compatibility with Semantic Versioning has been clarified. Please see the README (4d7de17)
- The depency on System.Interactive.Async has been bumped to `3.2.0` (3e65e03)

## [2.1.0] - 2019-05-18

### Added

- #1236: Bulk deletes (for messages) may now be accessed via the `MessagesBulkDeleted` event (dec353e)
- #1240: OAuth applications utilizing the `guilds.join` scope may now add users to guilds through any client (1356ea9)
- #1255: Message and attachment spoilers may now be set or detected (f3b20b2)
- #1260: DiscordWebhookClient may be created from a Webhook URL (f2113c7)
- #1261: A `GetCategoryChannel` helper may now be used to retrieve category channels directly from socket guilds (e03c527)
- #1263: "user joined the guild" messages are now supported (00d3f5a)
- #1271: AuthorID may now be retrieved from message delete audit log entries (1ae4220)
- #1293: News Channels are now supported 📰 (9084c42)
- `ExclusiveBulkDelete` configuration setting can be used to control bulk delete event behavior (03e6401)

### Removed

- #1294: The `IGuildUser` overload of `EmbedBuilder.WithAuthor` no longer exists (b52b54d)

### Fixed

- #1256: Fetching audit logs no longer raises null reference exceptions when a webhook has been deleted (049b014)
- #1268: Null reference exceptions on `MESSAGE_CREATE` concerning partial member objects no longer occur (377622b)
- #1278: The token validator now internally pads tokens to the proper length (48b327b)
- #1292: Messages now properly initialize empty collections (b2ebc03)
- The `DiscordSocketRestClient` is now properly initialized (a44c13a)
- Exceptions in event handlers are now always logged (f6e3200)

### Changed

- #1305: Token validation will fail when tokens contain whitespace (bb61efa)

### Misc

- #1241: Added documentation samples for Webhooks (655a006)
- #1243: Happy new year 🎉 (0275f7d)
- #1257: Improved clarity in comments in the command samples (2473619)
- #1276: Documentation uses a relative path for the logo asset (b80f0e8)
- #1303: EmbedBuilder documentation now builds in the correct spot (51618e6)
- #1304: Updated documentation (4309550)
- CI for this project is now powered by Azure DevOps (this is not a sponsored message 🚀) (9b2bc18)
- IDisposableAnalyzers should now be a development dependency (8003ac8)

## [2.0.1] - 2019-01-04

### Fixed

- #1226: Only escape the closing quotation mark of non-remainder strings (65b8c09)
- Commands with async RunModes will now propagate exceptions up to CommandExecuted (497918e)

### Misc

- #1225: Commands sample no longer hooks the log event twice (552f34c)
- #1227: The logo on the docs index page should scale responsively (d39bf6e)
- #1230: Replaced precondition sample on docs (feed4fd)

## [2.0.0] - 2018-12-28

### Added

- #747: `CommandService` now has a `CommandExecuted` event (e991715)
- #765: Parameters may have a name specified via `NameAttribute` (9c81ab9)
- #773: Both socket clients inherit from `BaseSocketClient` (9b7afec)
- #785: Primitives now automatically load a NullableTypeReader (cb0ff78)
- #819: Support for Welcome Message channels (30e867a)
- #835: Emoji may now be managed from a bot (b4bf046)
- #843: Webhooks may now be managed from a bot (7b2ddd0)
- #863: An embed may be converted to an `EmbedBuilder` using the `.ToEmbedBuilder()` method (5218e6b)
- #877: Support for reading rich presences (34b4e5a)
- #888: Users may now opt-in to using a proxy (678a723)
- #906: API Analyzers to assist users when writing their bot (f69ef2a)
- #907: Full support for channel categories (030422f)
- #913: Animated emoji may be read and written (a19ff18)
- #915: Unused parameters may be discarded, rather than failing the command (5f46aef)
- #929: Standard EqualityComparers for use in LINQ operations with the library's entities (b5e7548)
- #934: Modules now contain an `OnModuleBuilding` method, which is invoked when the module is built (bb8ebc1)
- #952: Added 'All' permission set for categories (6d58796)
- #957: Ratelimit related objects now include request information (500f5f4)
- #962: Add `GetRecommendedShardCountAsync` (fc5e70c)
- #970: Add Spotify track support to user Activities (64b9cc7)
- #973: Added `GetDefaultAvatarUrl` to user (109f663)
- #978: Embeds can be attached alongside a file upload (e9f9b48)
- #984, #1089: `VoiceServerUpdate` events are now publically accessible (e775853, 48fed06)
- #996: Added `DeleteMessageAsync` to `IMessageChannel` (bc6009e)
- #1005: Added dedicated `TimeSpan` TypeReader which "doesn't suck" (b52af7a)
- #1009: Users can now specify the replacement behavior or default typereaders (6b7c6e9)
- #1020: Users can now specify parameters when creating channels (bf5275e)
- #1030: Added `IsDeprecated`, `IsCustom` properties to `VoiceRegion` (510f474)
- #1037: Added `SocketUser.MutualGuilds`, various extension methods to commands (637d9fc)
- #1043: `Discord.Color` is now compatible with `System.Drawing.Color` (c275e57)
- #1055: Added audit logs (39dffe8)
- #1056: Added `GetBanAsync` (97c8931)
- #1102: Added `GetJumpUrl()` to messages (afc3a9d)
- #1123: Commands can now accept named parameters (419c0a5)
- #1124: Preconditions can now set custom error messages (5677f23)
- #1126: `Color` now has equality (a2d8800)
- #1159: Guild channels can now by synced with their parent category (5ea1fb3)
- #1165: Bring Guild and Message models up to date with the API (d30d122)
- #1166: Added `GetVoiceRegionsAsync` to `IGuild` (00717cf)
- #1183: Added Add Guild Member endpoint for OAuth clients (8ef5f81)
- #1196: Channel position can now be specified when creating a channel (a64ab60)
- #1198: The Socket client can now access its underlying REST client (65afd37)
- #1213: Added `GuildEmote#CreatorId` (92bf836)
- 'html' variant added to the `EmbedType` enum (42c879c)
- Modules can now be nested in non-module classes (4edbd8d)
- Added `BanAsync` to guild members (1905fde)
- Added the permisison bit for priority speaker (c1d7818)
- All result types can use `FromError` (748e92b)
- Added support for reading/writing slow mode (97d17cf)
- Added markdown format for URLs (f005af3)
- Reactions can now be added to messages in bulk (5421df1)

### Fixed

- #742: `DiscordShardedClient#GetGuildFor` will now direct null guilds to Shard 0 (d5e9d6f)
- #743: Various issues with permissions and inheritance of permissions (f996338)
- #755: `IRole.Mention` will correctly tag the @everyone role (6b5a6e7)
- #768: `CreateGuildAsync` will include the icon stream (865080a)
- #866: Revised permissions constants and behavior (dec7cb2)
- #872: Bulk message deletion should no longer fail for incomplete batch sizes (804d918)
- #923: A null value should properly reset a user's nickname (227f61a)
- #938: The reconnect handler should no longer deadlock during Discord outages (73ac9d7)
- #941: Fix behavior of OverrideTypeReader (170a2e0)
- #945: Fix properties on SocketCategoryChannel (810f6d6)
- #959: Webhooks now use the correct parameter when assigning to the Avatar URL (8876597)
- #966: Correct the implementation of HasFlag and ResolveChannel in permissions (32ebdd5)
- #968: Add missing parameter in WebSocket4Net constructor (8537924)
- #981: Enforce a maximum value when parsing timestamps from Discord (bfaa6fc)
- #993: Null content will no longer null-ref on message sends/edits (55299ff)
- #1003: Fixed ordering of parameters in permissions classes (a06e212)
- #1010: EmbedBuilder no longer produces mutable embeds (2988b38)
- #1012: `Embed.Length` should now yield the correct results (a3ce80c)
- #1017: GetReactionUsersAsync includes query parameters (9b29c00)
- #1022: GetReactionUsersAsync is now correctly paginated (79811d0)
- #1023: Fix/update invite-related behaviors (7022149)
- #1031: Messages with no guild-specific data should no longer be lost (3631886)
- #1036: Fixed cases where `RetryMode.RetryRatelimit` were ignored (c618cb3)
- #1044: Populate the guild in `SocketWebhookUser` (6a7810b)
- #1048: The REST client will now create a full GuildUser object (033d312)
- #1049: Fixed null-ref in `GetShardIdFor` (7cfed7f)
- #1059: Include 'view channel' in voice channel's All permissions set (e764daf)
- #1083: Default type readers will now be properly replaced (4bc06a0)
- #1093: Fixed race condition in audio client authentication (322d46e)
- #1139: Fixed consistency in exceptions (9e9a11d)
- #1151: `GetReactionUsersAsync` now uses the correct pagination constant (c898325)
- #1163: Reaction ratelimits are now placed in the same bucket, treated correctly (5ea1fb3)
- #1186: Webhooks can now send files with embeds correctly (c1d5152)
- #1192: CommandExecuted no longer fires twice for RuntimeResults (10233f3)
- #1195: Channel Create audit log events properly deserialize (dca6c33)
- #1202: The UDP client should no longer be used after disposed (ccb16e4)
- #1203: The Audio client should no longer lock up on disconnect (2c93363)
- #1209: MessageUpdated should no longer pass a null after object (91e0f03)
- Ignore messages with no ID in bulk delete (676be40)
- No longer attempt to load generic types as modules (b1eaa44)
- No longer complain when a `PRESENCES_REPLACE` update is received (beb3d46)
- CommandExecuted will be raised on async exception failures (6260749)
- ExecuteResult now contains the entire exception, not an abridged message (f549da5)
- CommandExecuted will no longer be raised twice for exceptions (aec7105)
- The default WebSocket will now close correctly (ac389f5)

### Changed

- #731: `IUserMessage#GetReactionUsersAsync` now takes an `IEmote` instead of a `string` (5d7f2fc)
- #744: IAsyncEnumerable has been redesigned (5bbd9bb)
- #777: `IGuild#DefaultChannel` will now resolve the first accessible channel, per changes to Discord (1ffcd4b)
- #781: Attempting to add or remove a member's EveryoneRole will throw (506a6c9)
- #801: `EmbedBuilder` will no longer implicitly convert to `Embed`, you must build manually (94f7dd2)
- #804: Command-related tasks will have the 'async' suffix (14fbe40)
- #812: The WebSocket4Net provider has been bumped to version 0.15, allowing support for .NET Standard apps (e25054b)
- #829: DeleteMessagesAsync moved from IMessageChannel to ITextChannel (e00f17f)
- #853: WebSocket will now use `zlib-stream` compression (759db34)
- #874: The `ReadMessages` permission is moving to `ViewChannel` (edfbd05)
- #877: Refactored Games into Activities (34b4e5a)
- #943: Multiple types of quotation marks can now be parsed (thanks 🍎) (cee71ef)
- #955: The `GameParty` model will now use long values (178ea8d)
- #986: Expose the internal entity TypeReaders (660fec0)
- #992: Throw an exception when trying to modify someone else's message (d50fc3b)
- #998: Commands can specify their own `IgnoreExtraArgs` behavior (6d30100)
- #1033: The `ReadMessages` permission bit is now named `ViewChannel` (5f084ad)
- #1042: Content parameter of `SendMessageAsync` is now optional (0ba8b06)
- #1057: An audio channel's `ConnectAsync` now allows users to handle the voice connection elsewhere, such as in Lavalink (890904f)
- #1094: Overhauled invites, added vanity invite support (ffe994a)
- #1108: Reactions now use the undocumented 1/.25 ratelimit, making them 4x faster (6b21b11)
- #1128: Bot tokens will now be validated for common mishaps before use (2de6cef)
- #1140: Check the invite `maxAge` parameter before making the request (649a779)
- #1164: All command results will now be raised in `CommandExecuted` (10f67a8)
- #1171: Clients have been changed to properly make use of `IDisposable` (7366cd4)
- #1172: Invite related methods were moved from `IGuildChannel` to `INestedChannel` (a3f5e0b)
- #1200: HasPrefix extensions now check for null values first (46e2674)
- `IGuildChannel#Nsfw` moved to `ITextChannel`, now maps to the API property (608bc35)
- Preemptive ratelimits are now logged under verbose, rather than warning. (3c1e766)
- The default InviteAge when creating Invites is now 24 hours (9979a02)
- All parameters to `ReplyAsync` have been made optional (b38dca7)
- The socket client will now use additional fields to fill in member/guild information on messages (8fb2c71)
- The Audio Client now uses Voice WS v3 (9ba38d7)

### Removed

- #790: Redundant overloads for `AddField` removed from EmbedBuilder (479361b)
- #925: RPC is no longer being maintained nor packaged (b30af57)
- #958: Remove support for user tokens (2fd4f56)
- User logins (including selfbots) are no longer supported (fc5adca)

### Misc

- #786: Unit tests for the Color structure (22b969c)
- #828: We now include a contributing guide (cd82a0f)
- #876: We now include a standard editorconfig (5c8c784)
- #963: Docs now include a release version, build instructions (88e6244)
- #964: Fix documentation spelling of 'echoes' (fda19b5)
- #967: Unit test permissions (63e6704)
- #968: Bumped version of WebSocket4Net to 0.15.2 (8537924)
- #972: Include sample bots in the source repository (217ec34)
- #1046: We now support .NET Standard 2.0 (bbbac85)
- #1114: Various performance optimizations (82cfdff)
- #1149: The CI will now test on Ubuntu as well as Windows (674a0fc)
- #1161: The entire documentation has been rewritten, all core entities were docstringed (ff0fea9)
- #1175: Documentation changes in command samples (fb8dbca)
- #1177: Added documentation for sharded bots (00097d3)
- #1219: The project now has a logo! 🎉 (5750c3e)
- This project is now licensed to the Discord.Net contributors (710e182)
- Added templates for pull requests (f2ddf51)
- Fixed documentation layout for the logo (bafdce4)

## [1.0.2] - 2017-09-09

### Fixed

- Guilds utilizing Channel Categories will no longer crash bots on the `READY` event.

## [1.0.1] - 2017-07-05

### Fixed

- #732: Fixed parameter preconditions not being loaded from class-based modules (b6dcc9e)
- #726: Fixed CalculateScore throwing an ArgumentException for missing parameters (7597cf5)
- EmbedBuilder URI validation should no longer throw NullReferenceExceptions in certain edge cases (d89804d)
- Fixed module auto-detection for nested modules (d2afb06)

### Changed

- ShardedCommandContext now inherits from SocketCommandContext (8cd99be)
