using System.Threading.Tasks;

namespace Discord
{
    public interface IModel
    {
        ulong Id { get; }

        Task Save();
    }
}
