using UnityEngine;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "particleSettings" , menuName = "MyAssets/ParticleSettings", order = 26)]
    public class ParticleSettings : ScriptableObject
    {
        public Vector2 startScaleMinMax;
        public Vector3 startRotationMin;
        public Vector3 startRotationMax;
        public Vector2 angularVelocityMinMax;
        public float gravityModifier;
    }
}