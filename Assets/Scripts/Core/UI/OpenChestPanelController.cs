using Core.GameData;
using Core.UI.Prefabs;
using Core.Managers;
using DG.Tweening;
using Enums;
using UnityEngine;

namespace Core.UI
{
    public class OpenChestPanelController : PanelController
    {
        [SerializeField] private Transform rewardStartPosition; 
        [SerializeField] private Transform rewardDisplayPosition;
        [SerializeField] private Transform rewardFinishPosition;
        [SerializeField] private ChestRewardPrefab chestRewardPrefab;
        
        private MyObjectPool<ChestRewardPrefab> _chestRewardPool;
        private ChestRewardDto _chestRewardDto;
        private Sequence _rewardSequence;
        
        //public Button doublePayoutForAdButton;

        public override void Start()
        {
            base.Start();
            
            backgroundButton.onClick.RemoveAllListeners();
            // doublePayoutForAdButton.onClick.RemoveAllListeners();
            // doublePayoutForAdButton.onClick.AddListener(ConfirmShowAd);

            _chestRewardPool = ObjectPoolManager.CreateObjectPool<ChestRewardPrefab>(chestRewardPrefab, transform);
        }

        public override void OpenPanel(params object[] args)
        {
            base.OpenPanel();

            // doublePayoutForAdButton.gameObject.SetActive(AdManager.DoublePayoutAdIsLoaded());

            _chestRewardDto = (ChestRewardDto) args[0];
             
            PanelManager.GetPanel<ChestDetailsPanelController>().OpenChestIcon(true);

            RefreshPanel();
        }

        private void RefreshPanel()
        {
            var rewardsLength = _chestRewardDto.chestRewards.Length;
            for (var i = 0; i < rewardsLength; i++)
            {
                var chestReward = _chestRewardPool.Get();
                chestReward.Init(_chestRewardDto.chestRewards[i]);
            }

            StartSequence();
        }

        private void StartSequence()
        {
            _rewardSequence = DOTween.Sequence();
            _rewardSequence.SetAutoKill(false);
            _rewardSequence.SetRecyclable(true);
        }

        /*private static void ConfirmShowAd()
        {
            PanelManager.OpenSubPanel<ConfirmRewardAdPanelController>();
        }*/

        private void ProcessReward()
        {
            // doublePayoutForAdButton.gameObject.SetActive(false);
            PlayerData.bcAmount += _chestRewardDto.chestRewards[(int)ChestRewardType.BananaCoins].rewardAmount;
            PlayerData.bpAmount += _chestRewardDto.chestRewards[(int)ChestRewardType.BluePrints].rewardAmount;
            PlayerData.sfAmount += _chestRewardDto.chestRewards[(int)ChestRewardType.StarFruits].rewardAmount;
            
            RefreshPanel();

            AdManager.reward = null;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus || AdManager.reward == null) return;
            
            ProcessReward();
        }

        protected override void ClosePanel()
        {
            base.ClosePanel();

            EventManager.payoutFinish.Raise();
            EventManager.refreshUi.Raise();
        }
    }
}