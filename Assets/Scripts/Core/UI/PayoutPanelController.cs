using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.UI
{
    public class PayoutPanelController : PanelController
    {
        [SerializeField] private TMP_Text payoutMessageText;
        [SerializeField] private TMP_Text payoutAmountText;
        [SerializeField] private TMP_Text payoutTypeText;
        [SerializeField] private Button doublePayoutForAdButton;

        private long _payoutAmount;

        public override void Start()
        {
            base.Start();
            
            // doublePayoutForAdButton.onClick.RemoveAllListeners();
            // doublePayoutForAdButton.onClick.AddListener(AdManager.ShowRewardedAd);
            
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.userEarnedRewardEvent, RewardEarned);
        }

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

            _payoutAmount = PlayerData.coinsAmount - HudController.CoinsAmount;

            payoutAmountText.text = _payoutAmount.ToString();

            payoutTypeText.text = SlotMachine.payout.ToString();

            StartTextAnimations();
        }

        private void RewardEarned()
        {
            PlayerData.coinsAmount += _payoutAmount;
            
            RefreshPanel();
        }

        protected override void ClosePanel()
        {
            base.ClosePanel();

            EventManager.payoutFinish.Raise();
            EventManager.refreshUi.Raise();
        }
    }
}