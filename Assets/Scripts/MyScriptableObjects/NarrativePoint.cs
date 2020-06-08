using System;
using Enums;
using UnityEngine;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "NarrativePoint", menuName = "MyAssets/Variables/NarrativePoints", order = 40)]
    public class NarrativePoint : ScriptableObject
    {
        public NarrativeTypes id;
        public NarrativeShard[] narrativeShard;
        public bool fireAndForget;

        private void OnEnable()
        {
            id = (NarrativeTypes) Enum.Parse(typeof(NarrativeTypes), name);
        }
    }

    [Serializable]
    public class NarrativeShard
    {
        public string[] textChunks;
        public Sprite storyImage;
    }
}