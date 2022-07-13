using System;
using UnityEngine;

namespace Core.Logging
{
    /// <summary>
    /// Core Logging specifically made for the Core system
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

        public static void LogError(string message)
        {
            Debug.LogError($"CORE-ERROR: {message}");
        }

        public static void LogWarning(string message)
        {
            Debug.LogWarning($"CORE-WARNING: {message}");
        }

        public static void Log(string message)
        {
            Debug.Log($"CORE-LOG: {message}");
        }
    }
}
