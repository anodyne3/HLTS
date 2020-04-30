using Enums;

namespace Core.UI
{
    public class Currency
    {
        public long currencyAmount;
        public CurrencyType currencyType;

        public Currency(long amount, CurrencyType type)
        {
            currencyAmount = amount;
            currencyType = type;
        }
    }
}