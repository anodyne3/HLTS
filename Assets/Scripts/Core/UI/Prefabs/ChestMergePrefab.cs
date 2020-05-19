using Enums;
using MyScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Prefabs
{
    public class ChestMergePrefab : GlobalAccess
    {
        [SerializeField] private Button mergeButton;
        [SerializeField] private TMP_Text givenAmount;
        [SerializeField] private SVGImage givenIcon;
        [SerializeField] private TMP_Text receivedAmount;
        [SerializeField] private SVGImage receivedIcon;
        [SerializeField] private Transform upgradeIndicator;

        private ChestMergeVariable _chestMerge;
        private bool _isUpgraded;

        private void Start()
        {
            mergeButton.onClick.RemoveAllListeners();
            mergeButton.onClick.AddListener(MergeChests);
        }

        public void Init(ChestMergeVariable chestMergeVariable)
        {
            _chestMerge = chestMergeVariable;

            givenAmount.text = _chestMerge.requiredAmount.ToString();
            givenAmount.outlineColor = ChestManager.GetChestVariable((int) _chestMerge.requiredType).chestColor;
            givenIcon.sprite = ChestManager.GetChestIcon(_chestMerge.requiredType);

            receivedAmount.text = _chestMerge.receivedAmount.ToString();
            receivedAmount.outlineColor = ChestManager.GetChestVariable((int) _chestMerge.receivedType).chestColor;
            receivedIcon.sprite = ChestManager.GetChestIcon(_chestMerge.receivedType);

            RefreshButton();
        }

        public void RefreshButton()
        {
            _isUpgraded = _chestMerge.mergeUpgradeLevel <
                          UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.ChestMerge);
            
            var isNextUpgrade = _chestMerge.mergeUpgradeLevel ==
                               UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.ChestMerge);
            
            upgradeIndicator.gameObject.SetActive(isNextUpgrade);

            mergeButton.interactable = isNextUpgrade ||
                                       _isUpgraded && PlayerData.GetChestCount(_chestMerge.requiredType) >=
                                       _chestMerge.requiredAmount;
        }

        private void MergeChests()
        {
            if (_isUpgraded)
                FirebaseFunctionality.ChestMerge(_chestMerge.mergeUpgradeLevel.ToString());
            else
                PanelManager.OpenSubPanel<UpgradePanelController>(UpgradeTypes.ChestMerge);
        }
    }
}