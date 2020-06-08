using System;
using Enums;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "NarrativePoint", menuName = "MyAssets/Variables/NarrativePoints", order = 40)]
    public class NarrativePoint : ScriptableObject
    {
        public NarrativeTypes id;
        public NarrativeShard[] narrativeShard;
        public TimelinePlayable timelinePlayable;

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