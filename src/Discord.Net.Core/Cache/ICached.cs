using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    internal interface ICached<TType>
    {
        void Update(TType model);

        TType ToModel();

        TResult ToModel<TResult>() where TResult : TType, new();
    }
}
