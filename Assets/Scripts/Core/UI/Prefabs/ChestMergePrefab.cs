using Enums;
using MyScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.UI.Prefabs
{
    public class ChestMergePrefab : GlobalAccess
    {
        [SerializeField] private Button mergeButton;
        [SerializeField] private TMP_Text mergeText;
        [SerializeField] private TMP_Text givenAmount;
        [SerializeField] private SVGImage givenIcon;
        [SerializeField] private TMP_Text receivedAmount;
        [SerializeField] private SVGImage receivedIcon;
        [SerializeField] private Transform upgradeIndicator;

        private ChestMergeVariable _chestMerge;

        private bool IsUpgraded => _chestMerge.mergeUpgradeLevel <
                                   UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.ChestMerge);
        private bool IsNextUpgrade => _chestMerge.mergeUpgradeLevel ==
                                      UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.ChestMerge);

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
        }

        public void RefreshButton()
        {
            mergeText.text = IsUpgraded ? Constants.ChestButtonMerge : Constants.ChestButtonUpgrade;

            mergeButton.interactable = IsNextUpgrade || IsUpgraded;
        }

        public void RefreshIndicators()
        {
            upgradeIndicator.gameObject.SetActive(IsNextUpgrade);
        }

        private void MergeChests()
        {
            if (IsUpgraded)
            {
                if (PlayerData.GetChestCount(_chestMerge.requiredType) >= _chestMerge.requiredAmount)
                    FirebaseFunctionality.ChestMerge(_chestMerge.mergeUpgradeLevel.ToString());
                else
                    AlertMessage.Init("Not Enough Chests");
                
                PanelManager.PunchButton(transform);
            }
            else
                PanelManager.OpenSubPanel<UpgradePanelController>(UpgradeTypes.ChestMerge);
        }
    }
}