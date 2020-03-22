using Core.Managers;
using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class MenuPanelController : PanelController
    {
        [SerializeField] private TMP_Text menuTitleText;
        [SerializeField] private Button resetAccountButton;
        [SerializeField] private Button linkAccountButton;
        [SerializeField] private Button unlinkAccountButton;

        public TMP_Text userId;

        public override void Start()
        {
            base.Start();

            resetAccountButton.onClick.RemoveAllListeners();
            resetAccountButton.onClick.AddListener(ResetAccount);
            linkAccountButton.onClick.RemoveAllListeners();
            linkAccountButton.onClick.AddListener(LinkAccount);
            unlinkAccountButton.onClick.RemoveAllListeners();
            unlinkAccountButton.onClick.AddListener(UnlinkAccount);
        }

        public override void OpenPanel()
        {
            base.OpenPanel();
            
            StartTextAnimations();

            if (PlayerData.firebaseUser == null) return;

            userId.text = PlayerData.firebaseUser.UserId;
        }

        private static void ResetAccount()
        {
            GameManager.ResetAccount();
        }

        private static void LinkAccount()
        {
            AudioManager.PlayClip(SoundEffectType.UiClick);
            FirebaseFunctionality.LinkAccount();
        }

        private static void UnlinkAccount()
        {
            AudioManager.PlayClip(SoundEffectType.UiClick);
            FirebaseFunctionality.UnlinkAccount();
        }
    }
}