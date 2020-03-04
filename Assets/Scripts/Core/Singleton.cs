using UnityEngine;

namespace Core
{
    public abstract class Singleton<T> : GlobalAccess where T : GlobalAccess
    {
        private static bool _shuttingDown = false;
        private static readonly object Lock = new object();
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_shuttingDown)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                                     "' already destroyed. Returning null.");
                    return null;
                }

                lock (Lock)
                {
                    if (_instance != null) return _instance;
                    
                    _instance = (T) FindObjectOfType(typeof(T));

                    if (_instance != null) return _instance;
                    
                    _instance = new GameObject().AddComponent<T>();
                    _instance.name = typeof(T) + " (Singleton)";

                    // Make instance persistent. why? for all?
                    DontDestroyOnLoad(_instance.gameObject);

                    return _instance;
                }
            }
        }

        private void OnApplicationQuit()
        {
            _shuttingDown = true;
        }

        private void OnDestroy()
        {
            _shuttingDown = true;
        }
    }
}