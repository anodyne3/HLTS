using System;
using Core.UI;
using Enums;
using Newtonsoft.Json;

namespace Core.GameData
{
    [Serializable]
    public class ChestRewardDto
    {
        public Resource[] chestRewards = new Resource[3];

        public ChestRewardDto(string fromJson)
        {
            var jsonParsed = JsonConvert.DeserializeObject<long[]>(fromJson);

            var chestRewardsLength = chestRewards.Length;
            for (var i = 0; i < chestRewardsLength; i++)
            {
                chestRewards[i] = new Resource(jsonParsed[i], (ResourceType) i);
            }
        }
    }
}