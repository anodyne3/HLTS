using MyScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class UpgradeRepairPanelController : PanelController
    {
        [SerializeField] private TMP_Text headerText;
        [SerializeField] private Button upgradeRepairButton;
        [SerializeField] private TMP_Text upgradeRepairButtonText;
        [SerializeField] private Button cancelButton;
        [SerializeField] private SVGImage targetIcon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;

        private UpgradeRepairVariable _upgradeRepairVariable;

        public override void Start()
        {
            base.Start();

            upgradeRepairButton.onClick.RemoveAllListeners();
            upgradeRepairButton.onClick.AddListener(UpgradeRepair);
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(ClosePanel);
        }

        public override void OpenPanel(params object[] args)
        {
            base.OpenPanel();

            _upgradeRepairVariable = (UpgradeRepairVariable) args[0];

            RefreshPanel();
        }

        private void RefreshPanel()
        {
            RefreshUpgradeRepairButton(ResourceCheck(_upgradeRepairVariable.resourceRequirements));

            upgradeRepairButtonText.text = _upgradeRepairVariable.upgrade ? "Upgrade" : "Repair";
            headerText.text = _upgradeRepairVariable.upgrade ? "Upgrade" : "Repair";
            targetIcon.sprite = _upgradeRepairVariable.icon;
            nameText.text = _upgradeRepairVariable.deviceName;
            descriptionText.text = _upgradeRepairVariable.description;
        }

        private bool ResourceCheck(Currency[] requiredResources)
        {
            var enoughResources = true;

            var requiredResourcesLength = requiredResources.Length;
            for (var i = 0; i < requiredResourcesLength; i++)
            {
                if (PlayerData.GetResourceAmount(requiredResources[i].currencyType) >=
                    requiredResources[i].currencyAmount)
                    continue;

                enoughResources = false;
            }

            return enoughResources;
        }


        private void RefreshUpgradeRepairButton(bool value)
        {
            upgradeRepairButton.interactable = value;
            upgradeRepairButtonText.color = value ? Color.white : Color.grey;
        }

        private void UpgradeRepair()
        {
            FirebaseFunctionality.UpgradeRepair(_upgradeRepairVariable);
        }
    }
}