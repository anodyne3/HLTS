using System.Collections.Generic;
using Core.GameData;
using Core.UI.Prefabs;
using DG.Tweening;
using Enums;
using MyScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class OpenChestPanelController : PanelController
    {
        [SerializeField] private Transform rewardDisplayPosition;
        [SerializeField] private Transform rewardFinishPosition;
        [SerializeField] private TweenSetting tweenStartSetting;
        [SerializeField] private TweenSetting tweenFinishSetting;
        
        public ChestRewardPrefab chestRewardPrefab;
        public Transform rewardStartPosition;
        
        [HideInInspector] public MyObjectPool<ChestRewardPrefab> chestRewardPool;
        
        private ChestRewardDto _chestRewardDto;
        private Sequence _rewardSequence;
        private readonly List<ChestRewardPrefab> _activeRewards = new List<ChestRewardPrefab>();

        public override void Start()
        {
            base.Start();

            backgroundButton.onClick.RemoveAllListeners();
            backgroundButton.onClick.AddListener(RushRewards);
        }

        public override void OpenPanel(params object[] args)
        {
            base.OpenPanel();

            _chestRewardDto = (ChestRewardDto) args[0];

            PanelManager.GetPanel<ChestDetailsPanelController>().OpenChestIcon(true);

            RefreshPanel();
        }

        private void RefreshPanel()
        {
            _rewardSequence = DOTween.Sequence();
            gameObject.SetActive(true);

            var rewardsLength = _chestRewardDto.chestRewards.Length;
            for (var i = 0; i < rewardsLength; i++)
            {
                if (_chestRewardDto.chestRewards[i].resourceAmount <= 0)
                {
                    rewardFinishPosition.GetChild(i).gameObject.SetActive(false);
                    continue;
                }

                var chestReward = chestRewardPool.Get();
                chestReward.Init(_chestRewardDto.chestRewards[i]);
                var chestRewardTransform = chestReward.transform;
                chestRewardTransform.position = rewardStartPosition.position;
                chestRewardTransform.rotation = rewardStartPosition.rotation;
                chestRewardTransform.localScale = tweenStartSetting.scaleStartValue;
                chestReward.gameObject.SetActive(true);
                rewardFinishPosition.GetChild(i).gameObject.SetActive(true);
                _activeRewards.Add(chestReward);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) rewardFinishPosition);

            var activeRewardsCount = _activeRewards.Count;
            for (var i = 0; i < activeRewardsCount; i++)
            {
                var totalStartDuration = tweenStartSetting.moveDuration + tweenStartSetting.delayBetweenInstance;
                var startTimePosition =
                    PanelManager.openPanelFadeSettings.fadeDuration + totalStartDuration * i;
                var finishTimePosition = startTimePosition + totalStartDuration;
                var i1 = i;
                var i2 = i;
                var finishChildId = (int) _activeRewards[i].rewardType;

                var rewardStartSequence = DOTween.Sequence()
                        .Insert(startTimePosition, _activeRewards[i].transform
                            .DOMove(rewardDisplayPosition.position, tweenStartSetting.moveDuration)
                            .SetEase(tweenStartSetting.sequenceEasing))
                        .Insert(startTimePosition, _activeRewards[i].transform
                            .DOScale(tweenStartSetting.scaleEndValue, tweenStartSetting.scaleDuration)
                            .SetEase(tweenStartSetting.sequenceEasing))
                        .InsertCallback(startTimePosition,
                            () => tweenStartSetting.DoRotate(_activeRewards[i1].transform.GetChild(0), false))
                        .Insert(finishTimePosition, _activeRewards[i].transform
                            .DOMove(rewardFinishPosition.GetChild(finishChildId).position, tweenFinishSetting.moveDuration)
                            .SetEase(tweenFinishSetting.sequenceEasing))
                        .Insert(finishTimePosition, _activeRewards[i].transform
                            .DOScale(tweenFinishSetting.scaleEndValue, tweenFinishSetting.scaleDuration)
                            .SetEase(tweenFinishSetting.sequenceEasing))
                        .InsertCallback(finishTimePosition,
                            () => tweenFinishSetting.DoRotate(_activeRewards[i2].transform.GetChild(0), false))
                    ;

                _rewardSequence.Insert(0.0f, rewardStartSequence);
            }
        }

        private void RushRewards()
        {
            if (!_rewardSequence.IsComplete())
                _rewardSequence.Complete(true);

            ClosePanel();

            foreach (var rewardPrefab in _activeRewards)
            {
                chestRewardPool.Release(rewardPrefab);
                rewardPrefab.gameObject.SetActive(false);
            }

            _activeRewards.Clear();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus || AdManager.reward == null) return;

            ProcessAdReward();
        }

        private void ProcessReward()
        {
            var chestRewardsLength = _chestRewardDto.chestRewards.Length;
            for (var i = 0; i < chestRewardsLength; i++)
                PlayerData.AddResourceAmount(_chestRewardDto.chestRewards[i]);
        }

        private void ProcessAdReward()
        {
            var doubledReward = new Resource(0, ResourceType.StarFruits); 
            
            var chestRewardsLength = _chestRewardDto.chestRewards.Length;
            for (var i = 0; i < chestRewardsLength; i++)
            {
                if (_chestRewardDto.chestRewards[i].resourceType != ResourceType.StarFruits) continue;
                
                doubledReward = _chestRewardDto.chestRewards[i];
                PlayerData.AddResourceAmount(_chestRewardDto.chestRewards[i]);
            }

            doubledReward.resourceAmount += doubledReward.resourceAmount; 
            
            var activeRewardsCount = _activeRewards.Count;
            for (var i = 0; i < activeRewardsCount; i++)
            {
                if (_activeRewards[i].rewardType == ResourceType.StarFruits)
                    _activeRewards[i].rewardAmount.text = doubledReward.resourceAmount.ToString();
            }

            AdManager.reward = null;
        }
        

        protected override void ClosePanel()
        {
            base.ClosePanel();
            
            ProcessReward();

            EventManager.payoutFinish.Raise();
            EventManager.refreshUi.Raise();
        }
    }
}