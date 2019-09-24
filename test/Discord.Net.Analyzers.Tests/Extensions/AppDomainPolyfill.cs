using System.Linq;
using System.Reflection;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyModel;

namespace System
{
    /// <summary> Polyfill of the AppDomain class from full framework. </summary>
    internal class AppDomain
    {
        public static AppDomain CurrentDomain { get; private set; }

        private AppDomain()
        {
        }

        static AppDomain()
        {
            CurrentDomain = new AppDomain();
        }

        public Assembly[] GetAssemblies()
        {
            var rid = RuntimeEnvironment.GetRuntimeIdentifier();
            var ass = DependencyContext.Default.GetRuntimeAssemblyNames(rid);

            return ass.Select(Assembly.Load).ToArray();
        }
    }
}
