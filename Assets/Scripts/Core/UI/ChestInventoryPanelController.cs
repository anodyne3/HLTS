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
        [SerializeField] private TMP_Text claimButtonText;
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

        private bool ClaimIsUpgraded => UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.ChestClaim) > 0;

        public override void Start()
        {
            base.Start();

            claimCurrentChestButton.onClick.RemoveAllListeners();
            claimCurrentChestButton.onClick.AddListener(ClaimChest);
            upgradeChestClaimButton.onClick.RemoveAllListeners();
            upgradeChestClaimButton.onClick.AddListener(UpgradeChestClaim);

            ChestsInit();

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshFill, true);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.chestRefreshEvent, ChestsRefresh, true);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.chestOpenEvent, RefreshPanel);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.upgradeRefreshEvent, RefreshUpgradeButton);
        }

        private void ChestsInit()
        {
            var chestTypesLength = ChestManager.chestTypes.Length;
            for (var i = 0; i < chestTypesLength; i++)
            {
                var chest = Instantiate(inventoryChestPrefab, chestInventoryContentHolder);
                chest.Init(ChestManager.chestTypes[i].chestType);
            }
        }

        public override void OpenPanel(params object[] args)
        {
            base.OpenPanel();

            RefreshPanel();
        }

        private void RefreshPanel()
        {
            RefreshUpgradeButton();

            ChestsRefresh();
            RefreshFill();
        }

        private void RefreshUpgradeButton()
        {
            if (!isActiveAndEnabled) return;
            
            if (UpgradeManager.IsUpgradeMaxed(UpgradeTypes.ChestClaim))
                upgradeChestClaimButton.gameObject.SetActive(false);
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
            if (!isActiveAndEnabled) return;
            
            RefreshClaimChestButton();
            currentChestProgressFillImage.color = ChestManager.CurrentChest.chestColor;
        }

        private void RefreshClaimChestButton()
        {
            claimButtonText.text = ClaimIsUpgraded ? Constants.ChestButtonUpgrade : Constants.ChestButtonClaim;
            
            currentChestIcon.sprite = PlayerData.currentChestRoll == ChestManager.CurrentChest.threshold
                ? ChestManager.GetChestVariable(ChestManager.CurrentChest.rank).chestIcon
                : ChestManager.GetChestVariable(ChestManager.CurrentChest.rank - 1).chestIcon;
        }

        private void ClaimChest()
        {
            if (ClaimIsUpgraded)
                PanelManager.OpenSubPanel<ChestClaimPanelController>();
            else
                PanelManager.OpenSubPanel<UpgradePanelController>((int) UpgradeTypes.ChestClaim);
        }

        private static void UpgradeChestClaim()
        {
            PanelManager.OpenSubPanel<UpgradePanelController>((int) UpgradeTypes.ChestClaim);
        }
    }
}