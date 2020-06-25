using DG.Tweening;
using MyScriptableObjects;
using UnityEngine;
using Utils;

namespace Core.MainSlotMachine
{
    public class BetIndicator : GlobalClass
    {
        [SerializeField] private Transform[] betIndicators;
        [SerializeField] private TweenSetting betIndicatorTweenSettings;
        [SerializeField] private Transform[] lastBetIndicators;
        [SerializeField] private TweenSetting lastBetIndicatorTweenSettings;

        private void Start()
        {
            if (SlotMachine == null) return;
            
            RefreshBetIndicators();
            RefreshLastBetIndicators();
            
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.payoutFinishEvent, StopAllFlashing);
        }

        public void RefreshBetIndicators()
        {
            var betIndicatorSequence = DOTween.Sequence();

            var betIndicatorsLength = betIndicators.Length;
            var newIndicatorPositions = new Vector3[betIndicatorsLength];

            for (var i = 0; i < betIndicatorsLength; i++)
            {
                var isActive = SlotMachine.BetAmount > i;
                betIndicators[i].gameObject.SetActive(isActive);
                
                switch (i)
                {
                    case 0:
                        newIndicatorPositions[i].Set(0.0f, 1.0f, 0.0f);
                        break;
                    case 1:
                        newIndicatorPositions[i].Set(isActive ? Constants.OffsetAmountLow : 0.0f, 1.0f, 0.0f);
                        break;
                    case 2:
                        newIndicatorPositions[i].Set(isActive ? -Constants.OffsetAmountLow : 0.0f, 1.0f, 0.0f);
                        break;
                    case 3:
                        newIndicatorPositions[i].Set(0.0f, isActive ? Constants.ScaleAmount : 1.0f,
                            isActive ? -Constants.RotationAmount : 0.0f);
                        break;
                    case 4:
                        newIndicatorPositions[i].Set(0.0f, isActive ? Constants.ScaleAmount : 1.0f,
                            isActive ? Constants.RotationAmount : 0.0f);
                        break;
                }
            }

            for (var i = 0; i < betIndicatorsLength; i++)
            {
                betIndicatorSequence
                    .SetEase(betIndicatorTweenSettings.sequenceEasing)
                    .Insert(0.0f,
                        betIndicators[i].DOLocalMoveY(newIndicatorPositions[i].x,
                            betIndicatorTweenSettings.moveDuration))
                    .Insert(0.0f,
                        betIndicators[i].DOLocalRotate(new Vector3(0.0f, 0.0f, newIndicatorPositions[i].z),
                            betIndicatorTweenSettings.moveDuration))
                    .Insert(0.0f,
                        betIndicators[i].DOScaleX(newIndicatorPositions[i].y,
                            betIndicatorTweenSettings.moveDuration));
            }
            
            EventManager.refreshUi.Raise();
        }

        public void RefreshLastBetIndicators()
        {
            var lastBetIndicatorSequence = DOTween.Sequence();

            var lastBetIndicatorsLength = lastBetIndicators.Length;
            var newIndicatorPositions = new Vector3[lastBetIndicatorsLength];

            for (var i = 0; i < lastBetIndicatorsLength; i++)
            {
                var isActive = SlotMachine.lastBetAmount > i && SlotMachine.lastBetAmount != 0;

                switch (i)
                {
                    case 0:
                        newIndicatorPositions[i].Set(0.0f, 1.0f, 0.0f);
                        break;
                    case 1:
                        newIndicatorPositions[i].Set(isActive ? Constants.OffsetAmountLow : 0.0f, 1.0f, 0.0f);
                        break;
                    case 2:
                        newIndicatorPositions[i].Set(isActive ? -Constants.OffsetAmountLow : 0.0f, 1.0f, 0.0f);
                        break;
                    case 3:
                        newIndicatorPositions[i].Set(0.0f, isActive ? Constants.ScaleAmount : 1.0f,
                            isActive ? -Constants.RotationAmount : 0.0f);
                        break;
                    case 4:
                        newIndicatorPositions[i].Set(0.0f, isActive ? Constants.ScaleAmount : 1.0f,
                            isActive ? Constants.RotationAmount : 0.0f);
                        break;
                }
            }
            
            for (var i = 0; i < lastBetIndicatorsLength; i++)
            {
                lastBetIndicatorSequence
                    .SetEase(lastBetIndicatorTweenSettings.sequenceEasing)
                    .Insert(0.0f,
                        lastBetIndicators[i].DOLocalMoveY(newIndicatorPositions[i].x,
                            lastBetIndicatorTweenSettings.moveDuration))
                    .Insert(0.0f,
                        lastBetIndicators[i].DOLocalRotate(
                            new Vector3(0.0f, 0.0f, newIndicatorPositions[i].z),
                            lastBetIndicatorTweenSettings.moveDuration))
                    .Insert(0.0f,
                        lastBetIndicators[i].DOScaleX(newIndicatorPositions[i].y,
                            lastBetIndicatorTweenSettings.moveDuration));
            }
            
            // EventManager.refreshUi.Raise();
        }

        public void FlashLastBetIndicators(int i)
        {
            // betIndicators[i].
        }

        private void StopAllFlashing()
        {
            
        }
    }
}