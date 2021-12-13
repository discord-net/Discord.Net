# Changelog

## [3.4.9] - 12/06/2021

### Fixed

- NRE when responding to an interaction with just an embed

## [3.4.8] - 12/04/2021

### Added

- Check for creating threads in news channels.
- Back netstandard2.0 and netstandard2.1 builds.
- IApplicationCommandInteraction interface and make IUserCommandInteraction, IMessageCommandInteraction, and ISlashCommandInteraction inherit it.
- FollowupWithFilesAsync to interactions to allow uploading multiple attachments.
- IUser User to IDiscordInteraction.

### Fixed

- NRE on modifying messages allowed mentions again.

### Misc

- Deprecate ApplicationCommandException in favor for HttpException.
- Made RespondAsync return a RestInteractionMessage based off of the parameters sent over to discord. This allows direct calls to ModifyAsync and DeleteAsync without having to get the interaction response first.
- Change Followup / Respond method signatures to move RequestOptions to the last parameter as well as change component to components.
- Update async summaries to contain information about the task in the return tag.

## [3.4.7] - 28/11/2021

### Added

- Methods for manually registering global comands.

### Fixed

- NRE on service providerless command execution.
- Wrong link in interactions framework docs.

## [3.4.6] - 27/11/2021

### Fixed

- Self user presence throwing upon set.

## [3.4.5] - 27/11/2021

### Fixed

- Another NRE with presence.

## [3.4.4] - 27/11/2021

### Fixed

- NRE with presence causing guild members to basically not exist.

## [3.4.3] - 27/11/2021

### Added

- PresenceUpdate socket event to help alleviate GuildMemberUpdate event calls.
- Guild.ClearUserCache(Predicate) to socket guilds.
- Warnings to the gateway client about intents, you can disable them by setting LogGatewayIntentWarnings to false in the discord socket config.
- DefaultApplications enum with the default games in the discord games lab.
- Support for sending multiple files on Webhooks.
- MaxBitrate to the IGuild interface.

### Fixed

- Bi-directional unicode names getting formatted wrong when calling IUser.ToString().
- CurrentUser with ICommandContext on DiscordShardedClient being null.
- NRE on Emote.TryParse.

## [3.4.2] - 26/11/2021

### Fixed

- Improve the GuildFeatures converter.
- Message/User commands are not being executed when their name have spaces on it.

## [3.4.1] - 26/11/2021

### Fixed

- SocketVoiceChannel options are created as generic mentionables in Interaction service

## [3.4.0] - 26/11/2021

### Added

- Interaction Service framework.

### Fixed

- NRE when modifying allowed mentions.

### Misc

- Updated summaries of interaction related classes.

## [3.3.3] - 11/23/2021

### Added

- Emoji to roles
- Invitable and Slowmode to thread creation
- Better discord errors.
- UseInteractionSnowflakeDate to config.

### Fixed

- Components not showing on FollowUpWithFile.
- Change the minimum length of slash commands to 1.
- Ratelimit timings.

### Misc

- Make RestUserCommand public.

## [3.3.2] - 11/21/2021

### Added

- GuildScheduledEventUserAdd and GuildScheduledEventUserRemove.
- New Cacheable<> type for rest based downloads while maintaining strong typed entities.

### Fixed

- Autocomplete not accepting values

## [3.3.1] - 11/21/2021

### Added

- ConfigureAwait(false) to async calls in rest based interactions.

### Fixed

- Slash commands will now have populated data.

## [3.3.0] - 11/19/2021

### Added

- Guild events.
- Rest-based http interactions.
- RTCRegion to voice channel properties. 
- Support Min and Max values on ApplicationCommandOptions.
- Automatically fix ordering of optional command options.
- Interaction Specific Interfaces.

### Fixed

- Maximum number of Select Menu Options.

### Misc

- Changed default sticker behavior. You can configure getting default stickers on startup and auto resolve stickers with AlwaysDownloadDefaultStickers and AlwaysResolveStickers.
- Logomark, doc settings edit.

## [3.2.0] - 11/09/2021

### Added

- Added `GetThreadMemberAsync` implementation.
- Added `GuildFeatures` to `IGuild`.
- Added checks for gateway intent in some methods. `DownloadUsersAsync` will throw an exception if you don't have the gateway intent enabled locally now, this should help new people specify the correct intents locally.
- Added strong types to `GuildJoinRequestDeleted` events.
- Added support for GUILD_JOIN_REQUEST_DELETE event.
- Added BotHTTPInteraction user flag.
- Added support for animated guild banners.
- Added student hub guild directory channel.
- Added support Guild Boost Progress Bars
- Added missing values to SystemChannelMessageDeny.
- Added state checking to ConnectionManager.StartAsync
- Added `SendFilesAsync` to channels.
- Added `Attachments` property to `MessageProperties`

