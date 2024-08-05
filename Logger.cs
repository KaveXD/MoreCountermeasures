using UnityEngine;

namespace MoreCountermeasures
{
    public class Logger
    {
        private static readonly string ModName = "MoreCountermeasures";

        public static void Log(string text)
        {
            Debug.Log($"[{ModName}]: {text}");
        }

        public static void LogError(string text)
        {
            Debug.LogError($"[{ModName}]: {text}");
        }
    }
}