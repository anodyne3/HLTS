using DG.Tweening;
using MyScriptableObjects;
using UnityEngine;
using Utils;

namespace Core.MainSlotMachine
{
    public class LastBetIndicator : GlobalClass
    {
        [SerializeField] private Transform[] lastBetIndicators;
        [SerializeField] private TweenSetting lastBetIndicatorTweenSettings;

        public void RefreshLastBetIndicators()
        {
            var lastBetIndicatorSequence = DOTween.Sequence();

            var lastBetIndicatorsLength = lastBetIndicators.Length;
            var newIndicatorPositions = new Vector3[lastBetIndicatorsLength];

            for (var i = 0; i < lastBetIndicatorsLength; i++)
            {
                var isActive = SlotMachine.lastBetAmount > i;

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
                        lastBetIndicators[i].transform.DOLocalMoveY(newIndicatorPositions[i].x,
                            lastBetIndicatorTweenSettings.moveDuration))
                    .Insert(0.0f,
                        lastBetIndicators[i].transform.DOLocalRotate(new Vector3(0.0f, 0.0f, newIndicatorPositions[i].z),
                            lastBetIndicatorTweenSettings.moveDuration))
                    .Insert(0.0f,
                        lastBetIndicators[i].transform.DOScaleX(newIndicatorPositions[i].y,
                            lastBetIndicatorTweenSettings.moveDuration));
            }
            
            EventManager.refreshUi.Raise();
        }
    }
}