using Enums;
using UnityEngine;
using Utils;

namespace Core.Managers
{
    public class GameManager : GlobalAccess
    {
        public Camera mainCamera;

        private void Awake()
        {
            Foundation.GameManager = this;
            
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            FirebaseCheckAndFixDependencies.Login();
        }

        private void ResetUser()
        {
            AudioManager.PlayClip(SoundEffectType.UiClick);
            
            PlayerPrefs.DeleteKey(Constants.ConsentKey);
            SceneManager.LoadScene(0);
        }
    }
}