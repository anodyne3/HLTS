namespace Core.Managers
{
    public class GameManager : GlobalAccess
    {
        public bool interactionEnabled;

        private void Awake()
        {
            Foundation.GameManager = this;

            DontDestroyOnLoad(gameObject);
        }

        public static void LoadMain()
        {
            SceneManager.LoadSceneAsynchronously(1);
        }
    }
}