using System;
using Core;
using Enums;
using UnityEngine;

namespace Utils
{
    public static class Constants
    {
        public const float LoadingSpinnerDegrees = 2.33f;
        public const string ConsentKey = "ConsentKey";
        public const float WorldSpaceTolerance = 0.01f;
        public const float FloatTolerance = 0.001f;

        //alertMessages
        public const string LowResourcesPrefix = "Not enough ";
        public const string UpgradeCompletedSuffix = " Upgraded";
        public const string PurchaseMessage = "Purchase Successful";
        public const string MergeMessage = "Chest Merge Successful";
        public const string ClaimMessage = " Chest Claimed";
        
        public static string GetResourceTypeName(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.BananaCoins:
                    return "Banana Coins";
                case ResourceType.BluePrints:
                    return "Blueprints";
                case ResourceType.StarFruits:
                    return "Starfruits";
                default:
                    return "Resources";
            }
        }
        public static string GetUpgradeTypeName(UpgradeTypes upgradeTypes)
        {
            switch (upgradeTypes)
            {
                case UpgradeTypes.AutoRoll:
                    return "Auto Roll ";
                case UpgradeTypes.CoinSlot:
                    return "Coin Slot ";
                case UpgradeTypes.ChestClaim:
                    return "Auto Chest ";
                case UpgradeTypes.ChestMerge:
                    return "Chest Merge ";
                case UpgradeTypes.SlotsLight:
                    return "Slots Light ";
                default:
                    return "Upgradeable ";
            }
        }

        //colours
        public static Color toggleOn = new Color32(0x39, 0xA8, 0x39, 0xFF);
        public static Color toggleOff = new Color32(0xAE, 0x17, 0x1B, 0xFF);

        //spriteAssets
        public const string BananaCoinIcon = "<sprite=\"BC\" index=0>";
        public const string BluePrintIcon = "<sprite=\"BP\" index=0>";
        public const string StarFruitIcon = "<sprite=\"SF\" index=0>";
        public const string SufficientCurrencyPrefix = "<color=#FFFFFF>";
        public const string InsufficientCurrencyPrefix = "<color=#BD2424>";

        public static string GetCurrencySpriteAsset(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.BananaCoins:
                    return "<sprite=\"BC\" index=0> ";
                case ResourceType.BluePrints:
                    return "<sprite=\"BP\" index=0> ";
                case ResourceType.StarFruits:
                    return "<sprite=\"SF\" index=0> ";
                default:
                    return "\"nonSpriteAssetCurrencyRequested\"";
            }
        }

        //firebase
        public const string PlayerDataPrefix = "users";
        public const string PlayerDataSuffix = "userData";
        public const string ChestData = "cd";
        public const string ChestPayout = "cp";
        public const string CurrentRoll = "cr";
        public const string LastPayout = "lp";
        public const string UpgradeData = "ud";
        public const string RollData = "rd";
        public const string WalletData = "wd";
        public const string ReelRollCloudFunction = "usersReelRollOnCall";
        public const string AdRewardClaimCloudFunction = "usersAdRewardClaimOnCall";
        public const string ChestClaimCloudFunction = "usersChestClaimOnCall";
        public const string ChestMergeCloudFunction = "usersChestMergeOnCall";
        public const string ChestOpenCloudFunction = "usersChestOpenOnCall";
        public const string DoUpgradeCloudFunction = "usersUpgradeDoOnCall";
        public const string ProductPurchaseFunction = "usersProductPurchaseOnCall";
        
        //urls
        public const string FirebaseDatabaseUrl = "https://he-loves-the-slots.firebaseio.com/";
        public const string PrivacyPolicyUrl = "https://he-loves-the-slots.web.app/PrivacyPolicy.html";
        public const string TermsAndConditionsUrl = "https://he-loves-the-slots.web.app/TermsAndConditions.html";
        public const string ShirtBuyUrl = "https://www.spreadshirt.com/user/Original+Star";
        public const string ShirtClaimUrl = "https://www.spreadshirt.com/";
        public const string LinkedInUrl = "https://www.linkedin.com/in/seth-games-51386baa/";

        //chests
        public const string ConfirmClaimMessagePrefix = "You are ";
        public const string ConfirmClaimMessageSuffix = " rolls from a better chest.";
        public const string ChestRewardPrefix = " ~";
        public const float ChestAddInterval = 0.666f;
        public const string ChestButtonClaim = "Claim";
        public const string ChestButtonMerge = "Merge";
        public const string ChestButtonUpgrade = "Upgrade";

        //ui
        public const float CoinsBackgroundBaseWidth = 280.0f;
        public const float CoinsBackgroundWidthMultiplier = 50.0f;

        //resource paths
        public const string ChestMergesPath = "ChestMerges";
        public const string ChestRewardsPath = "ChestRewards";
        public const string ChestsPath = "Chests";
        public const string FruitParticleSpritesPath = "FruitSprites";
        public const string MusicTrackPath = "MusicTracks";
        public const string ShopCategoryPath = "ShopCategories";
        public const string ShopProductPath = "ShopProducts";
        public const string SoundEffectPath = "SoundEffects";
        public const string UpgradesPath = "Upgrades";

        //tween settings paths
        public const string CloseButtonPunchSettingPath = "TweenSettings/closeButtonPunchSetting";

        //coin
        public const int CoinTrayMax = 12;
        public const float SpawnRange = 0.8f;
        public const float SpawnHeight = -2.0f;
        public const float CoinLoadSpeed = 0.67f;
        public const int CoinStartingSortingOrder = 1;
        public const int CoinInsertedSortingOrder = 11;
        public const int CoinPressedSortingOrder = 14;

        //arm
        public const float ArmDragMultiplier = 0.267f;
        public const float ArmPullTriggerAmount = 0.667f;
        public const float ArmPullResetSpeed = 1.667f;
        public const float ArmLockedTriggerAmount = 0.4f;
        public const float ClipTriggerTime = 0.666f;
        
        //bet indicators
        public const float OffsetAmountLow = -0.6165f;
        public const float ScaleAmount = 1.215f;
        public const float RotationAmount = 36f;

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
        public const float PauseBetweenRolls = 0.666f;

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

        //payoutWords
        public const string JackpotMessage = "Jackpot!";
        public const string YouWinMessage = "You Win!";

        public static readonly GameEvents GameEvents = new GameEvents();
    }

    [Serializable]
    public class GameEvents
    {
        //for debug only
        // public string coinInsertEvent = "coinInsertEvent";
        public string testEvent = "testEvent";

        public string armPullEvent = "armPullEvent";
        public string autoRollEvent = "autoRollEvent";
        public string chestOpenEvent = "chestOpenEvent";
        public string chestRefreshEvent = "chestRefreshEvent";
        public string coinConsumeEvent = "coinConsumeEvent";
        public string coinCreatedEvent = "coinCreatedEvent";
        public string coinDroppedEvent = "coinDroppedEvent";
        public string coinLoadEvent = "coinLoadEvent";
        public string generateCoinEvent = "generateCoinEvent";
        public string payoutFinishEvent = "payoutFinishEvent";
        public string payoutStartEvent = "payoutStartEvent";
        public string refreshCurrencyEvent = "refreshCurrencyEvent";
        public string refreshUiEvent = "refreshUiEvent";
        public string upgradeRefreshEvent = "upgradeRefreshEvent";
        public string userEarnedRewardEvent = "userEarnedRewardEvent";
        public string wheelResultEvent = "wheelResultEvent";
        public string wheelRollEvent = "wheelRollEvent";
    }
}