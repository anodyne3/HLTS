using Enums;
using TMPro;
using UnityEngine;
using Utils;

namespace Core.UI
{
    public class PayoutPanelController : PanelController
    {
        [SerializeField] private TMP_Text payoutWords;
        [SerializeField] private TMP_Text payoutMessage;

        public override void OpenPanel()
        {
            base.OpenPanel();

            RefreshPanel();
        }

        private void RefreshPanel()
        {
            payoutMessage.text = SlotMachine.payout == FruitType.Bar || SlotMachine.payout == FruitType.Banana ||
                                 SlotMachine.payout == FruitType.Barnana
                ? Constants.JackpotMessage
                : Constants.YouWinMessage;

            payoutWords.text = SlotMachine.payout.ToString();

            StartTextAnimations();
        }

        protected override void ClosePanel()
        {
            base.ClosePanel();

            EventManager.payoutFinish.Raise();
        }
    }
}