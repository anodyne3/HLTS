using System;
using Core.UI;
using GoogleMobileAds.Api;
using UnityEngine;

namespace Core.Managers
{
    public class AdManager : Singleton<AdManager>
    {
        private RewardedAd _rewardedAd;
        
#if UNITY_ANDROID
        private const string TestRewardedAdId = "ca-app-pub-3940256099942544/5224354917";
        private const string DoublePayoutAdId = "ca-app-pub-6539801580858512/6919681000";
        
#elif UNITY_IPHONE
        private const string TestRewardedAdId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unexpected_platform";
#endif

        private void Start()
        {
            MobileAds.Initialize(initStatus => { });

            /*_rewardedAd = new RewardedAd(TestRewardedAdId);

            // Called when an ad request has successfully loaded.
            _rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
            // Called when an ad request failed to load.
            _rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
            // Called when an ad is shown.
            _rewardedAd.OnAdOpening += HandleRewardedAdOpening;
            // Called when an ad request failed to show.
            _rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
            // Called when the user should be rewarded for interacting with the ad.
            _rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
            // Called when the ad is closed.
            _rewardedAd.OnAdClosed += HandleRewardedAdClosed;*/

            CreateAndLoadRewardedAd();
        }

        private void CreateAndLoadRewardedAd()
        {
            _rewardedAd = new RewardedAd(TestRewardedAdId);

            // Called when an ad request has successfully loaded.
            _rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
            // Called when an ad request failed to load.
            _rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
            // Called when an ad is shown.
            _rewardedAd.OnAdOpening += HandleRewardedAdOpening;
            // Called when an ad request failed to show.
            _rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
            // Called when the user should be rewarded for interacting with the ad.
            _rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
            // Called when the ad is closed.
            _rewardedAd.OnAdClosed += HandleRewardedAdClosed;

            var request = new AdRequest.Builder().Build();
            _rewardedAd.LoadAd(request);
        }

        public void ShowRewardedAd()
        {
            if (!_rewardedAd.IsLoaded()) return;

            _rewardedAd.Show();
        }

        private void HandleRewardedAdLoaded(object sender, EventArgs e)
        {
            Debug.LogError("ad loaded");
        }

        private void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs e)
        {
            Debug.LogError("ad failed to load");
        }

        private void HandleRewardedAdOpening(object sender, EventArgs e)
        {
            Debug.LogError("ad opening");
        }

        private void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs e)
        {
            Debug.LogError("ad failed to show");
        }
        
        private void HandleUserEarnedReward(object sender, Reward e)
        {
            Debug.LogError("ad earned reward: " + e.Amount + "," + e.Type);
            Reward = e;
        }

        public Reward Reward { get; private set; }

        private void HandleRewardedAdClosed(object sender, EventArgs e)
        {
            Debug.LogError("ad closed");
            EventManager.userEarnedReward.Raise();
            CreateAndLoadRewardedAd();
        }

        public bool DoublePayoutAdIsLoaded()
        {
            return _rewardedAd.IsLoaded();
        }
    }
}