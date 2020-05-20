using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.UI.Prefabs
{
    public class ChestInventoryPrefab : GlobalAccess
    {
        [SerializeField] private Button openChestButton;
        [SerializeField] private SVGImage chestIcon;
        [SerializeField] private TMP_Text chestCount;

        private int _oldCount;
        private ChestType _chestType;

        private void Start()
        {
            openChestButton.onClick.RemoveAllListeners();
            openChestButton.onClick.AddListener(OpenChestSubPanel);
            
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.chestRefreshEvent, Refresh);
        }

        public void Init(ChestType chestType)
        {
            _chestType = chestType;
            chestIcon.sprite = ChestManager.GetChestIcon(_chestType);
            
            _oldCount = PlayerData.GetChestCount(_chestType);
            
            Refresh();
        }

        private void Refresh()
        {
            var chestDifference = PlayerData.GetChestCount(_chestType) - _oldCount;
            
            _oldCount += chestDifference;
            chestCount.text = _oldCount.ToString();

            if (chestDifference <= 0) return;
            
            ChestManager.AddChestsAnim(_chestType, chestDifference);
        }

        private void OpenChestSubPanel()
        {
            PanelManager.OpenSubPanel<ChestDetailsPanelController>(_chestType);
        }
    }
}
