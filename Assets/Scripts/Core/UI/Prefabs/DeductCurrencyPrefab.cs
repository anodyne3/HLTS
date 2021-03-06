using DG.Tweening;
using MyScriptableObjects;
using TMPro;
using UnityEngine;

namespace Core.UI.Prefabs
{
    public class DeductCurrencyPrefab : GlobalAccess
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text currencyText;
        [SerializeField] private TweenSetting tweenSettings;
        private Sequence _deductSequence;

        public void Init(long currencyDifference)
        {
            currencyText.text = currencyDifference.ToString();
            var doMoveOffset = transform.localPosition;
            var rectWidth = CurrencyManager.currenciesRect.rect.width + tweenSettings.moveOffset;
            doMoveOffset.x += rectWidth;

            if (_deductSequence != null)
            {
                _deductSequence.Restart();
                return;
            }

            _deductSequence = DOTween.Sequence();
            _deductSequence
                .SetAutoKill(false)
                .SetRecyclable(true)
                .Insert(0.0f,
                    transform.DOLocalMove(doMoveOffset, tweenSettings.moveDuration).SetEase(tweenSettings.moveCurve))
                .InsertCallback(0.0f, () => canvasGroup.alpha = 1.0f)
                .InsertCallback(tweenSettings.fadeStartDelay, () => tweenSettings.DoFade(canvasGroup, false))
                .AppendCallback(CurrencyManager.ResizeCurrencyRect);
        }
    }
}