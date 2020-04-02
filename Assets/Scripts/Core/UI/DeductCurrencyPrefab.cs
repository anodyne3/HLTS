using DG.Tweening;
using MyScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class DeductCurrencyPrefab : GlobalAccess
    {
        public CanvasGroup canvasGroup;
        public TMP_Text currencyText;
        public TweenSetting tweenSettings;
        private Sequence _deductSequence;

        public void Init(long amount)
        {
            currencyText.text = amount.ToString();
            var doMoveOffset = transform.localPosition;
            doMoveOffset.y -= tweenSettings.moveOffset;

            if (_deductSequence != null)
            {
                _deductSequence.Restart();
                return;
            }

            _deductSequence = DOTween.Sequence();
            _deductSequence
                .SetAutoKill(false)
                .SetRecyclable(true)
                .Insert(0.0f, transform.DOLocalMove(doMoveOffset, tweenSettings.moveDuration).SetEase(tweenSettings.moveCurve))
                .InsertCallback(0.0f, () => canvasGroup.alpha = 1.0f)
                .InsertCallback(tweenSettings.fadeStartDelay, () => tweenSettings.DoFade(canvasGroup))
                .AppendCallback(() =>
                {
                    HudController.ResizeCurrencySizeDelta();
                });
        }
    }
}