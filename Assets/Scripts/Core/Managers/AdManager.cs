using System;
using GoogleMobileAds.Api;

namespace Core.Managers
{
    public class AdManager : Singleton<AdManager>
    {
        private RewardedAd _rewardedAd;
        
#if UNITY_ANDROID
        private const string TestRewardedAdId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
        private const string TestRewardedAdId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unexpected_platform";
#endif

        private void Start()
        {
            MobileAds.Initialize(initStatus => { });

            _rewardedAd = new RewardedAd(TestRewardedAdId);

            // Called when an ad request has successfully loaded.
            // _rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
            // Called when an ad request failed to load.
            // _rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
            // Called when an ad is shown.
            // _rewardedAd.OnAdOpening += HandleRewardedAdOpening;
            // Called when an ad request failed to show.
            // _rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
            // Called when the user should be rewarded for interacting with the ad.
            _rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
            // Called when the ad is closed.
            _rewardedAd.OnAdClosed += HandleRewardedAdClosed;

            CreateAndLoadRewardedAd();
        }

        private void CreateAndLoadRewardedAd()
        {
            _rewardedAd = new RewardedAd(TestRewardedAdId);

            var request = new AdRequest.Builder().Build();
            _rewardedAd.LoadAd(request);
        }

        public void ShowRewardedAd()
        {
            if (!_rewardedAd.IsLoaded()) return;

            _rewardedAd.Show();
        }

        /*
        private void HandleRewardedAdLoaded(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void HandleRewardedAdOpening(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs e)
        {
            throw new NotImplementedException();
        }
        */
        private void HandleUserEarnedReward(object sender, Reward e)
        {
            EventManager.userEarnedReward.Raise();
        }

        private void HandleRewardedAdClosed(object sender, EventArgs e)
        {
            CreateAndLoadRewardedAd();
        }
    }
}