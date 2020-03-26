using System;
using Core;
using Enums;

namespace Utils
{
    public static class Constants
    {
        public const float LoadingSpinnerDegrees = 2.33f;
        public const string ConsentKey = "ConsentKey";
        public const float WorldSpaceTolerance = 0.01f;

        //firebase
        public const string PlayerDataPrefix = "users";
        public const string PlayerDataSuffix = "userData";
        public const string ReelRollCloudFunction = "usersReelRollOnCall";

        //audio
        public const string MusicTrackPath = "MusicTracks";
        public const string SoundEffectPath = "SoundEffects";
        
        //ui rect
        public const float CoinsBackgroundBaseWidth = 300.0f;
        public const float CoinsBackgroundWidthMultiplier = 50.0f;
        public const float SizeDeltaTweenDuration = 0.267f;

        //resources
        public const string FruitParticleSpritesPath = "FruitSprites";
        
        //coin
        public const int CoinTrayMax = 12;
        public const float SpawnRange = 0.8f;
        public const float SpawnHeight = -2.0f;
        public const float CoinLoadSpeed = 0.67f;

        //arm
        public const float ArmPullTriggerAmount = 5.0f;
        public const float ArmPullResetSpeed = 6.67f;
        public const float ArmLockedTriggerAmount = 2.0f;
        public const float ClipTriggerTime = 0.6f;
        
        //animStates
        public const string ArmPullState = "LeverPull";
        public const string LeverBallGlowState = "LeverBallGlow";
        public const string WinLightState = "WinLight";

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
            new FruitDefinition {Id = 1, FruitType = FruitType.Plums},
            new FruitDefinition {Id = 2, FruitType = FruitType.Diamantes},
            new FruitDefinition {Id = 3, FruitType = FruitType.Plums},
            new FruitDefinition {Id = 4, FruitType = FruitType.Cherries},
            new FruitDefinition {Id = 5, FruitType = FruitType.Bananas},
            new FruitDefinition {Id = 6, FruitType = FruitType.Plums},
            new FruitDefinition {Id = 7, FruitType = FruitType.Cherries},
            new FruitDefinition {Id = 8, FruitType = FruitType.Plums},
            new FruitDefinition {Id = 9, FruitType = FruitType.Diamantes},
            new FruitDefinition {Id = 10, FruitType = FruitType.Plums},
            new FruitDefinition {Id = 11, FruitType = FruitType.Bars}
        };

        //payout
        public const int PlumsPayout = 25;
        public const int CherriesPayout = 150;
        public const int DiamondsPayout = 500;
        public const int MixedPayout = 1250;
        public const int BarnanaPayout = 2500;
        
        //payoutWords
        public const string JackpotMessage = "Jackpot!";
        public const string YouWinMessage = "You Win!";

        public static readonly GameEvents GameEvents = new GameEvents();
    }

    [Serializable]
    public class GameEvents
    {
        //for debug only
        public string autoSlotModeEvent = "autoSlotModeEvent";
        public string coinInsertEvent = "coinInsertEvent";

        public string armPullEvent = "armPullEvent";
        public string coinConsumeEvent = "coinConsumeEvent";
        public string coinCreatedEvent = "coinCreatedEvent";
        public string coinDroppedEvent = "coinDroppedEvent";
        public string coinLoadEvent = "coinLoadEvent";
        public string generateCoinEvent = "generateCoinEvent";
        public string payoutFinishEvent = "payoutFinishEvent";
        public string payoutStartEvent = "payoutStartEvent";
        public string refreshUiEvent = "refreshUiEvent";
        public string wheelResultEvent = "wheelResultEvent";
        public string wheelRollEvent = "wheelRollEvent";
    }
}