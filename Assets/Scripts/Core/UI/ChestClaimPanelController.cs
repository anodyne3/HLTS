using System.Collections.Generic;
using Core.UI.Prefabs;
using MyScriptableObjects;
using TMPro;
using UnityEngine;
using Utils;

namespace Core.UI
{
    public class ChestClaimPanelController : PanelController
    {
        public TweenPunchSetting punchSetting;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private ChestClaimPrefab chestClaimPrefab;
        [SerializeField] private Transform prefabHolder;

        private readonly List<ChestClaimPrefab> _claimButtons = new List<ChestClaimPrefab>();
        
        public override void Start()
        {
            base.Start();
        
            InitClaimButtons();
            
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.chestRefreshEvent, RefreshButtons);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshText, true);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.upgradeRefreshEvent, RefreshButtons, true);
        }

        private void InitClaimButtons()
        {
            var chestTypesLength = ChestManager.chestTypes.Length - 1;
            for (var i = 0; i < chestTypesLength; i++)
            {
                var chestClaimButton = Instantiate(chestClaimPrefab, prefabHolder);  
                chestClaimButton.Init(ChestManager.chestTypes[i]);
                _claimButtons.Add(chestClaimButton);
            }
        }

        private void RefreshText()
        {
            messageText.text = Constants.ConfirmClaimMessagePrefix + ChestManager.RollsToBetterChest +
                               Constants.ConfirmClaimMessageSuffix;
        }
        
        private void RefreshButtons()
        {
            if (!isActiveAndEnabled) return;
            
            foreach (var button in _claimButtons)
                button.RefreshButton();
        }

        public void ClosePanelRemote()
        {
            ClosePanel();
        }
    }
}