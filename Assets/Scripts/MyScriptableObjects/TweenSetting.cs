using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "TweenSetting", menuName = "MyAssets/TweenSettings/TweenSetting", order = 55)]
    public class TweenSetting : ScriptableObject
    {
        [Header("Move")]
        public float moveDuration;
        public float delayBetweenInstance;
        public AnimationCurve moveCurve;
        public float spawnZoneRadius;
        public float moveOffset;

        [Header("Punch")]
        [SerializeField] private TweenPunchSetting punchSetting;

        [Header("Fade")]
        public float fadeDuration;
        public float fadeStartDelay;
        public float fadeEndValue;

        [Header("SizeDelta")]
        public float sizeDeltaDuration;

        [Header("Scale")]
        public Vector3 scaleStartValue;
        public Vector3 scaleEndValue;
        public float scaleDuration;

        public Ease sequenceEasing;

        private Tweener _punchTween;

        public static int GetSpawnAmount(long difference)
        {
            if (difference > 2000)
                return 25;
            if (difference > 1000)
                return 20;
            if (difference > 200)
                return 15;
            if (difference > 9)
                return 10;
            
            return (int) difference;
        }

        public Vector3 RandomSpawnPosition()
        {
            return Random.insideUnitCircle * spawnZoneRadius;
        }

        public void DoPunch(Transform punchTarget)
        {
            if (_punchTween != null && !_punchTween.IsComplete())
            {
                _punchTween.Kill(true);
            }

            _punchTween = punchTarget.DOPunchScale(
                punchSetting.punchAmount,
                punchSetting.punchDuration,
                punchSetting.punchVibrato,
                punchSetting.punchElasticity);
        }
    }
}