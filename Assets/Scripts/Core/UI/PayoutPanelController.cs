using Enums;
using TMPro;
using UnityEngine;
using Utils;

namespace Core.UI
{
    public class PayoutPanelController : PanelController
    {
        [SerializeField] private TMP_Text payoutMessageText;
        [SerializeField] private TMP_Text payoutAmountText;
        [SerializeField] private TMP_Text payoutTypeText;

        public override void OpenPanel()
        {
            base.OpenPanel();

            RefreshPanel();
        }

        private void RefreshPanel()
        {
            payoutMessageText.text = SlotMachine.payout == FruitType.Bars || 
                                     SlotMachine.payout == FruitType.Bananas ||
                                     SlotMachine.payout == FruitType.Barnana
                ? Constants.JackpotMessage
                : Constants.YouWinMessage;

            payoutAmountText.text = (PlayerData.coinsAmount - HudController.CoinsAmount).ToString();

            payoutTypeText.text = SlotMachine.payout.ToString();

            StartTextAnimations();
        }

        protected override void ClosePanel()
        {
            base.ClosePanel();

            EventManager.payoutFinish.Raise();
            EventManager.refreshUi.Raise();
        }
    }
}