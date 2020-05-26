using System;
using System.Linq;
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

            doublePayoutForAdButton.gameObject.SetActive(AdManager.DoublePayoutAdIsLoaded());
            CurrencyManager.HideCurrencies(true);

            RefreshPanel();
        }

        private void RefreshPanel()
        {
            payoutMessageText.text = SlotMachine.payoutType.Any(x => x == FruitType.Bars) ||
                                     SlotMachine.payoutType.Any(x => x == FruitType.Bananas) ||
                                     SlotMachine.payoutType.Any(x => x == FruitType.Barnana)
                ? Constants.JackpotMessage
                : Constants.YouWinMessage;

            payoutTypeText.text = string.Empty;
            var payoutTypeCount = SlotMachine.payoutType.Count;
            for (var i = 0; i < payoutTypeCount; i++)
            {
                payoutTypeText.text += SlotMachine.payoutType[i].ToString();

                if (payoutTypeCount > 1)
                    payoutTypeText.text += "<br>";
            }

            _payoutCurrency.resourceAmount = SlotMachine.payoutAmount;

            payoutAmountText.text = _payoutCurrency.resourceAmount.ToString();

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