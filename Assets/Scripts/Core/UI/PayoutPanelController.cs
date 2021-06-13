using System.Linq;
using Core.Managers;
using DG.Tweening;
using Enums;
using MyScriptableObjects;
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
        [SerializeField] private TweenPunchSetting punchSetting; 

        private readonly Resource _payoutCurrency = new Resource(0, ResourceType.BananaCoins);
        private const AdType ThisAdType = AdType.DoublePayout;

        public override void Start()
        {
            base.Start();

            backgroundButton.onClick.RemoveAllListeners();
            doublePayoutForAdButton.onClick.RemoveAllListeners();
            doublePayoutForAdButton.onClick.AddListener(ConfirmShowAd);
        }

        private static void ConfirmShowAd()
        {
            PanelManager.OpenSubPanel<ConfirmRewardAdPanelController>(ThisAdType);
        }

        public override void OpenPanel(params object[] args)
        {
            base.OpenPanel();

            doublePayoutForAdButton.gameObject.SetActive(AdManager.AdIsLoaded(ThisAdType));
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
            punchSetting.DoPunch(payoutAmountText.transform);

            StartTextAnimations();
        }

        public void ProcessAdReward()
        {
            doublePayoutForAdButton.gameObject.SetActive(false);
            
            payoutAmountText.text = (_payoutCurrency.resourceAmount + _payoutCurrency.resourceAmount).ToString();
            payoutAmountText.transform.DOPunchScale(new Vector3(1.1f, 1.1f,1.1f), 0.666f);
        }

        protected override void ClosePanel()
        {
            base.ClosePanel();

            CurrencyManager.HideCurrencies(false);
            CurrencyManager.blockCurrencyRefresh = false;

            EventManager.payoutFinish.Raise();
            EventManager.refreshCurrency.Raise();
        }
    }
}