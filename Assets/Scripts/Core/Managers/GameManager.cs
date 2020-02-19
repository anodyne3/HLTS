using Enums;
using UnityEngine;
using Utils;

namespace Core.Managers
{
    public class GameManager : GlobalAccess
    {
        private void Awake()
        {
            Foundation.GameManager = this;
            
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            FirebaseCheckAndFixDependencies.Login();
        }
        
        public void ResetUser()
        {
            AudioManager.PlayClip(SoundEffectType.UiClick);
            DatabaseFunctions.UserReset();
            
            PlayerPrefs.DeleteKey(Constants.ConsentKey);
            SceneManager.LoadScene(0);
        }

        public static void LoadMain()
        {
            SceneManager.LoadScene(1);
        }
    }
}