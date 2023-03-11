using System;

namespace Discord.Interactions
{
    internal class EmptyServiceProvider : IServiceProvider
    {
        public static EmptyServiceProvider Instance => new EmptyServiceProvider();

        public object GetService(Type serviceType) => null;
    }
}
