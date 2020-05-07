using System.Collections.Generic;
using Core.UI.Prefabs;
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

        [SerializeField] private Image currentChestIcon;
        [SerializeField] private Slider currentChestProgressSlider;
        [SerializeField] private Image currentChestProgressFillImage;
        [SerializeField] private TMP_Text currentChestProgressText;
        [SerializeField] private TMP_Text claimCurrentChestButtonText;

        [Header("Chest Inventory")] [SerializeField]
        private Transform chestInventoryContentHolder;

        [SerializeField] private ChestInventoryPrefab inventoryChestPrefab;

        private readonly List<ChestInventoryPrefab> _chests = new List<ChestInventoryPrefab>();

        public override void Start()
        {
            base.Start();

            claimCurrentChestButton.onClick.RemoveAllListeners();
            claimCurrentChestButton.onClick.AddListener(ClaimChest);

            ChestsInit();

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.payoutFinishEvent, ChestsRefresh);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.chestRefreshEvent, PanelRefresh);

            ChestsRefresh();
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

            PanelRefresh();
        }

        private void PanelRefresh()
        {
            RefreshClaimChestButton(PlayerData.currentChestRoll >= ChestManager.chestTypes[0].threshold);

            currentChestIcon.sprite = ChestManager.CurrentChest.chestIcon;
            currentChestProgressSlider.value = ChestManager.GetFillAmount();
            currentChestProgressFillImage.color = ChestManager.CurrentChest.chestColor;
            currentChestProgressText.text = PlayerData.currentChestRoll + " / " + ChestManager.CurrentChest.threshold;

            ChestsRefresh();
        }

        private void ChestsRefresh()
        {
            var chestsCount = _chests.Count;
            for (var i = 0; i < chestsCount; i++)
            {
                _chests[i].Refresh();
            }
        }

        private void RefreshClaimChestButton(bool value)
        {
            claimCurrentChestButton.interactable = value;
            claimCurrentChestButtonText.color = value ? Color.white : Color.grey;
            currentChestIcon.color = value ? Color.white : Color.grey;
        }

        private static void ClaimChest()
        {
            PanelManager.OpenSubPanel<ClaimChestPanelController>();
        }
    }
}