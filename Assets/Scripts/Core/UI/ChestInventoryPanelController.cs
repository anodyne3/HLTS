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
        [Header("Current Chest Progress")]
        [SerializeField] private Button claimCurrentChestButton;
        [SerializeField] private Image currentChestIcon;
        [SerializeField] private Slider currentChestProgressSlider;
        [SerializeField] private TMP_Text currentChestProgressText;
        [SerializeField] private TMP_Text claimCurrentChestButtonText;
        [Header("Chest Inventory")]
        [SerializeField] private Transform chestInventoryContentHolder;
        [SerializeField] private ChestInventoryPrefab inventoryChestPrefab;
        
        private readonly List<ChestInventoryPrefab> _chests = new List<ChestInventoryPrefab>();
    
        public override void Start()
        {
            base.Start();

            claimCurrentChestButton.onClick.RemoveAllListeners();
            claimCurrentChestButton.onClick.AddListener(ClaimCurrentChest);
            
            var chestTypesLength = ChestManager.chestTypes.Length;
            for (var i = 0; i < chestTypesLength; i++)
            {
                var chest = Instantiate(inventoryChestPrefab, chestInventoryContentHolder);
                chest.Init(ChestManager.chestTypes[i].chestType);
                _chests.Add(chest);
            }
            
            RefreshChests();
        }

        public override void OpenPanel(params object[] args)
        {
            base.OpenPanel();
            
            RefreshPanel();
        }

        private void RefreshPanel()
        {
            RefreshClaimChestButton(PlayerData.currentChestRoll >= ChestManager.chestTypes[0].threshold);

            currentChestIcon.sprite = ChestManager.CurrentChest.chestIcon;
            currentChestProgressSlider.value = ChestManager.GetFillAmount();
            currentChestProgressText.text = PlayerData.currentChestRoll + " / " + ChestManager.CurrentChest.threshold;

            RefreshChests();
        }

        private void RefreshClaimChestButton(bool value)
        {
            claimCurrentChestButton.interactable = value;
            claimCurrentChestButtonText.color = value ? Color.white : Color.grey;
            currentChestIcon.color = value ? Color.white : Color.grey;
        }

        private void RefreshChests()
        {
            var chestsCount = _chests.Count;
            for (var i = 0; i < chestsCount; i++)
            {
                _chests[i].Refresh();
            }
        }

        private static void ClaimCurrentChest()
        {
            if (PlayerData.currentChestRoll < Constants.HiChestRoll)
                PanelManager.OpenSubPanel<ClaimChestPanelController>();
            else
                FirebaseFunctionality.ClaimChest();
        }
    }
}