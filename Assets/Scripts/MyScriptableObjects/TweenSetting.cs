using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "TweenSetting", menuName = "MyAssets/TweenSetting", order = 0)]
    public class TweenSetting : ScriptableObject
    {
        [Header("Move")]
        [SerializeField] public float tweenDuration;
        [SerializeField] public float delayBetweenInstance;
        [SerializeField] public AnimationCurve moveCurve;
        [SerializeField] public float spawnZoneRadius;
        [SerializeField] public float moveOffset;

        [Header("Punch")]
        [SerializeField] private Vector3 punchAmount;
        [SerializeField] private float punchDuration;
        [SerializeField] private int punchVibrato;
        [SerializeField] private float punchElasticity;

        [Header("Fade")]
        [SerializeField] public float fadeDuration;
        [SerializeField] public float fadeStartDelay;
        [SerializeField] public float fadeEndValue;

        [Header("SizeDelta")]
        [SerializeField] public float sizeDeltaDuration;

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
                punchAmount,
                punchDuration,
                punchVibrato,
                punchElasticity);
        }
    }
}