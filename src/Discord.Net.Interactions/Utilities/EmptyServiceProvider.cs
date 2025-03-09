using Microsoft.Extensions.DependencyInjection;
using System;

namespace Discord.Interactions;

internal class EmptyServiceProvider : IServiceProvider
{
    public static EmptyServiceProvider Instance => new ();

    public object GetService(Type serviceType)
    {
        if (serviceType == typeof(IServiceScopeFactory))
            return EmptyServiceScopeFactory.Instance;

        return null;
    }
}

internal class EmptyServiceScopeFactory : IServiceScopeFactory
{
    public static EmptyServiceScopeFactory Instance => new ();

    public IServiceScope CreateScope() => new EmptyServiceScope();
}

internal class EmptyServiceScope : IServiceScope
{
    public IServiceProvider ServiceProvider => EmptyServiceProvider.Instance;

    public void Dispose() { }
}
