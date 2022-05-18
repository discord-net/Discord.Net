using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    internal interface ICached<TType> : ICached, IDisposable
    {
        void Update(TType model);

        TType ToModel();
    }

    public interface ICached
    {
        bool IsFreed { get; }
    }
}
