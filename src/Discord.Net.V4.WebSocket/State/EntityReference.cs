using Discord.Net.V4.WebSocket.State.Handles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.State
{
    internal class EntityReference
    {
        /// <summary>
        ///     Gets the total number of active handles referencing the entity.
        /// </summary>
        /// <remarks>
        ///     This property does not reflect the actual references to the entity iself.
        /// </remarks>
        public int HandleReferenceCount
            => Handles.Count;

        public WeakReference Reference { get; }

        public HashSet<Guid> Handles { get; }

        public EntityReference(object entity)
        {
            Reference = new WeakReference(entity);
            Handles = new();
        }

        public void RegisterHandle(IEntityHandle handle)
        {
            Handles.Add(handle.HandleId);
            handle.AddNotifier(h => UnregisterHandle(h.HandleId));
        }

        public void UnregisterHandle(Guid id)
            => Handles.Remove(id);
    }
}
