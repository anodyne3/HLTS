using Core.Managers;
using Enums;
using MyScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class ConfirmRewardAdPanelController : PanelController
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TweenPunchSetting punchSetting;

        private AdType _adType;
    
        public override void Start()
        {
            base.Start();
        
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(ConfirmRewardAd);
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(ClosePanel);
        }

        public override void OpenPanel(params object[] args)
        {
            base.OpenPanel(args);

            _adType = (AdType) args[0];
        }

        private void ConfirmRewardAd()
        {
            punchSetting.DoPunch(confirmButton.transform, false);
            AdManager.ShowRewardedAd(_adType);
            
            base.ClosePanel();
        }

        protected override void ClosePanel()
        {
            base.ClosePanel();
            
            punchSetting.DoPunch(cancelButton.transform, false);
        }
    }
}