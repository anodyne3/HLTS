using System;
using System.Collections;
using System.Collections.Generic;
using Core.UI;
using Enums;
using Utils;
using MyScriptableObjects;
using UnityEngine;

namespace Core.Managers
{
    public class NarrativeManager : Singleton<NarrativeManager>
    {
        public NarrativePoint currentNarrativePoint;

        private readonly WaitUntil _gameManagerInteractionWait = new WaitUntil(() => GameManager.interactionEnabled);
        private readonly WaitUntil _payoutEventWait = new WaitUntil(() => !SlotMachine.wheelsAreRolling);
        private NarrativePanelController _narrativePanel;
        
        private Queue<NarrativePoint> _narrativeFireAndForget = new Queue<NarrativePoint>();

        private void Start()
        {
            LoadNarrativeQueue();
            
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinCreatedEvent,
                OpenIntroPanel);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.armPullEvent, ArmPullTests);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshUiTests);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.upgradeRefreshEvent,
                UpgradeRefreshTests);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.narrativeRefreshEvent,
                OpenNarrativePanel);
        }

        private void LoadNarrativeQueue()
        {
            var narrativePoints =
                GeneralUtils.SortLoadedList<NarrativePoint>(Constants.NarrativeData,
                    (x, y) => x.id.CompareTo(y.id));

            var narrativePointsLength = narrativePoints.Length;
            for (var i = 0; i < narrativePointsLength; i++)
            {
                if (narrativePoints[i].fireAndForget)
                    _narrativeFireAndForget.Enqueue(narrativePoints[i]);
            }
        }

        private void OpenIntroPanel()
        {
            RefreshHelpButton();

            if (_narrativePanel != null && _narrativePanel.gameObject.activeInHierarchy) return;

            if ((NarrativeTypes) PlayerData.narrativeProgress != NarrativeTypes.Intro) return;

            _narrativePanel = PanelManager.GetPanel<NarrativePanelController>();
            _narrativePanel.OpenPanel("blackoutBackground");
            CurrencyManager.HideCurrencies(true);
        }

        private static void ArmPullTests()
        {
            if ((NarrativeTypes) PlayerData.narrativeProgress != NarrativeTypes.PullLever) return;

            FirebaseFunctionality.ProgressNarrativePoint();
        }

        private void RefreshUiTests()
        {
            switch ((NarrativeTypes) PlayerData.narrativeProgress)
            {
                case NarrativeTypes.ChestRoll:
                    if (PlayerData.GetChestCount(ChestType.Bronze) > 0)
                        FirebaseFunctionality.ProgressNarrativePoint();
                    else if (ChestManager.CurrentChest != null &&
                             ChestManager.GetFillAmount(ChestManager.CurrentChest.rank) < 0.11f)
                        StartCoroutine(DelayedOpen(7.0f,
                            () => PanelManager.OpenPanelOnHold<NarrativePanelController>(_payoutEventWait)));
                    break;
                case NarrativeTypes.ChestGained:
                    if (CurrencyManager.GetCurrencyAmount(ResourceType.BluePrints) > 0)
                        FirebaseFunctionality.ProgressNarrativePoint();
                    break;
                case NarrativeTypes.Blueprints:
                    if (CurrencyManager.GetCurrencyAmount(ResourceType.BluePrints) > UpgradeManager
                            .GetUpgradeVariable(UpgradeTypes.CoinSlot).CurrentResourceRequirements[1].resourceAmount)
                        FirebaseFunctionality.ProgressNarrativePoint();
                    break;
                case NarrativeTypes.UpgradeMerge:
                    if (CurrencyManager.GetCurrencyAmount(ResourceType.BluePrints) >= Constants.ChestMergeTrigger)
                        PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait);
                    break;
                case NarrativeTypes.UpgradeClaim:
                    if (CurrencyManager.GetCurrencyAmount(ResourceType.BluePrints) >= Constants.ChestClaimTrigger)
                        PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait);
                    break;
                default:
                    return;
            }
        }

        private static IEnumerator DelayedOpen(float waitTime, Action callback)
        {
            yield return new WaitForSeconds(waitTime);

            callback.Invoke();
        }

        private static void UpgradeRefreshTests()
        {
            if ((NarrativeTypes) PlayerData.narrativeProgress != NarrativeTypes.CoinSlotUpgrade) return;

            if (UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.CoinSlot) > 0)
                FirebaseFunctionality.ProgressNarrativePoint();
        }

        private void OpenNarrativePanel()
        {
            if (PanelManager == null) return;

            switch ((NarrativeTypes) PlayerData.narrativeProgress)
            {
                case NarrativeTypes.PullLever:
                    StartCoroutine(DelayedOpen(2.0f,
                        () => PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait)));
                    break;
                case NarrativeTypes.ChestGained:
                    if (PlayerData.GetChestCount(ChestType.Bronze) > 0)
                        PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait);
                    break;
                case NarrativeTypes.Blueprints:
                    if (CurrencyManager.GetCurrencyAmount(ResourceType.BluePrints) > 0)
                        PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait);
                    break;
                case NarrativeTypes.CoinSlotUpgrade:
                    PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait);
                    break;
                case NarrativeTypes.UpgradeSlider:
                    StartCoroutine(DelayedOpen(2.0f,
                        () => PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait)));
                    break;
            }
        }

        public void RefreshHelpButton()
        {
            if (HudManager == null) return;

            switch (currentNarrativePoint.id)
            {
                case NarrativeTypes.PullLever:
                case NarrativeTypes.ChestRoll:
                case NarrativeTypes.ChestGained:
                case NarrativeTypes.Blueprints:
                case NarrativeTypes.CoinSlotUpgrade:
                    HudManager.helpButton.gameObject.SetActive(true);
                    break;
                default:
                    HudManager.helpButton.gameObject.SetActive(false);
                    break;
            }
        }

        public void RefreshCurrentNarrativePoint()
        {
            currentNarrativePoint = (NarrativePoint) Resources.Load(
                Constants.NarrativePointsPath + (NarrativeTypes) PlayerData.narrativeProgress,
                typeof(NarrativePoint));
        }
    }
}