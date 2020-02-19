using UnityEngine;

namespace Core
{
    public abstract class Singleton<T> : GlobalAccess where T : GlobalAccess
    {
        private static bool _shuttingDown = false;
        private static object _lock = new object();
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

                lock (_lock)
                {
                    if (_instance != null) return _instance;
                    
                    _instance = (T) FindObjectOfType(typeof(T));

                    if (_instance != null) return _instance;
                    
                    _instance = new GameObject().AddComponent<T>();
                    _instance.name = typeof(T) + " (Singleton)";
                    // var singletonObject = new GameObject();
                    // _instance = singletonObject.AddComponent<T>();
                    // singletonObject.name = typeof(T) + " (Singleton)";

                    // Make instance persistent. why
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

        //lazy singleton
        /*private static readonly Lazy<Singleton> Lazy =
            new Lazy<Singleton>(() => new GameObject("Singleton").AddComponent<Singleton>());

        protected static Singleton Instance => Lazy.Value;

        protected Singleton()
        {
        }*/
    }
}