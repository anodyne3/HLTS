using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.GameData;
using Core.MainSlotMachine;
using Core.Managers;
using Core.UI;
using DG.Tweening;
using Enums;
using Firebase;
using Firebase.Auth;
using Firebase.Functions;
using UnityEngine;
using Utils;

namespace Core
{
    public class FirebaseFunctionality : Singleton<FirebaseFunctionality>
    {
        private static FirebaseAuth _firebaseAuth;
        private FirebaseFunctions _firebaseFunc;
        private GameObject _gameEventListeners;

        public bool firebaseReady;

        public async void Init()
        {
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (dependencyStatus != DependencyStatus.Available)
            {
                AlertMessage.Init($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                return;
            }

            firebaseReady = true;
            CheckLogin();

            _firebaseFunc = FirebaseFunctions.DefaultInstance;

            _gameEventListeners = new GameObject("FirebaseGameEventListeners");
            _gameEventListeners.transform.SetParent(transform);
            EventManager.NewEventSubscription(_gameEventListeners, Constants.GameEvents.userEarnedRewardEvent,
                ClaimAdReward);
        }

        private void OnDestroy()
        {
            if (_firebaseAuth == null) return;

            _firebaseAuth.StateChanged -= AuthStateChanged;
            _firebaseAuth = null;
        }

        #region user

        private bool _signingIn;

        private async void AuthStateChanged(object sender, EventArgs eventArgs)
        {
            if (_firebaseAuth.CurrentUser == null && !_signingIn)
            {
                // AlertMessage.Init("No user signed in");
                await SignIn();
                return;
            }

            if (_firebaseAuth.CurrentUser == PlayerData.firebaseUser) return;

            var signedIn = PlayerData.firebaseUser != _firebaseAuth.CurrentUser &&
                           _firebaseAuth.CurrentUser != null;

            if (!signedIn)
            {
                if (PlayerData.firebaseUser != null)
                {
                    AlertMessage.Init("Signed out " + PlayerData.firebaseUser.UserId);
                }

                if (!firebaseReady || !PlayerData.ConsentGiven)
                {
                    AlertMessage.Init("Signed out - firebaseReady:" + firebaseReady + "; consentGiven:" +
                                      PlayerData.ConsentGiven);
                }
            }

            PlayerData.firebaseUser = _firebaseAuth.CurrentUser;

            if (!signedIn) return;

            StartCoroutine(PlayerData.OnLogin());
        }

        private void CheckLogin()
        {
            _firebaseAuth = FirebaseAuth.DefaultInstance;
            _firebaseAuth.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);
        }

        private async Task SignIn()
        {
            _signingIn = true;
            var task = _firebaseAuth.SignInAnonymouslyAsync();
            await task;
            if (task.IsCanceled)
            {
                AlertMessage.Init("SignInAnonymouslyAsync was cancelled.");
                return;
            }

            if (task.IsFaulted)
            {
                AlertMessage.Init("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            _signingIn = _firebaseAuth.CurrentUser != null;
            // var newUser = task.Result;
            // AlertMessage.Init("User signed in successfully: " + newUser.DisplayName + " (" + newUser.UserId + ")");
        }

        public void ResetAccount()
        {
            PlayerPrefs.DeleteKey(Constants.ConsentKey);
            PlayerPrefs.DeleteKey(Constants.WarningKey);
            PlayerData.StopDatabaseListeners();
            GlobalComponents.Instance.RemoveGlobalComponent<PlayerData>();
            DeleteAccount();
            Destroy(_gameEventListeners);
            GlobalComponents.Instance.RemoveGlobalComponent<FirebaseFunctionality>();
            GlobalComponents.Instance.RemoveGlobalComponent<CoinTray>();
            GlobalComponents.Instance.RemoveGlobalComponent<SlotMachine>();
            SceneManager.LoadSceneAsynchronously(0);
            DOTween.Clear();
        }

        private static void DeleteAccount()
        {
            _firebaseAuth.CurrentUser?.DeleteAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    AlertMessage.Init("DeleteAsync was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    AlertMessage.Init("DeleteAsync encountered an error: " + task.Exception);
                    return;
                }

                AlertMessage.Init("User deleted successfully.");
            });
        }

        public void LinkAccount()
        {
            //Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);
        }

        public void UnlinkAccount()
        {
        }

        #endregion

        #region slots

        public async Task RollReels(int betAmount)
        {
            var responseData = await GetHttpsCallable(betAmount.ToString(), Constants.ReelRollCloudFunction);

            if (responseData == null) return;

            var payoutAmount = ProcessBasicResponseData<long>(responseData);
            SlotMachine.payoutAmount = payoutAmount;
        }

        #endregion

        #region Ads

        public AdType shownAd = AdType.None;

