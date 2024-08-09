namespace Discord.Gateway;

public delegate ValueTask PreparedInvocableEventHandle(CancellationToken token);
public delegate ValueTask InvocableEventHandler<in TPackage>(TPackage package, CancellationToken token);
