using Core.UI;
using Enums;
using UnityEngine;

namespace Core.GameData
{
    public class ChestRewardDto
    {
        public readonly ChestReward[] chestRewards;

        public ChestRewardDto(string fromJson)
        {
            var jsonParsed = JsonUtility.FromJson<ChestRewardDto>(fromJson);
            chestRewards = jsonParsed.chestRewards;
            
            var chestRewardsLength = chestRewards.Length;
            for (var i = 0; i < chestRewardsLength; i++)
            {
                chestRewards[i].rewardType = (ChestRewardType) i;
            }
        }
    }
}