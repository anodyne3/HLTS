using System.Collections.Generic;
using Core.UI.Prefabs;
using Enums;
using MyScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.UI
{
    public class UpgradePanelController : PanelController
    {
        [SerializeField] private Button upgradeButton;
        [SerializeField] private TMP_Text upgradeButtonText;
        [SerializeField] private Button cancelButton;
        [SerializeField] private SVGImage targetIcon;
        [SerializeField] private TMP_Text upgradeNameText;
        [SerializeField] private TMP_Text descriptionText;
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

            _upgradeVariable = UpgradeManager.GetUpgradeVariable((UpgradeTypes) args[0]);

            RefreshPanel();
        }

        private void RefreshPanel()
        {
            upgradeNameText.text = _upgradeVariable.CurrentUpgradeName;
            targetIcon.sprite = _upgradeVariable.CurrentIcon;
            descriptionText.text = _upgradeVariable.CurrentDescription;

            RefreshUi();
        }

        private void RefreshUi()
        {
            RefreshResourceRequirements();
            RefreshUpgradeButton(UpgradeManager.HasResourcesForUpgrade(_upgradeVariable.upgradeType));
        }

        private void RefreshResourceRequirements()
        {
            if (UpgradeManager.IsUpgradeMaxed(_upgradeVariable.upgradeType)) return;
            
            var resourceRequirementsLength = _upgradeVariable.CurrentResourceRequirements.Length;
            var missingPrefabCount = resourceRequirementsLength - _activeRequirements.Count;

            if (missingPrefabCount > 0)
                for (var i = 0; i < missingPrefabCount; i++)
                {
                    var missingPrefab = Instantiate(resourceRequirementPrefab, resourceRequirementTransform);
                    _activeRequirements.Add(missingPrefab);
                }

            for (var i = 0; i < resourceRequirementsLength; i++)
                _activeRequirements[i].Refresh(_upgradeVariable.CurrentResourceRequirements[i]);
        }

        private void RefreshUpgradeButton(bool value)
        {
            upgradeButton.interactable = value;
            upgradeButtonText.color = value ? Color.white : Color.grey;
        }

        private void Upgrade()
        {
            FirebaseFunctionality.Upgrade(_upgradeVariable.upgradeType);
        }

        public void UpgradeComplete(long productId)
        {
            base.ClosePanel();

            var alertMessage =
                Constants.GetUpgradeTypeName(UpgradeManager.GetUpgradeVariable((UpgradeTypes) productId).upgradeType) +
                Constants.UpgradeCompletedSuffix;

            AlertMessage.Init(alertMessage);
            UpgradeManager.RefreshUpgrades();
        }
    }
}