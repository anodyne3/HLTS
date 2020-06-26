using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private readonly WaitUntil _narrativeCallBlockWait =
            new WaitUntil(() => !FirebaseFunctionality.narrativeCallBlock);

        private NarrativePanelController _narrativePanel;
        private bool _openPanelBlock;

        [HideInInspector] public bool currentNarrativeSeen;
        [SerializeField] private bool[] _narrativeState;

        private void Start()
        {
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent,
                CheckFireAndForgetNarratives);

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.armPullEvent, ArmPullTests);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.upgradeRefreshEvent,
                UpgradeRefreshTests);

            /*
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinCreatedEvent,
                OpenIntroPanel);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshUiTests);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.narrativeRefreshEvent,
                OpenNarrativePanel);
            */
        }

        [SerializeField] private List<NarrativeTypes> _fireAndForgetNarratives = new List<NarrativeTypes>();
        [SerializeField] private Queue<NarrativeTypes> _persistentNarratives = new Queue<NarrativeTypes>();
        private int _cheapestCoins;

        public void Init()
        {
            SetupNarrativeState();
            LoadNarratives();
        }

        private void LoadNarratives()
        {
            var allNarratives = GeneralUtils.SortLoadedList<NarrativePoint>(Constants.NarrativePointsPath,
                (x, y) => x.id.CompareTo(y.id));

            var allNarrativesLength = allNarratives.Length;
            for (var i = 0; i < allNarrativesLength; i++)
            {
                if (_narrativeState[(int) allNarratives[i].id])
                    continue;

                if (allNarratives[i].persistent)
                    _persistentNarratives.Enqueue(allNarratives[i].id);
                else
                    _fireAndForgetNarratives.Add(allNarratives[i].id);
            }
        }

        public void GetNarrativeTestData()
        {
            //_cheapestCoins
            var shopProductsLength = ShopManager.shopProducts.Length;
            for (var i = 0; i < shopProductsLength; i++)
            {
                var x = ShopManager.shopProducts[i];
                if (x.ResourceType != ResourceType.StarFruits) continue;

                _cheapestCoins = x.ResourceCost;
                break;
            }
        }

        private void CheckFireAndForgetNarratives()
        {
            foreach (var narrativePoint in _fireAndForgetNarratives)
            {
                switch (narrativePoint)
                {
                    case NarrativeTypes.Intro:
                        if (_narrativePanel != null && _narrativePanel.gameObject.activeInHierarchy) return;

                        RefreshHelpButton();

                        _narrativePanel = PanelManager.GetPanel<NarrativePanelController>();
                        _narrativePanel.OpenPanel(narrativePoint, "blackoutBackground");
                        CurrencyManager.HideCurrencies(true);
                        break;
                    case NarrativeTypes.Starfruits:
                        if (_cheapestCoins == 0) break;
                        if (CurrencyManager.GetCurrencyAmount(ResourceType.StarFruits) >= _cheapestCoins)
                            StartCoroutine(DelayedOpen(2.0f,
                                () => PanelManager.OpenPanelOnHold<NarrativePanelController>(
                                    _gameManagerInteractionWait, narrativePoint)));
                        break;
                    case NarrativeTypes.Finale:
                        if (UpgradeManager.CurrentProgress() > UpgradeManager.SufficientProgress)
                            PanelManager.OpenPanelOnHold<NarrativePanelController>(
                                _gameManagerInteractionWait, narrativePoint);
                        break;
                    default:
                        break;
                }
            }
        }

        private void ProgressPersistentNarrative()
        {
            switch (_persistentNarratives.Peek())
            {
                case NarrativeTypes.PullLever:
                    if (currentNarrativeSeen) return;
                    StartCoroutine(DelayedOpen(2.0f,
                        () => PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait)));
                    break;
                case NarrativeTypes.ChestRoll:
                    if (currentNarrativeSeen)
                    {
                        if (PlayerData.GetChestCount(ChestType.Bronze) > 0)
                            FirebaseFunctionality.UpdateNarrativeProgress(NarrativeTypes.ChestRoll);
                    }
                    else
                    {
                        currentNarrativeSeen = true;

                        if (ChestManager.CurrentChest != null &&
                            ChestManager.GetFillAmount(ChestManager.CurrentChest.rank) < 0.1f)
                            StartCoroutine(DelayedOpen(6.0f,
                                () => PanelManager.OpenPanelOnHold<NarrativePanelController>(_payoutEventWait)));
                    }

                    break;
                case NarrativeTypes.ChestGained:
                    if (currentNarrativeSeen)
                    {
                        if (CurrencyManager.GetCurrencyAmount(ResourceType.BluePrints) > 0)
                            FirebaseFunctionality.UpdateNarrativeProgress(NarrativeTypes.ChestGained);
                    }
                    else
                    {
                        currentNarrativeSeen = true;

                        if (PlayerData.GetChestCount(ChestType.Bronze) > 0)
                            StartCoroutine(DelayedOpen(2.0f, () =>
                                PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait)));
                    }

                    break;
                case NarrativeTypes.Blueprints:
                    if (currentNarrativeSeen)
                    {
                        if (CurrencyManager.GetCurrencyAmount(ResourceType.BluePrints) > UpgradeManager
                                .GetUpgradeVariable(UpgradeTypes.CoinSlot).CurrentResourceRequirements[1]
                                .resourceAmount)
                            FirebaseFunctionality.UpdateNarrativeProgress(NarrativeTypes.Blueprints);
                    }
                    else
                    {
                        currentNarrativeSeen = true;

                        PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait);
                    }

                    break;
                case NarrativeTypes.CoinSlotUpgrade:
                    PanelManager.OpenPanelOnHold<NarrativePanelController>(_payoutEventWait);
                    break;
                case NarrativeTypes.UpgradeSlider:
                    StartCoroutine(DelayedOpen(2.0f,
                        () => PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait)));
                    break;
                case NarrativeTypes.UpgradeMerge:
                    if (CurrencyManager.GetCurrencyAmount(ResourceType.BluePrints) >= Constants.ChestMergeTrigger)
                        StartCoroutine(DelayedOpen(2.0f, () =>
                            PanelManager.OpenPanelOnHold<NarrativePanelController>(_payoutEventWait)));
                    break;
                case NarrativeTypes.UpgradeClaim:
                    if (CurrencyManager.GetCurrencyAmount(ResourceType.BluePrints) >= Constants.ChestClaimTrigger)
                        StartCoroutine(DelayedOpen(2.0f, () =>
                            PanelManager.OpenPanelOnHold<NarrativePanelController>(_payoutEventWait)));
                    break;
            }
        }

        /*private void OpenIntroPanel()
        {
            if ((NarrativeTypes) PlayerData.narrativeProgress != NarrativeTypes.Intro) return;

            if (_narrativePanel != null && _narrativePanel.gameObject.activeInHierarchy) return;

            RefreshHelpButton();

            _narrativePanel = PanelManager.GetPanel<NarrativePanelController>();
            _narrativePanel.OpenPanel(NarrativeTypes.Intro, "blackoutBackground");
            CurrencyManager.HideCurrencies(true);
        }*/

        private void ArmPullTests()
        {
            if (_persistentNarratives.Peek() != NarrativeTypes.PullLever) return;

            FirebaseFunctionality.UpdateNarrativeProgress(NarrativeTypes.PullLever);
        }

        private void UpgradeRefreshTests()
        {
            switch (_persistentNarratives.Peek())
            {
                case NarrativeTypes.CoinSlotUpgrade:
                    if (UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.CoinSlot) > 0)
                        FirebaseFunctionality.UpdateNarrativeProgress(NarrativeTypes.CoinSlotUpgrade);
                    break;
                case NarrativeTypes.UpgradeMerge:
                    if (UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.ChestMerge) > 0)
                        FirebaseFunctionality.UpdateNarrativeProgress(NarrativeTypes.UpgradeMerge);
                    break;
                case NarrativeTypes.UpgradeClaim:
                    if (UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.ChestClaim) > 0)
                        FirebaseFunctionality.UpdateNarrativeProgress(NarrativeTypes.UpgradeClaim);
                    break;
            }
        }

        /*private void RefreshUiTests()
        {
            switch ((NarrativeTypes) PlayerData.narrativeProgress)
            {
                case NarrativeTypes.ChestRoll:
                    if (PlayerData.GetChestCount(ChestType.Bronze) > 0)
                        FirebaseFunctionality.UpdateNarrativeProgress(NarrativeTypes.ChestRoll);
                    break;
                case NarrativeTypes.ChestGained:
                    if (CurrencyManager.GetCurrencyAmount(ResourceType.BluePrints) > 0)
                        FirebaseFunctionality.UpdateNarrativeProgress(NarrativeTypes.ChestGained);
                    break;
                case NarrativeTypes.Blueprints:
                    if (CurrencyManager.GetCurrencyAmount(ResourceType.BluePrints) > UpgradeManager
                            .GetUpgradeVariable(UpgradeTypes.CoinSlot).CurrentResourceRequirements[1].resourceAmount)
                        FirebaseFunctionality.UpdateNarrativeProgress(NarrativeTypes.Blueprints);
                    break;
                /*case NarrativeTypes.Starfruits:
                    if (currentNarrativeSeen) return;
                    var cheapestCoins = 0;

                    var shopProductsLength = ShopManager.shopProducts.Length;
                    for (var i = 0; i < shopProductsLength; i++)
                    {
                        var x = ShopManager.shopProducts[i];
                        if (x.ResourceType != ResourceType.StarFruits) continue;

                        cheapestCoins = x.ResourceCost;
                        break;
                    }

                    if (CurrencyManager.GetCurrencyAmount(ResourceType.StarFruits) >= cheapestCoins)
                        StartCoroutine(DelayedOpen(2.0f,
                            () => PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait)));
                    break;#1#
                case NarrativeTypes.CoinSlotUpgrade:
                    if (currentNarrativeSeen) return;
                    currentNarrativeSeen = true;
                    PanelManager.OpenPanelOnHold<NarrativePanelController>(_payoutEventWait);
                    break;
                case NarrativeTypes.UpgradeMerge:
                    if (currentNarrativeSeen) return;
                    if (CurrencyManager.GetCurrencyAmount(ResourceType.BluePrints) >= Constants.ChestMergeTrigger)
                        StartCoroutine(DelayedOpen(2.0f, () =>
                            PanelManager.OpenPanelOnHold<NarrativePanelController>(_payoutEventWait)));
                    break;
                case NarrativeTypes.UpgradeClaim:
                    if (currentNarrativeSeen) return;
                    if (CurrencyManager.GetCurrencyAmount(ResourceType.BluePrints) >= Constants.ChestClaimTrigger)
                        StartCoroutine(DelayedOpen(2.0f, () =>
                            PanelManager.OpenPanelOnHold<NarrativePanelController>(_payoutEventWait)));
                    break;
                default:
                    return;
            }
        }*/

        private IEnumerator DelayedOpen(float waitTime, Action callback)
        {
            if (_openPanelBlock) yield break;

            _openPanelBlock = true;

            yield return new WaitForSeconds(waitTime);

            callback.Invoke();

            if (FirebaseFunctionality.narrativeCallBlock)
                yield return _narrativeCallBlockWait;

            _openPanelBlock = false;
        }

        /*private void OpenNarrativePanel()
        {
            if (PanelManager == null) return;

            switch ((NarrativeTypes) PlayerData.narrativeProgress)
            {
                case NarrativeTypes.PullLever:
                    StartCoroutine(DelayedOpen(2.0f,
                        () => PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait)));
                    break;
                case NarrativeTypes.ChestRoll:
                    if (ChestManager.CurrentChest != null &&
                        ChestManager.GetFillAmount(ChestManager.CurrentChest.rank) < 0.1f)
                        StartCoroutine(DelayedOpen(6.0f,
                            () => PanelManager.OpenPanelOnHold<NarrativePanelController>(_payoutEventWait)));
                    break;
                case NarrativeTypes.ChestGained:
                    if (PlayerData.GetChestCount(ChestType.Bronze) > 0)
                        StartCoroutine(DelayedOpen(2.0f, () =>
                            PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait)));
                    break;
                case NarrativeTypes.Blueprints:
                    PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait);
                    break;
                case NarrativeTypes.UpgradeSlider:
                    StartCoroutine(DelayedOpen(2.0f,
                        () => PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait)));
                    break;
            }
        }*/

        public void RefreshHelpButton()
        {
            if (HudManager == null) return;

            switch (_persistentNarratives.Peek())
            {
                case NarrativeTypes.PullLever:
                case NarrativeTypes.ChestRoll:
                case NarrativeTypes.ChestGained:
                case NarrativeTypes.Blueprints:
                case NarrativeTypes.CoinSlotUpgrade:
                case NarrativeTypes.UpgradeMerge:
                case NarrativeTypes.UpgradeClaim:
                    if (currentNarrativeSeen)
                        HudManager.helpButton.gameObject.SetActive(true);
                    break;
                /*case NarrativeTypes.PullLever:
                case NarrativeTypes.ChestRoll:
                case NarrativeTypes.ChestGained:
                case NarrativeTypes.Blueprints:
                case NarrativeTypes.CoinSlotUpgrade:
                    HudManager.helpButton.gameObject.SetActive(true);
                    break;
                case NarrativeTypes.UpgradeMerge:
                case NarrativeTypes.UpgradeClaim:
                    if (currentNarrativeSeen)
                        HudManager.helpButton.gameObject.SetActive(true);
                    break;*/
                default:
                    HudManager.helpButton.gameObject.SetActive(false);
                    break;
            }
        }

        /*public void RefreshCurrentNarrativePoint()
        {
            currentNarrativePoint = (NarrativePoint) Resources.Load(
                Constants.NarrativePointsPath + (NarrativeTypes) PlayerData.narrativeProgress,
                typeof(NarrativePoint));
        }*/

        public static NarrativePoint LoadNarrativePoint(NarrativeTypes narrativeType)
        {
            return (NarrativePoint) Resources.Load(Constants.NarrativePointsPath + narrativeType,
                typeof(NarrativePoint));
        }

        private void SetupNarrativeState()
        {
            var byteArray = new BitArray(new[] {(int) PlayerData.narrativeProgress});
            _narrativeState = new bool[byteArray.Count];
            byteArray.CopyTo(_narrativeState, 0);
        }

        public string UpdateNarrativeState(NarrativeTypes narrativeType)
        {
            _narrativeState[(int) narrativeType] = true;

            var newNarrativeState = 0;

            var narrativeStateLength = _narrativeState.Length;
            for (var i = 0; i < narrativeStateLength; i++)
                if (_narrativeState[i])
                    newNarrativeState |= 1 << i;

            if (_fireAndForgetNarratives.Remove(narrativeType))
                return newNarrativeState.ToString();

            _persistentNarratives.Dequeue();
            currentNarrativeSeen = false;

            return newNarrativeState.ToString();
        }
    }
}