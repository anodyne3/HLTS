﻿using Enums;
using Unity.VectorGraphics;
using UnityEngine;
using Utils;

namespace Core.Upgrades
{
    public class UpgradeIndicatorUi : GlobalAccess
    {
        [SerializeField] private UpgradeTypes upgradeType;

        private SVGImage _indicatorIcon;

        private void Start()
        {
            _indicatorIcon = (SVGImage) GetComponent(typeof(SVGImage));

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshIndicator, true);
        }

        private void RefreshIndicator()
        {
            var upgradeAvailable = !UpgradeManager.IsUpgradeMaxed(upgradeType) &&
                                   UpgradeManager.HasResourcesForUpgrade(upgradeType);

            DisplayIndicator(upgradeAvailable);
        }

        private void DisplayIndicator(bool value)
        {
            _indicatorIcon.enabled = value;
        }
    }
}