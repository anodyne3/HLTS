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

        public static readonly Vector2 CoinSlotStartPosition = new Vector2(0.0f, 100.0f);
        public static readonly Vector2 CoinSlotDestination = new Vector2(0.0f, 0.0f);
        public const float CoinLoadSpeed = 1.0f;
        
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