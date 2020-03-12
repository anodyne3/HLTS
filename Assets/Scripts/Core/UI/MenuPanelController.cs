using Core.Managers;
using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class MenuPanelController : PanelController
    {
        [SerializeField] private Button resetAccountButton;
        [SerializeField] private Button linkAccountButton;
        [SerializeField] private Button unlinkAccountButton;
        public TMP_Text userId;

        private void Start()
        {
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

            if (PlayerData.firebaseUser == null) return;
            
            userId.text = PlayerData.firebaseUser.UserId;
        }

        private void ResetAccount()
        {
            GameManager.ResetAccount();
        }

        private void LinkAccount()
        {
            AudioManager.PlayClip(SoundEffectType.UiClick);
            FirebaseFunctionality.LinkAccount();
        }
        
        private void UnlinkAccount()
        {
            AudioManager.PlayClip(SoundEffectType.UiClick);
            FirebaseFunctionality.UnlinkAccount();
        }
    }
}