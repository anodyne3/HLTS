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

        private readonly Resource _payoutCurrency = new Resource(0, ResourceType.BananaCoins);

        public override void Start()
        {
            base.Start();

            backgroundButton.onClick.RemoveAllListeners();
            doublePayoutForAdButton.onClick.RemoveAllListeners();
            doublePayoutForAdButton.onClick.AddListener(ConfirmShowAd);
        }

        private static void ConfirmShowAd()
        {
            PanelManager.OpenSubPanel<ConfirmRewardAdPanelController>();
        }

        public override void OpenPanel(params object[] args)
        {
            base.OpenPanel();

            // doublePayoutForAdButton.gameObject.SetActive(AdManager.DoublePayoutAdIsLoaded());
            CurrencyManager.HideCurrencies(true);

            RefreshPanel();
        }

        private void RefreshPanel()
        {
            payoutMessageText.text = SlotMachine.payout == FruitType.Bars ||
                                     SlotMachine.payout == FruitType.Bananas ||
                                     SlotMachine.payout == FruitType.Barnana
                ? Constants.JackpotMessage
                : Constants.YouWinMessage;


            _payoutCurrency.resourceAmount = PlayerData.GetResourceAmount(_payoutCurrency.resourceType) -
                                             CurrencyManager.GetCurrencyByType(_payoutCurrency.resourceType)
                                                 .currencyDetails.resourceAmount;

            payoutAmountText.text = _payoutCurrency.resourceAmount.ToString();

            payoutTypeText.text = SlotMachine.payout.ToString();

            StartTextAnimations();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus || AdManager.reward == null) return;

            ProcessReward();
        }

        private void ProcessReward()
        {
            PlayerData.AddResourceAmount(_payoutCurrency);
            doublePayoutForAdButton.gameObject.SetActive(false);

            RefreshPanel();

            AdManager.reward = null;
        }

        protected override void ClosePanel()
        {
            base.ClosePanel();
            
            CurrencyManager.HideCurrencies(false);
            
            EventManager.payoutFinish.Raise();
            EventManager.refreshCurrency.Raise();
        }
    }
}