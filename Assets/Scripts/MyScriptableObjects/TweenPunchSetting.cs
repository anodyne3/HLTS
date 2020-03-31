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
    }
}