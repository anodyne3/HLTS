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
using Firebase.Unity.Editor;
using MyScriptableObjects;
using UnityEngine;
using Utils;

namespace Core
{
    public class FirebaseFunctionality : Singleton<FirebaseFunctionality>
    {
        private static FirebaseAuth _firebaseAuth;
        private FirebaseApp _firebaseApp;
        private FirebaseFunctions _firebaseFunc;
        private GameObject _gameEventListeners;

        public bool firebaseReady;

        public async void Init()
        {
            var dependencyStatus = FirebaseApp.CheckAndFixDependenciesAsync();
            await dependencyStatus;

            if (dependencyStatus.Result == DependencyStatus.Available)
            {
                _firebaseApp = FirebaseApp.DefaultInstance;
                firebaseReady = true;
                CheckLogin();
            }
            else
                AlertMessage.Init($"Could not resolve all Firebase dependencies: {dependencyStatus.Result}");

            _firebaseApp.SetEditorDatabaseUrl(Constants.FirebaseDatabaseUrl);
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

        private void AuthStateChanged(object sender, EventArgs eventArgs)
        {
            if (_firebaseAuth.CurrentUser == null && !_signingIn)
            {
                AlertMessage.Init("No user signed in");
                SignIn();
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

            if (signedIn)
                StartCoroutine(PlayerData.OnLogin());
        }

        private void CheckLogin()
        {
            _firebaseAuth = FirebaseAuth.DefaultInstance;
            _firebaseAuth.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);
        }

        private async void SignIn()
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

            var newUser = task.Result;
            _signingIn = _firebaseAuth.CurrentUser != null;
            AlertMessage.Init("User signed in successfully: " + newUser.DisplayName + " (" + newUser.UserId + ")");
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

        public async void RollReels(int betAmount)
        {
            var responseData = await GetHttpsCallable(betAmount.ToString(), Constants.ReelRollCloudFunction);

            if (responseData == null) return;
            
            var payoutAmount = ProcessBasicResponseData<long>(responseData);
            SlotMachine.payoutAmount = payoutAmount;
        }

        #endregion

        #region Ads

        public AdType shownAd;
        
        private async void ClaimAdReward()
        {
            shownAd = (AdType) Enum.Parse(typeof(AdType), AdManager.reward.Type);
            var shownAdId = ((int) shownAd).ToString();
            var responseData = await GetHttpsCallable(shownAdId, Constants.AdRewardClaimCloudFunction);

            switch (shownAd)
            {
                case AdType.DoublePayout:
                    var payoutAmount = ProcessBasicResponseData<long>(responseData);
                    AlertMessage.Init("Extra " + payoutAmount + " Banana Coins awarded");
                    return;
                case AdType.DoubleChest:
                    AlertMessage.Init("Additional Chest contents awarded");
                    return;
                default:
                    AlertMessage.Init("Something went wrong with Ad Reward claim");
                    return;
            }
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus || AdManager.reward == null) return;

            ProcessReward();
        }

        private void ProcessReward()
        {
            switch (shownAd)
            {
                case AdType.DoublePayout:
                    PanelManager.GetPanel<PayoutPanelController>().ProcessAdReward();
                    break;
                case AdType.DoubleChest:
                    PanelManager.GetPanel<ChestOpenPanelController>().ProcessAdReward();
                    break;
                default:
                    AlertMessage.Init("Something went wrong with Ad Reward claim");
                    break;
            }
            
            AdManager.reward = null;
        }

        #endregion

        #region Chests

        public async void ClaimChest(ChestType claimedChest)
        {
            PanelManager.WaitingForServerPanel();
            var chestClaimId = claimedChest.ToString();
            var responseData = await GetHttpsCallable(chestClaimId, Constants.ChestClaimCloudFunction);

            var chestId = ProcessBasicResponseData<long>(responseData);
            ChestManager.ChestClaimed((ChestType) chestId);
        }

        public async void ChestOpen(ChestType chestType)
        {
            PanelManager.WaitingForServerPanel();
            var chestOpenId = ((int) chestType).ToString();
            var responseData = await GetHttpsCallable((chestOpenId), Constants.ChestOpenCloudFunction);

            if (responseData == null)
            {
                AlertMessage.Init("ChestOpen returned empty data");
                return;
            }

            ChestManager.OpenChestPayoutPanel(new ChestRewardDto(responseData));
        }

        public async void ChestMerge(string chestMergeLevel)
        {
            PanelManager.WaitingForServerPanel();
            var responseData = await GetHttpsCallable(chestMergeLevel, Constants.ChestMergeCloudFunction);

            if (chestMergeLevel == ProcessBasicResponseData<string>(responseData))
                ChestManager.CompleteMerge();
        }

        #endregion

        #region Upgrades

        public async void Upgrade(UpgradeVariable upgradeVariable)
        {
            PanelManager.WaitingForServerPanel();
            var upgradeId = ((int) upgradeVariable.upgradeType).ToString();
            var responseData = await GetHttpsCallable(upgradeId, Constants.DoUpgradeCloudFunction);

            PanelManager.GetPanel<UpgradePanelController>()
                .UpgradeComplete(ProcessBasicResponseData<long>(responseData));
        }

        #endregion

        #region Narrative

        [HideInInspector] public bool narrativeCallBlock;
        
        public async void ProgressNarrativePoint()
        {
            if (narrativeCallBlock) return;

            narrativeCallBlock = true;
            await GetHttpsCallable(Constants.ProgressNarrativeFunction);

            narrativeCallBlock = false;
            //kill this once tested
            AlertMessage.Init("Narrative Progressed");
        }

        #endregion

        #region Shop

        public async void PurchaseProduct(string shopProductId)
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

            AlertMessage.Init(callName + " returned empty");
            return null;
        }
        
        private async Task<object> GetHttpsCallable(string callName)
        {
            if (_firebaseFunc == null) 
                return null;
            
            var task = _firebaseFunc.GetHttpsCallable(callName).CallAsync();

            await task;
            
            if (task.IsFaulted)
                HandleFunctionError(task);

            PanelManager.WaitingForServerPanel(false);

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