using Enums;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu]
    public class SoundEffect : ScriptableObject
    {
        public SoundEffectType soundEffectType;
        public AudioClip[] soundEffectArray;
    }
}