        private async void ClaimAdReward()
        {
            shownAd = AdManager.reward;

            var shownAdId = ((int) shownAd).ToString();
            var responseData = await GetHttpsCallable(shownAdId, Constants.AdRewardClaimCloudFunction);

            switch (shownAd)
            {
                case AdType.DoublePayout:
                    var payoutAmount = ProcessBasicResponseData<long>(responseData);
                    AlertMessage.Init("Extra " + payoutAmount + " Banana Coins awarded");
                    PanelManager.GetPanel<PayoutPanelController>().ProcessAdReward();
                    break;
                case AdType.DoubleChest:
                    AlertMessage.Init("Additional Chest contents awarded");
                    PanelManager.GetPanel<ChestOpenPanelController>().ProcessAdReward();
                    return;
                default:
                    AlertMessage.Init("Something went wrong with Ad Reward claim");
                    return;
            }
            
            shownAd = AdType.None;
        }

        #endregion

        #region Chests

        public async Task ClaimChest(ChestType claimedChest)
        {
            PanelManager.WaitingForServerPanel();
            var chestClaimId = claimedChest.ToString();
            var responseData = await GetHttpsCallable(chestClaimId, Constants.ChestClaimCloudFunction);

            var chestId = ProcessBasicResponseData<long>(responseData);
            ChestManager.ChestClaimed((ChestType) chestId);
        }

        public async Task ChestOpen(ChestType chestType)
        {
            PanelManager.WaitingForServerPanel();
            var chestOpenId = ((int) chestType).ToString();
            var responseData = await GetHttpsCallable(chestOpenId, Constants.ChestOpenCloudFunction);

            if (responseData == null)
            {
                AlertMessage.Init("ChestOpen returned empty data");
                return;
            }

            ChestManager.OpenChestPayoutPanel(new ChestRewardDto(responseData));
        }

        public async Task ChestMerge(string chestMergeLevel)
        {
            PanelManager.WaitingForServerPanel();
            var responseData = await GetHttpsCallable(chestMergeLevel, Constants.ChestMergeCloudFunction);

            if (chestMergeLevel == ProcessBasicResponseData<string>(responseData))
                ChestManager.CompleteMerge();
        }

        #endregion

        #region Upgrades

        public async Task Upgrade(UpgradeTypes upgradeType)
        {
            PanelManager.WaitingForServerPanel();
            var upgradeId = ((int) upgradeType).ToString();
            var responseData = await GetHttpsCallable(upgradeId, Constants.DoUpgradeCloudFunction);

            PanelManager.GetPanel<UpgradePanelController>()
                .UpgradeComplete(ProcessBasicResponseData<long>(responseData));
        }

        #endregion

        #region Narrative

        [HideInInspector] public bool narrativeCallBlock;

        public async void UpdateNarrativeProgress(NarrativeTypes narrativeType)
        {
            if (narrativeCallBlock) return;

            narrativeCallBlock = true;

            var narrativeState = NarrativeManager.UpdateNarrativeState(narrativeType);

            await GetHttpsCallable(narrativeState, Constants.ProgressNarrativeFunction);

            narrativeCallBlock = false;
        }

        #endregion

        #region Shop

        public async Task PurchaseProduct(string shopProductId)
        {
            PanelManager.WaitingForServerPanel();
            var responseData = await GetHttpsCallable(shopProductId, Constants.ProductPurchaseFunction);

            var product = ProcessBasicResponseData<string>(responseData);

            ShopManager.CompletePurchase(product);
        }

        #endregion

        #region Common

        private async Task<object> GetHttpsCallable(string sendData, string callName)
        {
            var data = new Dictionary<string, object> {["text"] = sendData, ["push"] = true};

            if (_firebaseFunc == null)
                return null;

            var task = _firebaseFunc.GetHttpsCallable(callName).CallAsync(data);

            var response = await task;
            if (task.IsFaulted)
                HandleFunctionError(task);

            PanelManager.WaitingForServerPanel(false);

            if (response.Data != null)
                return response.Data;

            if (callName != Constants.ProgressNarrativeFunction)
                AlertMessage.Init(callName + " returned empty");

            return null;
        }

        private static void HandleFunctionError(Task httpsCallableResult)
        {
            if (httpsCallableResult.Exception == null)
            {
                AlertMessage.Init("Communication Error - Exception is null");
                return;
            }

            foreach (var innerException in httpsCallableResult.Exception.InnerExceptions)
            {
                if (!(innerException is FunctionsException e)) continue;

                var code = e.ErrorCode;
                var message = e.Message;
                AlertMessage.Init(code + message);
            }
        }

        private static T ProcessBasicResponseData<T>(object data)
        {
            var processedData = (Dictionary<object, object>) data;

            if (processedData.ContainsKey("text")) return (T) processedData["text"];

            return (T) data;
        }

        #endregion
    }
}