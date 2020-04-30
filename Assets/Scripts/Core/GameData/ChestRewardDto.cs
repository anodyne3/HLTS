using System;
using Core.UI;
using Enums;
using Newtonsoft.Json;

namespace Core.GameData
{
    [Serializable]
    public class ChestRewardDto
    {
        public /*readonly*/ ChestReward[] chestRewards = new ChestReward[3];

        public ChestRewardDto(string fromJson)
        {
            var jsonParsed = JsonConvert.DeserializeObject<int[]>(fromJson);

            var chestRewardsLength = chestRewards.Length;
            for (var i = 0; i < chestRewardsLength; i++)
            {
                chestRewards[i] = new ChestReward {rewardType = (CurrencyType) i, rewardAmount = jsonParsed[i]};
            }
        }
    }
}