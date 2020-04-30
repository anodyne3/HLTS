using System;
using Core.UI;
using Enums;
using Newtonsoft.Json;

namespace Core.GameData
{
    [Serializable]
    public class ChestRewardDto
    {
        public readonly Currency[] chestRewards = new Currency[3];

        public ChestRewardDto(string fromJson)
        {
            var jsonParsed = JsonConvert.DeserializeObject<long[]>(fromJson);
            // var jsonParsed = JsonConvert.DeserializeObject<int[]>(fromJson);

            var chestRewardsLength = chestRewards.Length;
            for (var i = 0; i < chestRewardsLength; i++)
            {
                chestRewards[i] = new Currency(jsonParsed[i], (CurrencyType) i);
                // chestRewards[i] = new Currency {currencyType = (CurrencyType) i, currencyAmount = jsonParsed[i]};
            }
        }
    }
}