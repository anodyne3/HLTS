using DG.Tweening;
using UnityEngine;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "RotateSetting", menuName = "MyAssets/TweenSettings/TweenRotateSetting", order = 56)]
    public class TweenRotateSetting : ScriptableObject
    {
        [SerializeField] public Vector3 rotateEndValue;
        [SerializeField] public float rotateDuration;
        [SerializeField] public Ease rotateEasing;
        [SerializeField] public RotateMode rotateMode;
        private Tweener _rotateTween;

        public void DoRotate(Transform rotateTarget, bool pooled = true)
        {
            if (_rotateTween != null)
            {
                if (pooled)
                {
                    _rotateTween.Restart();
                    return;
                }

                _rotateTween.Kill(true);
            }

            _rotateTween = rotateTarget.DORotate(
                rotateEndValue,
                rotateDuration,
                rotateMode = RotateMode.FastBeyond360)
                .SetEase(rotateEasing);

            if (pooled)
                _rotateTween.SetAutoKill(false).SetRecyclable(true);
        }
    }
}