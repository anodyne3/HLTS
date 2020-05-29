using System;
using Enums;
using GoogleMobileAds.Api;

namespace Core.Managers
{
    public class AdManager : Singleton<AdManager>
    {
        private RewardedAd _doublePayoutAd;
        private RewardedAd _doubleChestAd;
        public static Reward reward;
        private EventManager _eventManager;
        

#if UNITY_ANDROID
        private const string DoublePayoutAdId = "ca-app-pub-6539801580858512/6919681000";
        private const string DoubleChestAdId = "ca-app-pub-6539801580858512/5059218664";

#elif UNITY_IPHONE
        private const string TestRewardedAdId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unexpected_platform";
#endif

        private void Start()
        {
            MobileAds.Initialize(initStatus =>
            {
                InitAllAds();
            });

            _eventManager = EventManager;
        }

        private void InitAllAds()
        {
            _doublePayoutAd = CreateAndLoadRewardedAd(AdType.DoublePayout);
            _doubleChestAd = CreateAndLoadRewardedAd(AdType.DoubleChest);
        }

        public void ShowRewardedAd(AdType adType)
        {
            var rewardedAd = GetAdByType(adType);
            
            if (rewardedAd == null || !rewardedAd.IsLoaded()) return;
            
            rewardedAd.Show();
            // FirebaseFunctionality.shownAd = adType;
        }

        private RewardedAd CreateAndLoadRewardedAd(AdType adType)
        {
            string rewardedAdName;
            
            switch (adType)
            {
                case AdType.DoublePayout:
                    rewardedAdName = DoublePayoutAdId;
                    break;
                case AdType.DoubleChest:
                    rewardedAdName = DoubleChestAdId;
                    break;
                default:
                    AlertMessage.Init(adType + " ad type does not exist");
                    return null;
            }
            
            var rewardedAd = new RewardedAd(rewardedAdName);
            
            rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
            rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
            rewardedAd.OnAdClosed += HandleRewardedAdClosed;
            rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
            rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;

            rewardedAd.LoadAd(new AdRequest.Builder().Build());

            return rewardedAd;
        }

        private void HandleRewardedAdLoaded(object sender, EventArgs args)
        {
            // AlertMessage.Init("HandleRewardedAdLoaded " + sender);
        }

        private void HandleUserEarnedReward(object sender, Reward e)
        {
            reward = e;
            _eventManager.userEarnedReward.Raise();
        }

        private void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs e)
        {
            AlertMessage.Init("ad failed to load");
        }

        private void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs e)
        {
            AlertMessage.Init("ad failed to show");
        }

        private void HandleRewardedAdClosed(object sender, EventArgs e)
        {
            switch (FirebaseFunctionality.shownAd)
            {
                case AdType.DoublePayout:
                    _doublePayoutAd = CreateAndLoadRewardedAd(FirebaseFunctionality.shownAd);
                    return;
                case AdType.DoubleChest:
                    _doubleChestAd = CreateAndLoadRewardedAd(FirebaseFunctionality.shownAd);
                    return;
                default:
                    AlertMessage.Init(FirebaseFunctionality.shownAd + " ad does not exist");
                    return;
            }
        }

        public bool AdIsLoaded(AdType adType)
        {
            var requestedAd = GetAdByType(adType);

            return requestedAd != null && requestedAd.IsLoaded();
        }

        private RewardedAd GetAdByType(AdType adType)
        {
            switch (adType)
            {
                case AdType.DoublePayout:
                    return _doublePayoutAd;
                case AdType.DoubleChest:
                    return _doubleChestAd;
                default:
                    AlertMessage.Init(adType + " ad does not exist");
                    return null;
            }
        }
    }
}