using UnityEngine.SceneManagement;

namespace Core.Managers
{
    public class SceneManager : Singleton<SceneManager>
    {
        public void LoadScene(int sceneId)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneId, LoadSceneMode.Single);
        }

        public void LoadAdditiveScene(int sceneId)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneId, LoadSceneMode.Additive);
        }

        public void LoadSceneAsynchronously(int newSceneId)
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(newSceneId, LoadSceneMode.Single);
        }
    }
}