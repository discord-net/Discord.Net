using Discord.Models;

namespace Discord.Rest.Extensions;

public static class ModelExtensions
{
    public readonly struct KeyedModelCollection : IDisposable
    {
        private readonly Dictionary<Type, HashSet<IEntityModel>> _entityBuckets;

        public KeyedModelCollection(IModelSource model)
        {
            _entityBuckets = new();

            foreach (var otherModel in model.GetDefinedModels())
            {
                if (!_entityBuckets.TryGetValue(otherModel.GetType(), out var bucket))
                    _entityBuckets[otherModel.GetType()] = bucket = new();
            }
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }
    }

    public static KeyedModelCollection GetKeyedModelCollection(this IModelSource model)
    {

    }
}
