using System;
using System.Collections.Generic;
using Core.UI;
using Enums;
using Newtonsoft.Json;

namespace Core.GameData
{
    [Serializable]
    public class ChestRewardDto
    {
        public Resource[] chestRewards = new Resource[3];

        public ChestRewardDto(IReadOnlyList<int> chestPayout)
        {
            var chestRewardsLength = chestRewards.Length;
            for (var i = 0; i < chestRewardsLength; i++)
            {
                chestRewards[i] = new Resource(chestPayout[i], (ResourceType) i);
            }
        }
    }
}