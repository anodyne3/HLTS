using DG.Tweening;
using MyScriptableObjects;
using TMPro;
using UnityEngine.UI;

namespace Core.UI
{
    public class DeductCurrencyPrefab : GlobalAccess
    {
        public Image currencyImage;
        public TMP_Text currencyText;
        public TweenSetting tweenSettings;
        private Sequence _deductSequence;

        public void Init(long amount)
        {
            currencyText.text = amount.ToString();
            var doMoveOffset = transform.localPosition;
            doMoveOffset.y -= tweenSettings.moveOffset;
            gameObject.SetActive(true);

            if (_deductSequence != null)
            {
                _deductSequence.Restart();
                return;
            }

            _deductSequence = DOTween.Sequence();
            _deductSequence
                .SetAutoKill(false)
                .SetRecyclable(true)
                .Insert(0.0f, transform.DOLocalMove(doMoveOffset, tweenSettings.tweenDuration).SetEase(tweenSettings.moveCurve))
                .Insert(tweenSettings.fadeStartDelay, currencyImage.DOFade(tweenSettings.fadeEndValue, tweenSettings.fadeDuration))
                .Insert(tweenSettings.fadeStartDelay, currencyText.DOFade(tweenSettings.fadeEndValue, tweenSettings.fadeDuration))
                .AppendCallback(() =>
                {
                    gameObject.SetActive(false);
                    HudController.ResizeCurrencySizeDelta();
                });
        }
    }
}