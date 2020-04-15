# Changelog

## [2.2.0] - 2020-04-14
TODO: write changelog

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
