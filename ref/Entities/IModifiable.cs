using System;
using System.Threading.Tasks;

namespace Discord
{
    public interface IModifiable<TModel>
    {
        /// <summary> Modifies one or more of the properties of this object. </summary>
        Task Modify(Action<TModel> func);
    }
}
