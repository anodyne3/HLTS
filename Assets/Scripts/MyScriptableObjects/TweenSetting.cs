using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "TweenSetting", menuName = "MyAssets/TweenSettings/TweenSetting", order = 55)]
    public class TweenSetting : ScriptableObject
    {
        [Header("Move")] public float moveDuration;
        public float delayBetweenInstance;
        public AnimationCurve moveCurve;
        public float spawnZoneRadius;
        public float moveOffset;

        [Header("Rotate")] [SerializeField] private TweenRotateSetting rotateSetting;

        [Header("Punch")] [SerializeField] private TweenPunchSetting punchSetting;

        [Header("Fade")] [SerializeField] private TweenFadeSetting fadeSetting;
        public float fadeStartDelay;

        [Header("SizeDelta")] public float sizeDeltaDuration;

        [Header("Scale")] public Vector3 scaleStartValue;
        public Vector3 scaleEndValue;
        public float scaleDuration;

        public Ease sequenceEasing;

        public int GetSpawnAmount(long difference)
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

        public void DoRotate(Transform rotateTarget, bool pooled = true)
        {
            rotateSetting.DoRotate(rotateTarget, pooled);
        }
        
        public void KillRotate()
        {
            rotateSetting.Kill();
        }

        public void DoPunch(Transform punchTarget, bool pooled = true)
        {
            punchSetting.DoPunch(punchTarget, pooled);
        }

        public void DoFade(Image target, bool pooled = true)
        {
            fadeSetting.DoFade(target, pooled);
        }

        public void DoFade(CanvasGroup target, bool pooled = true)
        {
            fadeSetting.DoFade(target, pooled);
        }
    }
}