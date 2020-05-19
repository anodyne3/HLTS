using Enums;
using MyScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Prefabs
{
    public class ChestClaimPrefab : GlobalAccess
    {
        [SerializeField] private SVGImage chestIcon;
        [SerializeField] private TMP_Text claimButtonText;
        [SerializeField] private Button chestClaimButton;
        [SerializeField] private Transform upgradeIndicator;

        private ChestVariable _chestVariable;
        private bool _isUpgraded;
        private ChestClaimPanelController _chestClaimPanel;

        private void Start()
        {
            chestClaimButton.onClick.RemoveAllListeners();
            chestClaimButton.onClick.AddListener(ClaimChest);

            _chestClaimPanel = PanelManager.GetPanel<ChestClaimPanelController>();
        }

        public void Init(ChestVariable chestType)
        {
            _chestVariable = chestType;
            chestIcon.sprite = _chestVariable.chestIcon;
        }

        public void RefreshButton()
        {
            var canClaim = PlayerData.currentChestRoll >= _chestVariable.threshold;

            _isUpgraded = _chestVariable.rank < UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.ChestClaim);

            var isNextUpgrade = _chestVariable.rank ==
                                UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.ChestClaim);

            upgradeIndicator.gameObject.SetActive(isNextUpgrade);
            
            chestClaimButton.interactable = isNextUpgrade || _isUpgraded && canClaim;
            claimButtonText.color = _isUpgraded && canClaim ? Color.white : Color.grey;
            chestIcon.color = _isUpgraded && canClaim ? Color.white : Color.grey;
        }

        private void ClaimChest()
        {
            if (_isUpgraded)
            {
                FirebaseFunctionality.ClaimChest(_chestVariable.chestType);
                _chestClaimPanel.punchSetting.DoPunch(chestClaimButton.transform, false);
                if (PlayerData.currentChestRoll >= ChestManager.GetChestVariable(0).threshold)
                    _chestClaimPanel.ClosePanelRemote();
            }
            else
                PanelManager.OpenSubPanel<UpgradePanelController>(UpgradeTypes.ChestClaim);
        }
    }
}