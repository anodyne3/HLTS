using Core.Upgrades;
using Enums;
using MyScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.UI
{
    public class ChestDetailsPanelController : PanelController
    {
        [SerializeField] private TMP_Text chestName;
        [SerializeField] private TMP_Text chestAmount;
        [SerializeField] private SVGImage chestClosedIcon;
        [SerializeField] private SVGImage chestOpenIcon;
        [SerializeField] private Button openChestButton;
        [SerializeField] private Button chestMergeButton;
        [SerializeField] private TMP_Text bcMaxText;
        [SerializeField] private TMP_Text bpMaxText;
        [SerializeField] private TMP_Text sfMaxText;
        [SerializeField] private UpgradeIndicatorUi upgradeIndicator;

        private ChestVariable _chestVariable;

        public override void Start()
        {
            base.Start();

            openChestButton.onClick.RemoveAllListeners();
            openChestButton.onClick.AddListener(OpenChest);
            chestMergeButton.onClick.RemoveAllListeners();
            chestMergeButton.onClick.AddListener(OpenMergePanel);

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshPanel);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.chestRefreshEvent, RefreshPanel);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.chestOpenEvent, CloseChestIcon);
        }

        public override void OpenPanel(params object[] args)
        {
            base.OpenPanel();

            var chestType = (ChestType) args[0];

            _chestVariable = ChestManager.chestTypes[(int) chestType];

            InitPanel();
        }

        private void InitPanel()
        {
            if (_chestVariable == null) return;

            chestName.text = _chestVariable.chestName;
            chestClosedIcon.sprite = _chestVariable.chestIcon;
            chestOpenIcon.sprite = _chestVariable.chestOpenIcon;
            bcMaxText.text = Constants.BananaCoinIcon + Constants.ChestRewardPrefix + _chestVariable.bcMax;
            bpMaxText.text = Constants.BluePrintIcon + Constants.ChestRewardPrefix + _chestVariable.bpMax;
            sfMaxText.text = Constants.StarFruitIcon + Constants.ChestRewardPrefix + _chestVariable.sfMax;

            RefreshPanel();
            CloseChestIcon();
        }

        private void RefreshPanel()
        {
            var chestCount = PlayerData.GetChestCount(_chestVariable.chestType);
            openChestButton.interactable = chestCount > 0;
            chestAmount.text = chestCount.ToString();
            HideUpgradeIndicator();
        }

        private void HideUpgradeIndicator()
        {
            switch (_chestVariable.chestType)
            {
                case ChestType.Bronze:
                    upgradeIndicator.gameObject.SetActive(
                        UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.ChestMerge) < 3);
                    break;
                case ChestType.Silver:
                    upgradeIndicator.gameObject.SetActive(
                        UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.ChestMerge) > 2);
                    break;
                case ChestType.Gold:
                    upgradeIndicator.gameObject.SetActive(false);
                    break;
            }
        }

        private void CloseChestIcon()
        {
            OpenChestIcon(false);
        }

        public void OpenChestIcon(bool value)
        {
            chestOpenIcon.enabled = value;
            chestClosedIcon.enabled = !value;
        }

        private void OpenChest()
        {
            if (PlayerData.GetChestCount(_chestVariable.chestType) < 1) return;

            FirebaseFunctionality.ChestOpen(_chestVariable.chestType);
        }

        private static void OpenMergePanel()
        {
            PanelManager.OpenSubPanel<ChestMergePanelController>();
        }
    }
}