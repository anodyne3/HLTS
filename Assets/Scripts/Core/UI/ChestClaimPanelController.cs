﻿using System.Collections.Generic;
using Core.UI.Prefabs;
using Enums;
using MyScriptableObjects;
using TMPro;
using UnityEngine;
using Utils;

namespace Core.UI
{
    public class ChestClaimPanelController : PanelController
    {
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private TMP_Text areYouSureText;
        [SerializeField] private ChestClaimPrefab chestClaimPrefab;
        [SerializeField] private Transform prefabHolder;

        private readonly List<ChestClaimPrefab> _claimButtons = new List<ChestClaimPrefab>();

        public override void Start()
        {
            base.Start();

            InitClaimButtons();

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.chestRefreshEvent, RefreshButtons);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshText, true);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.upgradeRefreshEvent,
                RefreshUpgradeIndicators, true);
        }

        public override void OpenPanel(params object[] args)
        {
            base.OpenPanel(args);

            RefreshButtons();
            RefreshUpgradeIndicators();
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
            areYouSureText.gameObject.SetActive(ChestManager.CurrentChest.chestType != ChestType.Bronze);
            
            messageText.text = Constants.ConfirmClaimMessagePrefix + ChestManager.RollsToBetterChest +
                               Constants.ConfirmClaimMessageSuffix;
            RefreshButtons();
        }

        private void RefreshButtons()
        {
            foreach (var button in _claimButtons)
                button.RefreshButton();
        }

        private void RefreshUpgradeIndicators()
        {
            foreach (var button in _claimButtons)
                button.RefreshUpgradeIndicators();
        }
    }
}