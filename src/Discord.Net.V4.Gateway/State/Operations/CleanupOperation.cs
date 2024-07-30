namespace Discord.Gateway.State.Operations;

public sealed record CleanupOperation(Func<CancellationToken, Task> CleanupTask) : IStateOperation;
