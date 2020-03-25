using UnityEngine;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "Burst", menuName = "MyAssets/ParticleBurst", order = 25)]
    public class ParticleBurst : ScriptableObject
    {
        public float burstTime;
        public int burstAmountMin;
        public int burstAmountMax;
        public int cycles;
        public float burstInterval;
        public Vector3 startPositionOffset;
        public Vector2 startVelocityMin;
        public Vector2 startVelocityMax;
        public float lifeSpan;
    }
}