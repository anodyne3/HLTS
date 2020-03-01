using System;
using Core;
using Core.GameData;
using Enums;
using UnityEngine;

namespace Utils
{
    public static class Constants
    {
        public const string ConsentKey = "ConsentKey";

        public const string MusicTrackPath = "MusicTracks";
        public const string SoundEffectPath = "SoundEffects";

        public const float WorldSpaceTolerance = 0.01f;

        public const float SpawnRange = 0.8f;
        public const float SpawnHeight = -2.0f;
        public const float CoinLoadSpeed = 0.67f;
        
        public const float ArmPullTriggerAmount = 10.0f;
        public const float ArmPullResetSpeed = 1.0f;
        
        //reels
        public const int TotalSpinTime = 330;
        public const int RightReelStopTime = 180;
        public const int MiddleReelStopTime = 120;
        public const float FastSpinDegrees = 15.0f;
        public const float MediumSpinDegrees = 5.0f;
        public const float SlowSpinDegrees = 1.0f;
        
        //fruit
        public static readonly FruitDefinition[] FruitDefinitions =
        {
            new FruitDefinition {Id = 0, FruitType = FruitType.Cherries},
            new FruitDefinition {Id = 1, FruitType = FruitType.Plum},
            new FruitDefinition {Id = 2, FruitType = FruitType.Diamond},
            new FruitDefinition {Id = 3, FruitType = FruitType.Plum},
            new FruitDefinition {Id = 4, FruitType = FruitType.Cherries},
            new FruitDefinition {Id = 5, FruitType = FruitType.Banana},
            new FruitDefinition {Id = 6, FruitType = FruitType.Plum},
            new FruitDefinition {Id = 7, FruitType = FruitType.Cherries},
            new FruitDefinition {Id = 8, FruitType = FruitType.Plum},
            new FruitDefinition {Id = 9, FruitType = FruitType.Diamond},
            new FruitDefinition {Id = 10, FruitType = FruitType.Plum},
            new FruitDefinition {Id = 11, FruitType = FruitType.Bar}
        };
        
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
        public string autoSlotModeEvent = "autoSlotModeEvent";
        public string coinInsertEvent = "coinInsertEvent";
        
        public string armPullEvent = "armPullEvent";
        public string coinConsumeEvent = "coinConsumeEvent";
        public string coinCreatedEvent = "coinCreatedEvent";
        public string coinDroppedEvent = "coinDroppedEvent";
        public string coinLoadEvent = "coinLoadEvent";
        public string generateCoinEvent = "generateCoinEvent";
        public string refreshUiEvent = "refreshUiEvent";
        public string wheelResultEvent = "wheelResultEvent";
        public string wheelRollEvent = "wheelRollEvent";
    }
}