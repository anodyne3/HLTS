using System;
using Enums;
using UnityEngine;

namespace Core.UI
{
    [Serializable]
    public class Currency
    {
        [HideInInspector] public long currencyAmount;
        public CurrencyType currencyType;

        public Currency(long amount, CurrencyType type)
        {
            currencyAmount = amount;
            currencyType = type;
        }
    }
}