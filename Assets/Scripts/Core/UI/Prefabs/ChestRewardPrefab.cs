using Enums;
using TMPro;
using UnityEngine;

namespace Core.UI.Prefabs
{
    public class ChestRewardPrefab : GlobalAccess
    {
        [SerializeField] private TMP_Text rewardAmount;
        [SerializeField] private SVGImage rewardIcon;

        [HideInInspector] public CurrencyType rewardType;
        
        public void Init(Currency chestReward)
        {
            rewardType = chestReward.currencyType;
            rewardAmount.text = chestReward.currencyAmount.ToString();
            rewardIcon.sprite = ResourceManager.GetCurrencySprite(chestReward.currencyType);
        }
    }
}