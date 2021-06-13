using Enums;
using Yodo1.MAS;

namespace Core.Managers
{
    public class AdManager : Singleton<AdManager>
    {
        public AdType reward;
        
        public static void Init()
        {
            Yodo1U3dMas.SetInitializeDelegate(InitializeDelegate);
            Yodo1U3dMas.InitializeSdk();
            Yodo1U3dMas.SetRewardedAdDelegate(RewardedAdDelegate);
        }

        private static void InitializeDelegate(bool success, Yodo1U3dAdError error)
        {
            if (success) return;
            
            AlertMessage.Init("Ads Initialisation Failure: " + error);
        }

        private static void RewardedAdDelegate(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError error)
        {
            // AlertMessage.Init("[Yodo1 Mas] RewardVideoDelegate:" + adEvent + "\n" + error);
            switch (adEvent)
            {
                case Yodo1U3dAdEvent.AdClosed:
                    // AlertMessage.Init("[Yodo1 Mas] Reward video ad has been closed.");
                    break;
                case Yodo1U3dAdEvent.AdOpened:
                    // AlertMessage.Init("[Yodo1 Mas] Reward video ad has shown successful.");
                    break;
                case Yodo1U3dAdEvent.AdError:
                    AlertMessage.Init("ad failed to load, " + error);
                    break;
                case Yodo1U3dAdEvent.AdReward:
                    EventManager.userEarnedReward.Raise();
                    // AlertMessage.Init("[Yodo1 Mas] Reward video ad reward, give rewards to the player.");
                    break;
                default:
                    AlertMessage.Init("Unrecognized adEvent: " + nameof(adEvent) + ", " + adEvent);
                    break;
            }
        }

        public void ShowRewardedAd(AdType adType)
        {
            reward = adType;
            
            switch (adType)
            {
                case AdType.DoubleChest:
                case AdType.DoublePayout:
                    Yodo1U3dMas.ShowRewardedAd();
                    break;
                default:
                    return;
            }
        }

        public static bool AdIsLoaded(AdType adType)
        {
            switch (adType)
            {
                case AdType.DoubleChest:
                case AdType.DoublePayout:
                    return Yodo1U3dMas.IsRewardedAdLoaded();
                default:
                    return false;
            }
        }
    }
}