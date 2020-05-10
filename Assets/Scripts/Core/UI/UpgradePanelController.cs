using System.Collections.Generic;
using Core.UI.Prefabs;
using MyScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.UI
{
    public class UpgradePanelController : PanelController
    {
        [SerializeField] private TMP_Text headerText;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private TMP_Text upgradeButtonText;
        [SerializeField] private Button cancelButton;
        [SerializeField] private SVGImage targetIcon;
        [SerializeField] private TMP_Text upgradeNameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private ResourceRequirement resourceRequirementPrefab;
        [SerializeField] private Transform resourceRequirementTransform;

        private UpgradeVariable _upgradeVariable;
        private readonly List<ResourceRequirement> _activeRequirements = new List<ResourceRequirement>();

        public override void Start()
        {
            base.Start();

            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(Upgrade);
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(ClosePanel);

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshUi);
        }

        public override void OpenPanel(params object[] args)
        {
            base.OpenPanel();

            _upgradeVariable = UpgradeManager.GetUpgradeVariable((int) args[0]);

            RefreshPanel();
        }

        private void RefreshPanel()
        {
            upgradeButtonText.text = _upgradeVariable.IsUpgrade ? "Upgrade" : "Repair";
            headerText.text = _upgradeVariable.IsUpgrade ? "Upgrade" : "Repair";
            targetIcon.sprite = _upgradeVariable.icon;
            upgradeNameText.text = _upgradeVariable.upgradeName;
            descriptionText.text = _upgradeVariable.description;
            levelText.text = _upgradeVariable.IsUpgrade ? "Level " + _upgradeVariable.currentLevel : "Needs Repair";

            RefreshUi();
        }
        
        private void RefreshUi()
        {
            RefreshResourceRequirements();
            RefreshUpgradeButton(UpgradeManager.HasResourcesForUpgrade(_upgradeVariable.id));
        }

        private void RefreshResourceRequirements()
        {
            var resourceRequirementsLength = _upgradeVariable.resourceRequirements.Length;
            var missingPrefabCount = resourceRequirementsLength - _activeRequirements.Count;

            if (missingPrefabCount > 0)
                for (var i = 0; i < missingPrefabCount; i++)
                {
                    var missingPrefab = Instantiate(resourceRequirementPrefab, resourceRequirementTransform);
                    _activeRequirements.Add(missingPrefab);
                }

            for (var i = 0; i < resourceRequirementsLength; i++)
                _activeRequirements[i].Refresh(_upgradeVariable.resourceRequirements[i]);
        }

        private void RefreshUpgradeButton(bool value)
        {
            upgradeButton.interactable = value;
            upgradeButtonText.color = value ? Color.white : Color.grey;
        }

        private static bool ResourceCheck(IReadOnlyList<Resource> requiredResources)
        {
            var enoughResources = true;

            var requiredResourcesLength = requiredResources.Count;
            for (var i = 0; i < requiredResourcesLength; i++)
            {
                if (PlayerData.GetResourceAmount(requiredResources[i].resourceType) >=
                    requiredResources[i].resourceAmount)
                    continue;

                enoughResources = false;
            }

            return enoughResources;
        }

        private void Upgrade()
        {
            FirebaseFunctionality.Upgrade(_upgradeVariable);
        }
    }
}