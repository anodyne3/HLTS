using Core.UI;
using Enums;
using UnityEngine;
using Utils;

namespace Core.Upgrades
{
    public class AutoRoll : GlobalAccess
    {
        [SerializeField] private AutoRollItem betMinObject;
        [SerializeField] private AutoRollItem betLessObject;
        [SerializeField] private AutoRollItem autoRollObject;
        [SerializeField] private AutoRollItem betMoreObject;
        [SerializeField] private AutoRollItem betMaxObject;

        private int _upgradeState;

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
            
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshUi, true);
        }

        private void RefreshUi()
        {
            _upgradeState = UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.AutoRoll);
            RefreshButtons();
            RefreshButtonLights();
        }

        private void BetMin()
        {
            if (_upgradeState > 2)
            {
                SlotMachine.BetMin();
            }
            else
            {
                PanelManager.OpenPanelSolo<UpgradePanelController>(UpgradeTypes.AutoRoll);
            }
        }

        private void BetLess()
        {
            if (_upgradeState > 1)
            {
                SlotMachine.BetLess();
            }
            else
            {
                PanelManager.OpenPanelSolo<UpgradePanelController>(UpgradeTypes.AutoRoll);
            }
        }

        private void ToggleAutoRoll()
        {
            if (_upgradeState > 0)
            {
                EventManager.autoRoll.Raise();
            }
            else
            {
                PanelManager.OpenPanelSolo<UpgradePanelController>(UpgradeTypes.AutoRoll);
            }
        }

        private void BetMore()
        {
            if (_upgradeState > 1)
            {
                SlotMachine.BetMore();
            }
            else
            {
                PanelManager.OpenPanelSolo<UpgradePanelController>(UpgradeTypes.AutoRoll);
            }
        }

        private void BetMax()
        {
            if (_upgradeState > 2)
            {
                SlotMachine.BetMax();
            }
            else
            {
                PanelManager.OpenPanelSolo<UpgradePanelController>(UpgradeTypes.AutoRoll);
            }
        }

        private void RefreshButtons()
        {
            if (_upgradeState > 0)
                autoRollObject.RepairButton();

            if (_upgradeState > 1)
            {
                betMoreObject.RepairButton();
                betLessObject.RepairButton();
            }

            if (_upgradeState > 2)
            {
                betMaxObject.RepairButton();
                betMinObject.RepairButton();
            }

            betLessObject.gameObject.SetActive(_upgradeState > 0);
            betMoreObject.gameObject.SetActive(_upgradeState > 0);
            
            betMaxObject.gameObject.SetActive(_upgradeState > 1);
            betMinObject.gameObject.SetActive(_upgradeState > 1);
        }

        private void RefreshButtonLights()
        {
            betMinObject.ButtonLit(SlotMachine.BetAmount > 1);
            betLessObject.ButtonLit(SlotMachine.BetAmount > 1);
            autoRollObject.ButtonLit(SlotMachine.autoMode);
            betMoreObject.ButtonLit(!SlotMachine.CoinSlotFull);
            betMaxObject.ButtonLit(!SlotMachine.CoinSlotFull);
        }
    }
}