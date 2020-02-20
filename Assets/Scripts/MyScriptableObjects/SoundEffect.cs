using Enums;
using UnityEngine;

namespace MyScriptableObjects
{
    [CreateAssetMenu]
    public class SoundEffect : ScriptableObject
    {
        public SoundEffectType soundEffectType;
        public AudioClip[] soundEffectArray;
    }
}