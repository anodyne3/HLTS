using DG.Tweening;
using Enums;
using MyScriptableObjects;
using UnityEngine;

namespace Core.UI.Prefabs
{
    public class ChestAddedPrefab : GlobalAccess
    {
        [SerializeField] private SVGImage chestIcon;
        [SerializeField] private TweenSetting tweenSettings;
        
        private Sequence _addSequence;

        public void Init(ChestType id)
        {
            chestIcon.sprite = ChestManager.GetChestIcon(id);

            var startPosition = transform.localPosition;
            var startMoveOffset = startPosition;
            startMoveOffset.y += tweenSettings.moveOffset;

            if (_addSequence != null)
            {
                _addSequence.Kill();
                // _addSequence.Restart();
                // return;
            }
            
            _addSequence = DOTween.Sequence();
            _addSequence
                // .SetAutoKill(false)
                // .SetRecyclable(true)
                .SetEase(tweenSettings.moveCurve)
                //start
                .Insert(0.0f,
                    transform.DOLocalMove(startMoveOffset, tweenSettings.moveDuration))
                .InsertCallback(0.0f, () => { tweenSettings.DoRotate(transform, false); })
                .Insert(0.0f,
                    transform.DOScale(tweenSettings.scaleEndValue, tweenSettings.scaleDuration))
                //finish
                .Insert(tweenSettings.moveDuration,
                    transform.DOLocalMove(startPosition, tweenSettings.moveDuration).SetEase(tweenSettings.moveCurve))
                .InsertCallback(tweenSettings.moveDuration,
                    () => { tweenSettings.DoRotate(chestIcon.transform, false); })
                .Insert(tweenSettings.moveDuration,
                    transform.DOScale(tweenSettings.scaleStartValue, tweenSettings.scaleDuration))
                .OnComplete(() =>
                {
                    ChestManager.ChestAddedAnimComplete(this);
                });
        }
    }
}