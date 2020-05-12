using Enums;
using UnityEngine;
using Utils;

namespace Core.Upgrades
{
    public class UpgradeIndicator : GlobalAccess
    {
        [SerializeField] private UpgradeTypes upgradeType;
        
        private SpriteRenderer _indicatorIcon;
    
        private void Start()
        {
            _indicatorIcon = (SpriteRenderer) GetComponent(typeof(SpriteRenderer));
            
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshIndicator, true);
        }

        private void RefreshIndicator()
        {
            if (!isActiveAndEnabled) return;
            
            var upgradeAvailable = UpgradeManager.HasResourcesForUpgrade(upgradeType);
            
            DisplayIndicator(upgradeAvailable);
        }

        private void DisplayIndicator(bool value)
        {
            _indicatorIcon.enabled = value;
        }
    }
}