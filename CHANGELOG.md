# Changelog

## [Unreleased]
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
- 'html' variant added to the `EmbedType` enum (42c879c)

### Fixed
- #742: `DiscordShardedClient#GetGuildFor` will now direct null guilds to Shard 0 (d5e9d6f)
- #743: Various issues with permissions and inheritance of permissions (f996338)
- #755: `IRole.Mention` will correctly tag the @everyone role (6b5a6e7)
- #768: `CreateGuildAsync` will include the icon stream (865080a)
- #866: Revised permissions constants and behavior (dec7cb2)
- #872: Bulk message deletion should no longer fail for incomplete batch sizes (804d918)
- #923: A null value should properly reset a user's nickname (227f61a)
- #938: The reconnect handler should no longer deadlock during Discord outages (73ac9d7)
- Ignore messages with no ID in bulk delete (676be40)

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
- `IGuildChannel#Nsfw` moved to `ITextChannel`, now maps to the API property (608bc35)
- Preemptive ratelimits are now logged under verbose, rather than warning. (3c1e766)
- The default InviteAge when creating Invites is now 24 hours (9979a02)


### Removed
- #790: Redundant overloads for `AddField` removed from EmbedBuilder (479361b)
- #925: RPC is no longer being maintained nor packaged (b30af57)
- User logins (including selfbots) are no longer supported (fc5adca)

### Misc
- This project is now licensed to the Discord.Net contributors (710e182)
- #786: Unit tests for the Color structure (22b969c)
- #828: We now include a contributing guide (cd82a0f)
- #876: We now include a standard editorconfig (5c8c784)

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
