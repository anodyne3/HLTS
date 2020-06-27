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
        private readonly List<NarrativeTypes> _fireAndForgetNarratives = new List<NarrativeTypes>();
        private readonly Queue<NarrativeTypes> _persistentNarratives = new Queue<NarrativeTypes>();
        private readonly WaitUntil _gameManagerInteractionWait = new WaitUntil(() => GameManager.interactionEnabled);
        private readonly WaitUntil _payoutEventWait = new WaitUntil(() => !SlotMachine.wheelsAreRolling);
        private readonly WaitUntil _narrativeCallBlockWait =
            new WaitUntil(() => !FirebaseFunctionality.narrativeCallBlock);

        [HideInInspector] public NarrativeTypes currentNarrativePointType;
        [HideInInspector] public bool currentNarrativeSeen;

        private NarrativePanelController _narrativePanel;
        private bool _openPanelBlock;
        private Transform _persistentNarrativeEvents;
        private int _cheapestCoins;
        private bool[] _narrativeState;

        private void Start()
        {
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent,
                CheckFireAndForgetNarratives);

            _persistentNarrativeEvents = new GameObject("PersistentNarrativeEvents").transform;
            _persistentNarrativeEvents.SetParent(GameManager.transform);

            EventManager.NewEventSubscription(_persistentNarrativeEvents.gameObject,
                Constants.GameEvents.refreshUiEvent,
                CheckPersistentNarrative);
            EventManager.NewEventSubscription(_persistentNarrativeEvents.gameObject, Constants.GameEvents.armPullEvent,
                ArmPullTests);
            EventManager.NewEventSubscription(_persistentNarrativeEvents.gameObject,
                Constants.GameEvents.upgradeRefreshEvent,
                UpgradeRefreshTests);
        }

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

        private void CheckFireAndForgetNarratives()
        {
            if (BlockDoubleOpening()) return;

            foreach (var narrativePoint in _fireAndForgetNarratives)
            {
                switch (narrativePoint)
                {
                    case NarrativeTypes.Intro:
                        RefreshHelpButton();
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
                        if (UpgradeManager.CurrentProgress() >= UpgradeManager.SufficientProgress)
                            PanelManager.OpenPanelOnHold<NarrativePanelController>(
                                _gameManagerInteractionWait, narrativePoint);
                        break;
                }
            }
        }

        private void CheckPersistentNarrative()
        {
            if (_persistentNarratives.Count == 0)
            {
                Destroy(_persistentNarrativeEvents.gameObject);
                return;
            }

            if (BlockDoubleOpening()) return;

            var narrativePoint = _persistentNarratives.Peek();

            switch (narrativePoint)
            {
                case NarrativeTypes.PullLever:
                    if (currentNarrativeSeen) return;
                    StartCoroutine(DelayedOpen(2.0f,
                        () => PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait,
                            narrativePoint)));
                    break;
                case NarrativeTypes.ChestRoll:
                    if (currentNarrativeSeen)
                    {
                        if (PlayerData.GetChestCount(ChestType.Bronze) > 0)
                            FirebaseFunctionality.UpdateNarrativeProgress(NarrativeTypes.ChestRoll);
                    }
                    else
                    {
                        if (ChestManager.CurrentChest != null &&
                            ChestManager.GetFillAmount(ChestManager.CurrentChest.rank) > 0.01f)
                            StartCoroutine(DelayedOpen(2.0f,
                                () => PanelManager.OpenPanelOnHold<NarrativePanelController>(_payoutEventWait,
                                    narrativePoint)));
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
                        if (PlayerData.GetChestCount(ChestType.Bronze) > 0)
                            PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait,
                                narrativePoint);
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
                        PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait,
                            narrativePoint);

                    break;
                case NarrativeTypes.CoinSlotUpgrade:
                    if (currentNarrativeSeen) return;
                    PanelManager.OpenPanelOnHold<NarrativePanelController>(_payoutEventWait,
                            narrativePoint);
                    break;
                case NarrativeTypes.UpgradeSlider:
                    if (currentNarrativeSeen) return;
                    StartCoroutine(DelayedOpen(2.0f,
                        () => PanelManager.OpenPanelOnHold<NarrativePanelController>(_gameManagerInteractionWait,
                            narrativePoint)));
                    break;
                case NarrativeTypes.UpgradeMerge:
                    if (currentNarrativeSeen) return;
                    if (CurrencyManager.GetCurrencyAmount(ResourceType.BluePrints) >= Constants.ChestMergeTrigger)
                        StartCoroutine(DelayedOpen(2.0f, () =>
                            PanelManager.OpenPanelOnHold<NarrativePanelController>(_payoutEventWait, narrativePoint)));
                    break;
                case NarrativeTypes.UpgradeClaim:
                    if (currentNarrativeSeen) return;
                    if (CurrencyManager.GetCurrencyAmount(ResourceType.BluePrints) >= Constants.ChestClaimTrigger)
                        StartCoroutine(DelayedOpen(2.0f, () =>
                            PanelManager.OpenPanelOnHold<NarrativePanelController>(_payoutEventWait, narrativePoint)));
                    break;
            }
        }

        private bool BlockDoubleOpening()
        {
            if (_narrativePanel == null)
                _narrativePanel = PanelManager.GetPanel<NarrativePanelController>();

            return _narrativePanel.gameObject.activeInHierarchy;
        }

        private void ArmPullTests()
        {
            if (_persistentNarratives.Count == 0)
            {
                Destroy(_persistentNarrativeEvents.gameObject);
                return;
            }

            if (_persistentNarratives.Peek() != NarrativeTypes.PullLever) return;

            FirebaseFunctionality.UpdateNarrativeProgress(NarrativeTypes.PullLever);
        }

        private void UpgradeRefreshTests()
        {
            if (_persistentNarratives.Count == 0)
            {
                Destroy(_persistentNarrativeEvents.gameObject);
                return;
            }

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

        public void RefreshHelpButton()
        {
            if (HudManager == null) return;

            if (_persistentNarratives.Count == 0)
            {
                HudManager.helpButton.gameObject.SetActive(false);
                return;
            }

            switch (_persistentNarratives.Peek())
            {
                case NarrativeTypes.PullLever:
                case NarrativeTypes.ChestRoll:
                case NarrativeTypes.ChestGained:
                case NarrativeTypes.Blueprints:
                case NarrativeTypes.CoinSlotUpgrade:
                case NarrativeTypes.UpgradeMerge:
                case NarrativeTypes.UpgradeClaim:
                    HudManager.helpButton.gameObject.SetActive(currentNarrativeSeen);
                    break;
                default:
                    HudManager.helpButton.gameObject.SetActive(false);
                    break;
            }
        }

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

        public void GetNarrativeCheapestCoinsCost()
        {
            var shopProductsLength = ShopManager.shopProducts.Length;
            for (var i = 0; i < shopProductsLength; i++)
            {
                var x = ShopManager.shopProducts[i];
                if (x.ResourceType != ResourceType.StarFruits) continue;

                _cheapestCoins = x.ResourceCost;
                break;
            }
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

            currentNarrativeSeen = false;
            _persistentNarratives.Dequeue();

            if (_persistentNarratives.Count == 0)
                Destroy(_persistentNarrativeEvents.gameObject);

            RefreshHelpButton();

            return newNarrativeState.ToString();
        }
    }
}