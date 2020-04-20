using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class AccountPanelController : PanelController
    {
        [SerializeField] private Button resetAccountButton;
        [SerializeField] private Button linkAccountButton;
        [SerializeField] private Button unlinkAccountButton;
        [SerializeField] private Button policiesButton;

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
            policiesButton.onClick.RemoveAllListeners();
            policiesButton.onClick.AddListener(OpenPoliciesPanel);
        }

        public override void OpenPanel()
        {
            base.OpenPanel();

            if (PlayerData.firebaseUser == null) return;

            userId.text = PlayerData.firebaseUser.UserId;
        }

        private static void ResetAccount()
        {
            PanelManager.OpenSubPanel<ConfirmResetPanelController>();
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
        
        private static void OpenPoliciesPanel()
        {
            PanelManager.OpenSubPanel<PoliciesPanelController>();
        }
    }
}