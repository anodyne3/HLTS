using Core.UI;
using UnityEngine;
using Utils;

namespace Core.Upgrades
{
    public class AutoRoll : GlobalAccess
    {
        [SerializeField] private int autoRollUpgradeId;

        [SerializeField] private AutoRollItem betMinObject;
        [SerializeField] private AutoRollItem betLessObject;
        [SerializeField] private AutoRollItem autoRollObject;
        [SerializeField] private AutoRollItem betMoreObject;
        [SerializeField] private AutoRollItem betMaxObject;
        
        private void Start()
        {
            autoRollObject.button.OnClick.RemoveAllListeners();
            autoRollObject.button.OnClick.AddListener(ToggleAutoRoll);
            betMinObject.button.OnClick.RemoveAllListeners();
            betMinObject.button.OnClick.AddListener(BetMin);
            betLessObject.button.OnClick.RemoveAllListeners();
            betLessObject.button.OnClick.AddListener(BetLess);
            betMoreObject.button.OnClick.RemoveAllListeners();
            betMoreObject.button.OnClick.AddListener(BetMore);
            betMaxObject.button.OnClick.RemoveAllListeners();
            betMaxObject.button.OnClick.AddListener(BetMax);
            
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshUi);
            
            RefreshUi();
        }

        private void RefreshUi()
        {
            RefreshButtonInteraction();
            autoRollObject.EnableButton(SlotMachine.autoMode);
        }

        private void ToggleAutoRoll()
        {
            if (UpgradeManager.GetUpgradeCurrentLevel(autoRollUpgradeId) > 0)
            {
                EventManager.autoRoll.Raise();
                autoRollObject.light2d.enabled = SlotMachine.autoMode;
            }
            else
            {
                PanelManager.OpenPanelSolo<UpgradePanelController>(autoRollUpgradeId);
            }
        }
        
        private void BetMin()
        {
            if (UpgradeManager.GetUpgradeCurrentLevel(autoRollUpgradeId) > 2)
            {
                SlotMachine.BetMin();
                betMinObject.light2d.enabled = SlotMachine.autoMode;
            }
            else
            {
                PanelManager.OpenPanelSolo<UpgradePanelController>(autoRollUpgradeId);
            }
        }

        private void BetLess()
        {
            if (UpgradeManager.GetUpgradeCurrentLevel(autoRollUpgradeId) > 1)
            {
                SlotMachine.BetLess();
                betLessObject.light2d.enabled = SlotMachine.autoMode;
            }
            else
            {
                PanelManager.OpenPanelSolo<UpgradePanelController>(autoRollUpgradeId);
            }
        }

        private void BetMore()
        {
            if (UpgradeManager.GetUpgradeCurrentLevel(autoRollUpgradeId) > 1)
            {
                SlotMachine.BetMore();
                betMoreObject.light2d.enabled = SlotMachine.autoMode;
            }
            else
            {
                PanelManager.OpenPanelSolo<UpgradePanelController>(autoRollUpgradeId);
            }
        }

        private void BetMax()
        {
            if (UpgradeManager.GetUpgradeCurrentLevel(autoRollUpgradeId) > 2)
            {
                SlotMachine.BetMax();
                betMaxObject.light2d.enabled = SlotMachine.autoMode;
            }
            else
            {
                PanelManager.OpenPanelSolo<UpgradePanelController>(autoRollUpgradeId);
            }
        }

        private void RefreshButtonInteraction()
        {
            var upgradeState = UpgradeManager.GetUpgradeCurrentLevel(autoRollUpgradeId);
            
            if (upgradeState > 0)
                autoRollObject.RepairButton();

            if (upgradeState > 1)
            {
                betMoreObject.RepairButton();
                betLessObject.RepairButton();
            }

            if (upgradeState <= 2) return;
            
            betMaxObject.RepairButton();
            betMinObject.RepairButton();
        }
    }
}