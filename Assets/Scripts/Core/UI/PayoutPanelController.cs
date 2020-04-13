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
        public Button doublePayoutForAdButton;

        private long _payoutAmount;

        public override void Start()
        {
            base.Start();
            
            backgroundButton.onClick.RemoveAllListeners();
            doublePayoutForAdButton.onClick.RemoveAllListeners();
            doublePayoutForAdButton.onClick.AddListener(ConfirmShowAd);
        }

        public override void OpenPanel()
        {
            base.OpenPanel();

            doublePayoutForAdButton.gameObject.SetActive(AdManager.DoublePayoutAdIsLoaded());

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

        private static void ConfirmShowAd()
        {
            PanelManager.OpenSubPanel<ConfirmRewardAdPanelController>();
        }

        private void ProcessReward()
        {
            PlayerData.coinsAmount += _payoutAmount;
            doublePayoutForAdButton.gameObject.SetActive(false);
            
            RefreshPanel();

            AdManager.reward = null;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus || AdManager.reward == null) return;
            
            ProcessReward();
        }

        protected override void ClosePanel()
        {
            base.ClosePanel();

            EventManager.payoutFinish.Raise();
            EventManager.refreshUi.Raise();
        }
    }
}