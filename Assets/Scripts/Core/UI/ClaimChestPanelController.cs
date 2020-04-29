using Core.Managers;
using MyScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.UI
{
    public class ClaimChestPanelController : PanelController
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TweenPunchSetting punchSetting;
        [SerializeField] private TMP_Text messageText;
    
        public override void Start()
        {
            base.Start();
        
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(ConfirmAction);
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(ClosePanel);

            RefreshPanel();
        }

        private void RefreshPanel()
        {
            messageText.text = Constants.ConfirmClaimMessagePrefix + ChestManager.RollsToBetterChest +
                               Constants.ConfirmClaimMessageSuffix;
        }

        private void ConfirmAction()
        {
            punchSetting.DoPunch(confirmButton.transform, false);
            
            FirebaseFunctionality.ClaimChest();
            
            base.ClosePanel();
        }

        protected override void ClosePanel()
        {
            base.ClosePanel();
            
            punchSetting.DoPunch(cancelButton.transform, false);
        }
    }
}