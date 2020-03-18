using Enums;
using UnityEngine;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "SoundEffect - ", menuName = "MyAssets/SoundEffect", order = 20)]
    public class SoundEffect : ScriptableObject
    {
        public SoundEffectType soundEffectType;
        public AudioClip[] soundEffectArray;
    }
}