### Fixed

- Fixed numeric type check for slash command options.
- Fixed followup with file overwrite having incorrect parameter locations.
- Fixed non value type options not being included in autocomplete

### Misc

- Renamed `PARTY_PRIVACY_FRIENDS`, `PARTY_PRIVACY_VOICE_CHANNEL`, and `EMBEDDED` to `PartyPrivacyFriends`, `PartyPrivacyVoiceChannel`, and `Embedded` in `ActivityProperties`.
- Removed error log for gateway reconnects.
- Removed BannerId and AccentColor from socket users.

## [3.1.7] - 10/20/2021

### Added

- Added `Icon` property when modifying roles.
- Added `GuildAvatar` and `GetGuildAvatarUrl` to `IGuildUser`.

### Fixed

- Fixed `InvalidOperationException` in modify guild channel.

### Misc

- Revert received at time, confirmed by discord staff to be accurate

## [3.1.4 - 3.1.6] - 10/12/2021

### Added

- Added check for duplicate responses to interactions.
- Added check for 3 second timeout to interactions.

### Fixes

- Fixed assignment of `UserMentions`
- Fixed `CleanContent` not being correct
- Fixed SocketSlashCommandData access modifier
- Fixed url validation for embed urls

## [3.1.2 + 3.1.3] - 10/12/2021

### Added

- Added .NET 5 build.
- Added `Ephemeral` property to `Attachment`.
- Added `ChannelTypes` to `ApplicationCommandOption` and builders.
- Added event for `SocketAutocompleteInteraction` called `AutocompleteExecuted`.
- Added `CleanContent` to `IMessage` and implemented entities.
- Added support for `discord://` protocol on buttons.
- Added `Competing` status type.
- Added `Icon` and `GetIconUrl` to roles.
- Added `Interaction` to `IMessage` and implemented entities.
- Added `ContextMenuCommand` message type.
- Added stage instance audit logs as well as thread audit log type.
- Added restriction for description not being null in `SlashCommandBuilder`.

### Fixes

- Fixed channel name ratelimits being ignored + any other property specific ratelimits.
- Fix different rest channels not deserializing properly.
- Fix NRE in modify guild channel.
- Fixed system messages not including mentioned users.
- Fixed sub commands being interpreted as a parameter for autocomplete.
- Fixed `Type` not being set in `SocketApplicationCommand`.

### Misc

- Simplified `FollowupWithFileAsync` to just take a file path.
- Renamed `Default` and Required to `IsDefault` and `IsRequired` in `IApplicationCommandOption`.
- Renamed `DefaultPermission` to `IsDefaultPermission` in `IApplicationCommand`.
- Refactored some summaries
- Renamed `Available` to `IsAvailable` in stickers.
- Removed file extension check.
- Remove null collections in favor for empty collections.

## [3.1.1] - 9/23/2021

### Added

- Added `SocketAutocompleteInteraction`
- Added `IsAutocomplete` to slash command builders

## [3.1.0] - 9/21/2021

### Added

- Added warning for duplicate select menu values.
- Added `DisconnectAsync` and `MoveAsync` to `IGuild` and its children (`RestGuild`/`SocketGuild`).
- Added `CreateStageChannelAsync` to `IGuild` and its children (`RestGuild`/`SocketGuild`).
- Added `ISticker[]` as a parameter to `SendMessageAsync` and `ReplyAsync`.

### Fixes

- Fixed ratelimit sorcery and witchcraft.

### Misc

- Renamed `SelectMenu` to `SelectMenuComponent`.
- Removed obsolete `AcknowledgeAsync` method from interactions in favor for `DeferAsync`.

## [3.0.2] - 9/13/2021

### Added

- Added `Hierarchy` to `RestGuildUser` and `IGuildUser`
- Added github sponsors!
- Added implicit conversion from string to emoji and emote, allowing you to just pass in unicode into any emoji/emote func
- Added `ToBuilder` functions to `ButtonComponent` and `SelectMenu`
- Added `FollowupWithFileAsync` to interactions.

### Fixes

- Fixed `auto_archive_duration` within thread models
- Fixed `ModifyGuildCommandPermissionsAsync` not allowing 0 args
- Fixed `MofifyXYZCommand` using abstracts in activators (big no no)
- Fixed specifying null for components within a modify context not removing the components within the message.

## [3.0.1] - 8/25/2021

### Added

- Added a method in `ComponentBuilder` to create a builder from a message

### Fixes

- Fixed default stickers not being set correctly
- Fixed NRE with application command routes
- Fixed KeyNotFoundException on Message Create
- Fixed RestUserMessage not being used for some message types
- Fixed NRE on default stickers
- Fixed Stickers being empty on messages
- Fixed `SocketUnknownSticker.ResolveAsync` throwing NRE

## [3.0.0] - 8/5/2021

