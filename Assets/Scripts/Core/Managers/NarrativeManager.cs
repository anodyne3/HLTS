using System;
using Core.UI;
using Enums;
using Utils;
using MyScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Managers
{
    public class NarrativeManager : Singleton<NarrativeManager>
    {
        public NarrativePoint currentNarrativePoint;
        private NarrativePoint[] _narrativePoints;
        private int _narrativePointsLength;

        private void Start()
        {
            // EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinLoadEvent, CoinLoadTests);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshUiTests);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.armPullEvent, ArmPullTests);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.upgradeRefreshEvent,
                UpgradeRefreshTests);

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.narrativeRefreshEvent,
                OpenNarrativePanel);
        }

        //constants
        private const int ChestMergeTrigger = 20;
        private const int ChestClaimTrigger = 120;

        /*private void CoinLoadTests()
        {
            if ((NarrativeTypes) PlayerData.narrativeProgress != NarrativeTypes.PullLever) return;
            
            if (SlotMachine.BetAmount > 0)
                FirebaseFunctionality.ProgressNarrativePoint();
        }*/
        
        private static void RefreshUiTests()
        {
            switch ((NarrativeTypes) PlayerData.narrativeProgress)
            {
                case NarrativeTypes.ChestRoll:
                    if (PlayerData.GetChestCount(ChestType.Bronze) > 0)
                        FirebaseFunctionality.ProgressNarrativePoint();
                    break;
                case NarrativeTypes.ChestOpen:
                    if (CurrencyManager.GetCurrencyAmount(ResourceType.BluePrints) > 0)
                        FirebaseFunctionality.ProgressNarrativePoint();
                    break;
            }
        }

        private static void ArmPullTests()
        {
            if ((NarrativeTypes) PlayerData.narrativeProgress != NarrativeTypes.PullLever) return;

            if (PlayerData.currentChestRoll > 1)
                FirebaseFunctionality.ProgressNarrativePoint();
        }

        private static void UpgradeRefreshTests()
        {
            if ((NarrativeTypes) PlayerData.narrativeProgress != NarrativeTypes.PullLever) return;

            if (UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.CoinSlot) > 0)
                FirebaseFunctionality.ProgressNarrativePoint();
        }

        private static void OpenNarrativePanel()
        {
            if (PanelManager == null) return;

            switch ((NarrativeTypes) PlayerData.narrativeProgress)
            {
                case NarrativeTypes.Intro:
                    PanelManager.OpenPanelOnHold<NarrativePanelController>();
                    break;
                case NarrativeTypes.SlotMachine:
                    PanelManager.OpenPanelOnHold<NarrativePanelController>();
                    break;
                /*case NarrativeTypes.LoadCoin:
                    PanelManager.OpenPanelOnHold<NarrativePanelController>();
                    break;*/
                case NarrativeTypes.PullLever:
                    if (SlotMachine.BetAmount > 0)
                        PanelManager.OpenPanelOnHold<NarrativePanelController>();
                    break;
                case NarrativeTypes.ChestRoll:
                    if (ChestManager.GetFillAmount(ChestManager.CurrentChest.rank) > 0.0f)
                        PanelManager.OpenPanelOnHold<NarrativePanelController>();
                    break;
                case NarrativeTypes.ChestOpen:
                    if (PlayerData.GetChestCount(ChestType.Bronze) > 0)
                        PanelManager.OpenPanelOnHold<NarrativePanelController>();
                    break;
                case NarrativeTypes.Blueprints:
                    if (CurrencyManager.GetCurrencyAmount(ResourceType.BluePrints) > 0)
                        PanelManager.OpenPanelOnHold<NarrativePanelController>();
                    break;
                case NarrativeTypes.CoinSlotUpgrade:
                    PanelManager.OpenPanelOnHold<NarrativePanelController>();
                    break;
                case NarrativeTypes.UpgradeSlider:
                    if (UpgradeManager.IsSliderActive())
                        PanelManager.OpenPanelOnHold<NarrativePanelController>();
                    break;
                case NarrativeTypes.ChestMerge:
                    if (PlayerData.chestData[0] >= ChestMergeTrigger)
                        PanelManager.OpenPanelOnHold<NarrativePanelController>();
                    break;
                case NarrativeTypes.ChestClaim:
                    if (CurrencyManager.GetCurrencyAmount(ResourceType.BananaCoins) >= ChestClaimTrigger)
                        PanelManager.OpenPanelOnHold<NarrativePanelController>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void ShowHelpButton(bool value = true)
        {
            if (HudManager == null) return;

            HudManager.helpButton.gameObject.SetActive(value);
        }

        public void RefreshCurrentNarrativePoint()
        {
            currentNarrativePoint = (NarrativePoint) Resources.Load(
                Constants.NarrativePointsPath + (NarrativeTypes) PlayerData.narrativeProgress,
                typeof(NarrativePoint));
        }
    }
}