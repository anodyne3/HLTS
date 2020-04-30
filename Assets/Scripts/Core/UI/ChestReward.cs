using System;
using Enums;

namespace Core.UI
{
    [Serializable]
    public class ChestReward
    {
        public int rewardAmount;
        public CurrencyType rewardType;
    }
}