### Added

- Added stage support.
- Added multi embed support for normal messages and refactored methods for interaction with embeds.
- Added check for proper urls in embeds and buttons.
- Added thread support.
- Added `NsfwLevel` property to guilds.
- Added missing message types: GuildDiscoveryDisqualified, GuildDiscoveryRequalified, GuildDiscoveryGracePeriodInitialWarning, GuildDiscoveryGracePeriodFinalWarning, ThreadCreated, ThreadStarterMessage, and GuildInviteReminder.
- Added `NUMBER` application command option type.
- Added missing audit log types: IntegrationCreated,IntegrationUpdated, IntegrationDeleted, StageInstanceCreated, StageInstanceUpdated, StageInstanceDeleted, StickerCreated, StickerUpdated, and StickerUpdated.

### Fixes

- Fixed respond async fallback not taking in components.
- Fixed Emoji UnicodeAndNames throwing exception because of duplicit keys.
- Fixed `PermissionTarget` and `ApplicationCommandPermissionTarget` confusion and Invalid Form Body for modifying channel overwrites.

### Misc

- Made custom id an optional parameter in buttons.
- Refactored the component builders to be more flexible.
- Changed `AcknowledgeAsync` to `DeferAsync` to avoid confusion.
- Updated `MaxOptionsCount` and `MaxChoiceCount` to 25.

## [2.4.9] - 7/17/20201

### Fixes

- The followup method that takes a single embed didnt return the RestFollupMessage

## [2.4.8] - 7/17/2021

### Added

- Added `ApplicationCommandPermissionTarget` enum for slash command permissions, slash commands dont use `PermissionTarget` anymore.

### Fixed

- Fixed `AddPermissionOverride` not working
- Fixed invalid form body for responding with no embed

## [2.4.6] - 7/14/2021

### Added

- Added the ability to send multiple embeds on interaction responses and also fixed a few bits of docs.

## [2.4.5] - 7/10/2021

### Added

- Added `TimestampTag`
- Added `DeleteAllGlobalCommandsAsync` to Rest client.
- Added `DeleteSlashCommandsAsync` to SocketGuild and RestGuild, this **will** remove all the slash commands in the guild so be careful.

### Fixes

- Fixed `GetOriginalResponseAsync` using wrong http method and route.

## [2.4.4] - 7/7/2021

- Added comment and parsing for `Mentionable` application option type.
- Added another `AddOption` method to SlashCommandOptionBuilder.

### Added

### Fixes

- Fixed `GetCommandPermission()` throwing if no permissions are found, it will now return null.
- Fixed SlashCommandBuilder incorrectly limiting SubCommands.
- Fixed incorrect casing of .net types in SocketSlashCommandData.
- Fixed ambiguous method in `SlashCommandBuilder` .
- Fixed `WithAuthor` extension not using default avatars: https://github.com/discord-net/Discord.Net/pull/1890
- Fixed Slash command routes not throwing a `ApplicationCommandException`
- Fixed duplicate `GetOriginalResponse` method in `SocketSlashCommand`

## [2.4.3] - 7/6/20201

### Added

- Added `GetSlashCommandsAsync` and `GetSlashCommandAsync` methods to `SocketGuild` and `RestGuild`
- Added `GetGuild` method to `RestGuildCommand`
- Added `GetCommandPermission` and `ModifyCommandPermissions` to `RestGuildCommand`
- Added `BulkOverwriteGlobalCommands` and `BulkOverwriteGuildCommands` to the rest client, this allows you to make a whole bunch of slash commands in one request
- Added `DefaultPermission` to IApplicationCommands.

### Fixed

- Fixed bad parsing on SocketSlashCommand for channel resolved types.

## [2.4.1] - 7/3/20201

### Added

- Added `SelectMenu` as a component type.
- Added methods to build select menus in the `ComponentBuilder`.
- Added a `Values` property to `SocketMessageComponentData`, this is how you see the selected option on a select menus interaction event.
- Added resolved models to `SocketSlashCommandDataOption`, the `Value` field will now use the _strong_ type of the option type, ex a guild user option will now have the value of a `SocketGuildUser`
- Changed the embed description length to 4096 (https://discord.com/developers/docs/resources/channel#embed-limits)
- Streamlined the interaction data parsing, no more weird null exceptions in the `INTERACTION_CREATED` event
- Changed `SocketInteraction.Data` to the `IDiscordInteractionData` type.
- Merged DNET into interactions, we are now on api v9. This also includes all the bug fixes that dnet did.
- Added the new channel permission flags (https://discord.com/developers/docs/topics/permissions#permissions-bitwise-permission-flags)

### Fixes

- Fixed the component builders checks for adding new components.

## [2.3.7 and older] - 5/29/2021

### Added

- Added message component support
- Added Interaction support
- Added slash command support
