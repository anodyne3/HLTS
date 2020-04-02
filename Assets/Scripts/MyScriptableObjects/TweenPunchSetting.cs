using DG.Tweening;
using UnityEngine;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "PunchSetting", menuName = "MyAssets/TweenSettings/TweenPunchSetting", order = 56)]
    public class TweenPunchSetting : ScriptableObject
    {
        [SerializeField] public Vector3 punchAmount;
        [SerializeField] public float punchDuration;
        [SerializeField] public int punchVibrato;
        [SerializeField] public float punchElasticity;
        private Tweener _punchTween;

        public void DoPunch(Transform punchTarget, bool pooled = true)
        {
            if (_punchTween != null)
            {
                if (pooled)
                {
                    _punchTween.Restart();
                    return;
                }

                _punchTween.Kill(true);
            }

            _punchTween = punchTarget.DOPunchScale(
                punchAmount,
                punchDuration,
                punchVibrato,
                punchElasticity);

            if (pooled)
                _punchTween.SetAutoKill(false).SetRecyclable(true);
        }
    }
}