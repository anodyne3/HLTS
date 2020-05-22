using DG.Tweening;
using MyScriptableObjects;
using TMPro;
using UnityEngine;

namespace Core.UI
{
    public class AlertMessage : GlobalClass
    {
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private TweenSetting initTween;
        [SerializeField] private TweenSetting closeTween;
        [SerializeField] private CanvasGroup canvasGroup;

        private Sequence _sequence;

        [ContextMenu("alertMessageTest")]
        public void TestAlertMessage()
        {
            Init("This is a Test Message!");
        }

        public void Init(string message)
        {
            messageText.text = message;

            if (_sequence != null)
            {
                _sequence.Restart();
                return;
            }

            var doMoveOffset = transform.localPosition;
            doMoveOffset.y += initTween.moveOffset;

            _sequence = DOTween.Sequence();
            _sequence
                .SetAutoKill(false)
                .SetRecyclable(true)
                .Insert(0.0f,
                    transform.DOLocalMove(doMoveOffset, initTween.moveDuration).SetEase(initTween.moveCurve))
                .InsertCallback(0.0f, () => canvasGroup.alpha = 1.0f)
                .Insert(0.0f, transform.DOScale(initTween.scaleStartValue, 0.0f).SetEase(initTween.moveCurve))
                .InsertCallback(0.0f, () => initTween.DoRotate(transform))
                .Insert(0.0f,
                    transform.DOScale(initTween.scaleEndValue, initTween.scaleDuration).SetEase(initTween.moveCurve))
                .Insert(closeTween.fadeStartDelay,
                    transform.DOLocalMove(-doMoveOffset, closeTween.moveDuration).SetEase(closeTween.moveCurve))
                .InsertCallback(initTween.fadeStartDelay, () => closeTween.DoRotate(transform))
                .Insert(initTween.fadeStartDelay,
                    transform.DOScale(closeTween.scaleEndValue, closeTween.scaleDuration).SetEase(closeTween.moveCurve))
                .InsertCallback(closeTween.fadeStartDelay, () => initTween.DoFade(canvasGroup, false));
        }
    }
}