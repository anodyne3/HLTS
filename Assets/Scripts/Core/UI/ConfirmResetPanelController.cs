﻿using Enums;
using MyScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class ConfirmResetPanelController : PanelController
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TweenPunchSetting punchSetting;
    
        public override void Start()
        {
            base.Start();
        
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(ConfirmReset);
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(ClosePanel);
        }

        private void ConfirmReset()
        {
            AudioManager.PlayClip(SoundEffectType.UiClick);
            
            punchSetting.DoPunch(confirmButton.transform, false);
            FirebaseFunctionality.ResetAccount();
        }

        protected override void ClosePanel()
        {
            base.ClosePanel();
            
            punchSetting.DoPunch(cancelButton.transform, false);
        }
    }
}