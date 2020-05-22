using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "FadeSetting", menuName = "MyAssets/TweenSettings/TweenFadeSetting", order = 57)]
    public class TweenFadeSetting : ScriptableObject
    {
        [SerializeField] public float fadeDuration;
        [SerializeField] public float fadeEndValue;
        private Tweener _tween;

        public void DoFade(Image target, bool pooled = true)
        {
            if (_tween != null)
            {
                if (pooled)
                {
                    _tween.Restart();
                    return;
                }

                _tween.Kill(true);
            }

            _tween = target.DOFade(fadeEndValue, fadeDuration);

            if (pooled)
                _tween.SetAutoKill(false).SetRecyclable(true);
        }

        public void DoFade(CanvasGroup target, bool pooled = true)
        {
            if (_tween != null)
            {
                if (pooled)
                {
                    _tween.Restart();
                    return;
                }

                _tween.Kill(true);
            }

            _tween = target.DOFade(fadeEndValue, fadeDuration);

            if (pooled)
                _tween.SetAutoKill(false).SetRecyclable(true);
        }
    }
}