using Core.Upgrades;
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

        public void RefreshBetIndicators()
        {
            var betIndicatorSequence = DOTween.Sequence();

            var betIndicatorsLength = betIndicators.Length;
            var newIndicatorPositions = new Vector3[betIndicatorsLength];

            for (var i = 0; i < betIndicatorsLength; i++)
            {
                var isActive = SlotMachine.BetAmount > i;

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
                        betIndicators[i].transform.DOLocalMoveY(newIndicatorPositions[i].x,
                            betIndicatorTweenSettings.moveDuration))
                    .Insert(0.0f,
                        betIndicators[i].transform.DOLocalRotate(new Vector3(0.0f, 0.0f, newIndicatorPositions[i].z),
                            betIndicatorTweenSettings.moveDuration))
                    .Insert(0.0f,
                        betIndicators[i].transform.DOScaleX(newIndicatorPositions[i].y,
                            betIndicatorTweenSettings.moveDuration));
            }
            
            EventManager.refreshUi.Raise();
        }
    }
}