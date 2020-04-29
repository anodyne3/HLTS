using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Prefabs
{
    public class ChestInventoryPrefab : GlobalAccess
    {
        [SerializeField] private Button openChestButton;
        [SerializeField] private SVGImage chestIcon;
        [SerializeField] private TMP_Text chestCount;

        private ChestType _chestType;

        private void Start()
        {
            openChestButton.onClick.RemoveAllListeners();
            openChestButton.onClick.AddListener(OpenChestSubPanel);
        }

        public void Init(ChestType chestType)
        {
            _chestType = chestType;
            chestIcon.sprite = ChestManager.GetChestIcon(_chestType);
        }

        public void Refresh()
        {
            chestCount.text = PlayerData.GetChestCount(_chestType).ToString();
        }

        private void OpenChestSubPanel()
        {
            PanelManager.OpenSubPanel<ChestDetailsPanelController>(_chestType);
        }
    }
}
