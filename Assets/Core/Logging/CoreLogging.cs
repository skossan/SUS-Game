using UnityEngine;

namespace Core.Logging
{
    /// <summary>
    /// Core Logging specifically made for this Core system
    /// </summary>
    public static class CoreLogging
    {
        public static void LogMissingComponent<T>(GameObject parent)
        {
            LogError($"Missing component {typeof(T)}. Please add a {typeof(T)} component to the {parent.name}");
        }

        public static void LogMissingGameObject<T>(GameObject parent)
        {
            LogError($"Missing GameObject {typeof(T)}. Please add a {typeof(T)} GameObject to the {parent.name}");
        }

        private static void LogError(string message)
        {
            Debug.LogError($"CORE-WARNING: {message}");
        }
    }
}
