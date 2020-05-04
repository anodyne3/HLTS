using System;
using Core.UI.Prefabs;
using MyScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    [Serializable]
    public class Currency
    {
        public Image currencyIcon;
        public TMP_Text currencyAmountText;
        public TweenPunchSetting addCurrencyPunchTween;
        public Resource currencyDetails;
        public DeductCurrencyPrefab deductCurrencyPrefab;
        [HideInInspector] public long currencyDifference;
        [HideInInspector] public bool updateCurrency;

        public void UpdateTextDisplay()
        {
            currencyAmountText.text = currencyDetails.resourceAmount.ToString();
        }

        public void DoPunch()
        {
            addCurrencyPunchTween.DoPunch(currencyIcon.transform);
        }
    }
}