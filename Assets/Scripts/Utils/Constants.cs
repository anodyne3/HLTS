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
        public const float ArmPullResetSpeed = 1.0f;
        
        //reels
        public const int TotalSpinTime = 360;
        public const int RightReelStopTime = 210;
        public const int MiddleReelStopTime = 90;
        public const float FastSpinDegrees = 15.0f;
        public const float MediumSpinDegrees = 5.0f;
        public const float SlowSpinDegrees = 1.0f;
        
        //payout
        public const int PlumsPayout = 25;
        public const int CherriesPayout = 150;
        public const int DiamondsPayout = 500;
        public const int MixedPayout = 2000;
        public const int BarnanaPayout = 5000;

        public static readonly GameEvents GameEvents = new GameEvents();
    }


    [Serializable]
    public class GameEvents
    {
        public string armPullEvent = "armPullEvent";
        public string coinConsumeEvent = "coinConsumeEvent";
        public string coinLoadEvent = "coinLoadEvent";
        public string wheelResultEvent = "wheelResultEvent";
        public string wheelRollEvent = "wheelRollEvent";
    }
}