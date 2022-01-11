#if DEBUG
    await interactionService.RegisterCommandsToGuildAsync(<test_guild_id>);
#else
    await interactionService.RegisterCommandsGloballyAsync();
#endif
