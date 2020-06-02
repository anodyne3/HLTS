using Enums;
using MyScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.UI.Prefabs
{
    public class ChestClaimPrefab : GlobalAccess
    {
        [SerializeField] private SVGImage chestIcon;
        [SerializeField] private TMP_Text claimButtonText;
        [SerializeField] private Button chestClaimButton;
        [SerializeField] private Transform upgradeIndicator;

        private ChestVariable _chestVariable;

        private bool IsUpgraded => _chestVariable.rank < UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.ChestClaim);

        private bool IsNextUpgrade =>
            _chestVariable.rank == UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.ChestClaim);

        // private ChestClaimPanelController _chestClaimPanel;

        private void Start()
        {
            chestClaimButton.onClick.RemoveAllListeners();
            chestClaimButton.onClick.AddListener(ClaimChest);

            // _chestClaimPanel = PanelManager.GetPanel<ChestClaimPanelController>();
        }

        public void Init(ChestVariable chestType)
        {
            _chestVariable = chestType;
            chestIcon.sprite = _chestVariable.chestIcon;
        }

        public void RefreshButton()
        {
            var canClaim = PlayerData.currentChestRoll >= _chestVariable.threshold;

            claimButtonText.text = IsUpgraded ? Constants.ChestButtonClaim : Constants.ChestButtonUpgrade;
            chestClaimButton.interactable = IsNextUpgrade || IsUpgraded && canClaim;
        }

        public void RefreshUpgradeIndicators()
        {
            upgradeIndicator.gameObject.SetActive(IsNextUpgrade);
        }

        private void ClaimChest()
        {
            if (IsUpgraded)
            {
                FirebaseFunctionality.ClaimChest(_chestVariable.chestType);
                PanelManager.PunchButton(transform);
            }
            else
                PanelManager.OpenSubPanel<UpgradePanelController>(UpgradeTypes.ChestClaim);
        }
    }
}