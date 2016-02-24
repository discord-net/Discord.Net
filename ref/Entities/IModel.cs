using System.Threading.Tasks;

namespace Discord
{
    public interface IModel<TId> : IModel
    {
        TId Id { get; }
    }
    public interface IModel
    {
        Task Save();
    }
}
