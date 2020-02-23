using System;
using UnityEngine;

namespace Utils
{
    public static class Constants
    {
        public const string ConsentKey = "ConsentKey";

        public const string MusicTrackPath = "MusicTracks";
        public const string SoundEffectPath = "SoundEffects";

        public const float WorldSpaceTolerance = 0.01f;

        public const float CoinLoadSpeed = 0.67f;
        
        public const float ArmPullTriggerAmount = 10.0f;

        public static readonly GameEvents GameEvents = new GameEvents();
    }


    [Serializable]
    public class GameEvents
    {
        public string armPullEvent = "armPullEvent";
        public string coinLoadEvent = "coinLoadEvent";
        public string wheelResultEvent = "wheelResultEvent";
        public string wheelRollEvent = "wheelRollEvent";
    }
}