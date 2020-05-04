using MyScriptableObjects;
using UnityEngine;

namespace Core.UpgradeRepairs
{
    public partial class AutoRoll : GlobalAccess
    {
        [SerializeField] private UpgradeRepairVariable autoRollUpgrades;

        [SerializeField] private AutoRollItem betMinObject;
        [SerializeField] private AutoRollItem betLessObject;
        [SerializeField] private AutoRollItem autoRollObject;
        [SerializeField] private AutoRollItem betMoreObject;
        [SerializeField] private AutoRollItem betMaxObject;

        private void Start()
        {
            betMinObject.button.onClick.AddListener(BetMin);
            betLessObject.button.onClick.AddListener(BetLess);
            autoRollObject.button.onClick.AddListener(ToggleAutoRoll);
            betMoreObject.button.onClick.AddListener(BetMore);
            betMaxObject.button.onClick.AddListener(BetMax);

            RefreshButtonInteraction();
        }

        private void BetMin()
        {
            if (SlotMachine.betAmount > 1)
                SlotMachine.betAmount = 1;
        }

        private void BetLess()
        {
            if (SlotMachine.betAmount > 1)
                SlotMachine.betAmount--;
        }

        private static void ToggleAutoRoll()
        {
            EventManager.autoSlotMode.Raise();
        }

        private void BetMore()
        {
            if (SlotMachine.betAmount < SlotMachine.coinSlotMaxBet)
                SlotMachine.betAmount++;
        }

        private void BetMax()
        {
            if (SlotMachine.betAmount < SlotMachine.coinSlotMaxBet)
                SlotMachine.betAmount = SlotMachine.coinSlotMaxBet;
        }

        private void RefreshButtonInteraction()
        {
            var upgradeState = UpgradeRepairManager.GetUpgradeRepairState(autoRollUpgrades.id);

            if (upgradeState < 1) return;

            autoRollObject.button.interactable = true;

            if (upgradeState < 2) return;
            betMoreObject.EnableButton(SlotMachine.betAmount < SlotMachine.coinSlotMaxBet);
            betLessObject.EnableButton(SlotMachine.betAmount > 0);

            if (upgradeState < 3) return;
            
            betMaxObject.EnableButton(SlotMachine.betAmount < SlotMachine.coinSlotMaxBet);
            betMinObject.EnableButton(SlotMachine.betAmount > 0);
        }
    }
}