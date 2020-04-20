using IngameDebugConsole;
using UnityEngine;

namespace Core.Managers
{
    public class GameManager : GlobalAccess
    {
        public bool interactionEnabled;
        public bool debug;
        [SerializeField] private DebugLogManager debugLogManager;

        private void Awake()
        {
            Foundation.GameManager = this;

            DontDestroyOnLoad(gameObject);
            
            if (debugLogManager == null) return;
            
            if (!debug)
                Destroy(debugLogManager.gameObject);
        }

        public static void LoadMain()
        {
            SceneManager.LoadSceneAsynchronously(1);
        }
    }
}