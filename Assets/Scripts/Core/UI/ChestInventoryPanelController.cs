using System.Collections.Generic;
using Core.UI.Prefabs;
using DG.Tweening;
using Enums;
using MyScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.UI
{
    public class ChestInventoryPanelController : PanelController
    {
        [Header("Current Chest Progress")] [SerializeField]
        private Button claimCurrentChestButton;

        [SerializeField] private Button upgradeChestClaimButton;
        [SerializeField] private Image currentChestIcon;
        [SerializeField] private Slider currentChestProgressSlider;
        [SerializeField] private Image currentChestProgressFillImage;
        [SerializeField] private TMP_Text currentChestProgressText;
        [SerializeField] private TMP_Text claimCurrentChestButtonText;
        [SerializeField] private TweenPunchSetting tweenPunchSetting;

        [Header("Chest Inventory")] [SerializeField]
        private Transform chestInventoryContentHolder;

        [SerializeField] private ChestInventoryPrefab inventoryChestPrefab;

        private readonly List<ChestInventoryPrefab> _chests = new List<ChestInventoryPrefab>();

        public override void Start()
        {
            base.Start();

            claimCurrentChestButton.onClick.RemoveAllListeners();
            claimCurrentChestButton.onClick.AddListener(ClaimChest);
            upgradeChestClaimButton.onClick.RemoveAllListeners();
            upgradeChestClaimButton.onClick.AddListener(UpgradeChestClaim);

            ChestsInit();

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshFill);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.chestRefreshEvent, ChestsRefresh);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.chestOpenEvent, RefreshPanel);
        }

        private void ChestsInit()
        {
            var chestTypesLength = ChestManager.chestTypes.Length;
            for (var i = 0; i < chestTypesLength; i++)
            {
                var chest = Instantiate(inventoryChestPrefab, chestInventoryContentHolder);
                chest.Init(ChestManager.chestTypes[i].chestType);
                _chests.Add(chest);
            }
        }

        public override void OpenPanel(params object[] args)
        {
            base.OpenPanel();

            RefreshPanel();
        }

        private void RefreshPanel()
        {
            if (UpgradeManager.IsUpgradeMaxed(UpgradeTypes.ChestClaim))
                upgradeChestClaimButton.gameObject.SetActive(false);

            ChestsRefresh();
            RefreshFill();
        }

        private void RefreshFill()
        {
            var currentChestRank = ChestManager.CurrentChest.rank;
            currentChestProgressSlider.DOValue(ChestManager.GetFillAmount(currentChestRank),
                tweenPunchSetting.punchDuration);
            currentChestProgressText.text =
                PlayerData.currentChestRoll + " / " + ChestManager.CurrentChest.threshold;

            if (PlayerData.currentChestRoll != ChestManager.CurrentChest.threshold) return;

            var nextChestVariable = ChestManager.GetChestVariable(currentChestRank + 1);

            currentChestProgressFillImage.color = nextChestVariable.chestColor;
            currentChestProgressText.text = PlayerData.currentChestRoll + " / " + nextChestVariable.threshold;
            currentChestProgressSlider.DOValue(ChestManager.GetFillAmount(currentChestRank + 1),
                tweenPunchSetting.punchDuration);
            RefreshClaimChestButton();
        }

        private void ChestsRefresh()
        {
            RefreshClaimChestButton();
            currentChestProgressFillImage.color = ChestManager.CurrentChest.chestColor;
            var chestsCount = _chests.Count;
            for (var i = 0; i < chestsCount; i++)
            {
                _chests[i].Refresh();
            }
        }

        private void RefreshClaimChestButton()
        {
            var value = PlayerData.currentChestRoll >= ChestManager.chestTypes[0].threshold;
            claimCurrentChestButton.interactable = value;
            claimCurrentChestButtonText.color = value ? Color.white : Color.grey;
            currentChestIcon.color = value ? Color.white : Color.grey;
            currentChestIcon.sprite = PlayerData.currentChestRoll == ChestManager.CurrentChest.threshold
                ? ChestManager.GetChestVariable(ChestManager.CurrentChest.rank).chestIcon
                : ChestManager.GetChestVariable(ChestManager.CurrentChest.rank - 1).chestIcon;
        }

        private static void ClaimChest()
        {
            PanelManager.OpenSubPanel<ClaimChestPanelController>(ChestManager
                .GetChestVariable(ChestManager.CurrentChest.rank - 1).chestType);
        }

        private static void UpgradeChestClaim()
        {
            PanelManager.OpenSubPanel<UpgradePanelController>((int) UpgradeTypes.ChestClaim);
        }
    }
}