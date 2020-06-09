using Core.Upgrades;
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
        private UpgradeIndicatorUi _upgradeIndicatorUi;

        private int _oldCount;
        private ChestType _chestType;

        private void Start()
        {
            openChestButton.onClick.RemoveAllListeners();
            openChestButton.onClick.AddListener(OpenChestSubPanel);
            
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, HideUpgradeIndicator,
                true);
        }

        public void Init(ChestType chestType)
        {
            _chestType = chestType;
            chestIcon.sprite = ChestManager.GetChestIcon(_chestType);

            _oldCount = PlayerData.GetChestCount(_chestType);
            _upgradeIndicatorUi = (UpgradeIndicatorUi) GetComponentInChildren(typeof(UpgradeIndicatorUi));
            
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.chestRefreshEvent, Refresh, true);
        }

        private void Refresh()
        {
            var chestDifference = PlayerData.GetChestCount(_chestType) - _oldCount;

            _oldCount += chestDifference;
            chestCount.text = _oldCount.ToString();

            if (chestDifference <= 0) return;

            ChestManager.AddChestsAnim(_chestType, chestDifference);
        }

        private void HideUpgradeIndicator()
        {
            switch (_chestType)
            {
                case ChestType.Bronze:
                    _upgradeIndicatorUi.gameObject.SetActive(
                        UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.ChestMerge) < 3);
                    break;
                case ChestType.Silver:
                    _upgradeIndicatorUi.gameObject.SetActive(
                        UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.ChestMerge) > 2);
                    break;
                case ChestType.Gold:
                    _upgradeIndicatorUi.gameObject.SetActive(false);
                    break;
            }
        }

        private void OpenChestSubPanel()
        {
            PanelManager.OpenSubPanel<ChestDetailsPanelController>(_chestType);
        }
    }
}