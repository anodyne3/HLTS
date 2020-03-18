using Enums;
using TMPro;
using UnityEngine;
using Utils;

namespace Core.UI
{
    public class PayoutPanelController : PanelController
    {
        private TMP_Text _payoutWords;
        private TMP_Text _payoutMessage;
        private Animator _payoutAnimator; 

        private void Start()
        {
            _payoutAnimator = (Animator) GetComponent(typeof(Animator));
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.payoutStartEvent, OpenPanel);
        }

        public override void OpenPanel()
        {
            base.OpenPanel();

            RefreshPanel();
        }

        private void RefreshPanel()
        {
            _payoutMessage.text = SlotMachine.payout == FruitType.Bar || SlotMachine.payout == FruitType.Banana ||
                                  SlotMachine.payout == FruitType.Barnana
                ? Constants.JackpotMessage
                : Constants.YouWinMessage;

            _payoutWords.text = SlotMachine.payout.ToString();

            _payoutAnimator.Play(Constants.PayoutWordsState);
        }

        protected override void ClosePanel()
        {
            base.ClosePanel();

            EventManager.payoutFinish.Raise();
        }
    }
}