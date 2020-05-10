using System;
using System.Collections.Generic;
using Core.UI;
using Enums;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.GameData
{
    [Serializable]
    public class ChestRewardDto
    {
        public Resource[] chestRewards = new Resource[3];

        public ChestRewardDto(object responseData)
        {
            var processedData = (Dictionary<object, object>) responseData;

            if (!processedData.ContainsKey("text")) return;

            var chestRewardList = (List<object>) processedData["text"];

            var chestRewardsLength = chestRewards.Length;
            for (var i = 0; i < chestRewardsLength; i++)
                chestRewards[i] = new Resource((long) chestRewardList[i], (ResourceType) i);
        }
    }
}