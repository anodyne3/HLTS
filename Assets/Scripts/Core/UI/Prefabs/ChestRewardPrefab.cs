using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Prefabs
{
    public class ChestRewardPrefab : GlobalAccess
    {
        [SerializeField] private TMP_Text rewardAmount;
        [SerializeField] private Image rewardIcon;

        private ChestReward _chestReward;
        
        public void Init(ChestReward chestReward)
        {
            _chestReward = chestReward;
            
            rewardAmount.text = _chestReward.rewardAmount.ToString();
            rewardIcon.sprite = ResourceManager.GetRewardSprite(_chestReward.rewardType);
        }
    }
}