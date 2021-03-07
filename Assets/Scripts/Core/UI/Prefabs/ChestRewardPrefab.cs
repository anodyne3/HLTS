using Enums;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;

namespace Core.UI.Prefabs
{
    public class ChestRewardPrefab : GlobalAccess
    {
        [SerializeField] private SVGImage rewardIcon;
        [HideInInspector] public ResourceType rewardType;
        public TMP_Text rewardAmount;
        
        public void Init(Resource chestReward)
        {
            rewardType = chestReward.resourceType;
            rewardAmount.text = chestReward.resourceAmount.ToString();
            rewardIcon.sprite = ResourceManager.GetCurrencySprite(chestReward.resourceType);
        }
    }
}