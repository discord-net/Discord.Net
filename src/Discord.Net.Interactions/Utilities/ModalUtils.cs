using System;
using System.Collections.Concurrent;

namespace Discord.Interactions
{
    internal static class ModalUtils
    {
        private static readonly ConcurrentDictionary<Type, ModalInfo> _modalInfos = new ConcurrentDictionary<Type, ModalInfo>();

        public static ModalInfo GetOrAdd(Type type, InteractionService interactionService)
        {
            if (!typeof(IModal).IsAssignableFrom(type))
                throw new ArgumentException($"Type must implement {nameof(IModal)}", nameof(type));

            return _modalInfos.GetOrAdd(type, ModuleClassBuilder.BuildModalInfo(type, interactionService));
        }

        public static bool TryGet(Type type, out ModalInfo modalInfo)
        {
            if (!typeof(IModal).IsAssignableFrom(type))
                throw new ArgumentException($"Type must implement {nameof(IModal)}", nameof(type));

            return _modalInfos.TryGetValue(type, out modalInfo);
        }

        public static bool TryRemove(Type type, out ModalInfo modalInfo)
        {
            if (!typeof(IModal).IsAssignableFrom(type))
                throw new ArgumentException($"Type must implement {nameof(IModal)}", nameof(type));

            return _modalInfos.TryRemove(type, out modalInfo);
        }

        public static void Clear() => _modalInfos.Clear();

        public static int Count() => _modalInfos.Count;
    }
}
