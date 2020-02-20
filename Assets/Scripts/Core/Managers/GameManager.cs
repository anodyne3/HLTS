using Enums;
using UnityEngine;
using Utils;

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

        private void Start()
        {
            LoadMain();
        }

        public static void LoadMain()
        {
            SceneManager.LoadScene(1);
        }

        public void ResetAccount()
        {
            AudioManager.PlayClip(SoundEffectType.UiClick);
            DatabaseFunctions.ResetAccount();

            PlayerPrefs.DeleteKey(Constants.ConsentKey);
            SceneManager.LoadScene(0);
        }

        public void LinkAccount()
        {
            AudioManager.PlayClip(SoundEffectType.UiClick);
            DatabaseFunctions.LinkAccount();
        }

        public void GrabCoin()
        {
        }
        
        public void SpendCoin()
        {
        }

        public void PullArm()
        {
            SlotMachine.OnArmPull();
        }
    }
}