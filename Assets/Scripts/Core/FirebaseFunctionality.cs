﻿using System.Collections.Generic;
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
using UnityEngine.UI;
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
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus.Result}");

            _firebaseApp.SetEditorDatabaseUrl(Constants.FirebaseDatabaseUrl);
            _firebaseFunc = FirebaseFunctions.DefaultInstance;

            _gameEventListeners = new GameObject("FirebaseGameEventListeners");
            _gameEventListeners.transform.SetParent(transform);
            EventManager.NewEventSubscription(_gameEventListeners, Constants.GameEvents.wheelRollEvent, RollReels);
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

        private void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {
            if (_firebaseAuth.CurrentUser == null && !_signingIn)
            {
                Debug.Log("No user signed in");
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
                    Debug.Log("Signed out " + PlayerData.firebaseUser.UserId);
                }

                if (!firebaseReady || !PlayerData.ConsentGiven)
                {
                    Debug.Log("Signed out - firebaseReady:" + firebaseReady + "; consentGiven:" +
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
                Debug.LogError("SignInAnonymouslyAsync was cancelled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            var newUser = task.Result;
            _signingIn = _firebaseAuth.CurrentUser != null;
            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
        }

        public void ResetAccount()
        {
            PlayerPrefs.DeleteKey(Constants.ConsentKey);
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

        private void DeleteAccount()
        {
            _firebaseAuth.CurrentUser?.DeleteAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("DeleteAsync was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User deleted successfully.");
            });
        }

        //probably unnecessary 
        public void SignOut()
        {
            // PlayerData.StopDatabaseListeners();
            _firebaseAuth.SignOut();
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

        private async void RollReels()
        {
            await ReelRoll();
        }

        private async Task ReelRoll()
        {
            var rollReel = _firebaseFunc.GetHttpsCallable(Constants.ReelRollCloudFunction).CallAsync();

            await rollReel;
            if (rollReel.IsFaulted)
            {
                //maybe show message to player regarding failed internet
                HandleFunctionError(rollReel);
            }
        }

        #endregion

        #region Ads

        private async void ClaimAdReward()
        {
            PanelManager.WaitingForServerPanel();
            await AdRewardClaim();
        }

        private async Task AdRewardClaim()
        {
            var adRewardClaim = _firebaseFunc.GetHttpsCallable(Constants.AdRewardClaimCloudFunction).CallAsync();

            await adRewardClaim;
            if (adRewardClaim.IsFaulted)
            {
                //maybe show message to player regarding some issue
                HandleFunctionError(adRewardClaim);
            }
            
            PanelManager.WaitingForServerPanel(false);
        }

        #endregion

        #region Chests

        public async void ClaimChest(ChestType claimedChest)
        {
            PanelManager.WaitingForServerPanel();
            await ChestClaim(claimedChest);
        }

        private async Task ChestClaim(ChestType claimedChest)
        {
            var data = new Dictionary<object, object> {["text"] = claimedChest.ToString(), ["push"] = true};

            var chestClaim = _firebaseFunc.GetHttpsCallable(Constants.ChestClaimCloudFunction).CallAsync(data);

            var response = await chestClaim;
            if (chestClaim.IsFaulted)
            {
                //maybe show message to player regarding some issue
                HandleFunctionError(chestClaim);
            }

            PanelManager.WaitingForServerPanel(false);
            
            //maybe show message to player regarding some issue
            if (response.Data == null)
            {
                Debug.LogError("ChestClaim returned empty data");
                return;
            }

            var chestId = ProcessBasicResponseData(response.Data);
            
            ChestManager.ChestAdded((ChestType)chestId);
        }

        public async void OpenChest(ChestType chestType)
        {
            PanelManager.WaitingForServerPanel();
            await ChestOpen(((int) chestType).ToString());
        }

        private async Task ChestOpen(string chestType)
        {
            var data = new Dictionary<string, object> {["text"] = chestType, ["push"] = true};

            var chestClaim = _firebaseFunc.GetHttpsCallable(Constants.ChestOpenCloudFunction)
                .CallAsync(data);

            var response = await chestClaim;
            if (chestClaim.IsFaulted)
            {
                //maybe show message to player regarding some issue
                HandleFunctionError(chestClaim);
            }

            PanelManager.WaitingForServerPanel(false);
            
            //maybe show message to player regarding some issue
            if (response.Data == null)
            {
                Debug.LogError("ChestOpen returned empty data");
                return;
            }

            ChestManager.OpenChest(new ChestRewardDto(response.Data));
        }

        #endregion

        #region Upgrades

        public async void Upgrade(UpgradeVariable upgradeVariable)
        {
            PanelManager.WaitingForServerPanel();
            await DoUpgrade(((int) upgradeVariable.upgradeType).ToString());
        }

        private async Task DoUpgrade(string upgradeId)
        {
            var data = new Dictionary<string, object> {["text"] = upgradeId, ["push"] = true};

            var doUpgrade = _firebaseFunc.GetHttpsCallable(Constants.DoUpgradeRepair).CallAsync(data);

            var response = await doUpgrade;
            if (doUpgrade.IsFaulted)
            {
                //maybe show message to player regarding some issue
                HandleFunctionError(doUpgrade);
            }

            PanelManager.WaitingForServerPanel(false);
            
            //maybe show message to player regarding some issue
            if (response.Data == null)
            {
                Debug.LogError("ChestClaim returned empty data");
                return;
            }

            UpgradeManager.upgradeId = ProcessBasicResponseData(response.Data);

            
            PanelManager.GetPanel<UpgradePanelController>().UpgradeComplete();
        }

        #endregion

        #region Common

        private static void HandleFunctionError(Task httpsCallableResult)
        {
            if (httpsCallableResult.Exception == null)
            {
                Debug.LogError("adRewardClaim.Exception is null");
                return;
            }

            foreach (var innerException in httpsCallableResult.Exception.InnerExceptions)
            {
                if (!(innerException is FunctionsException e)) continue;

                var code = e.ErrorCode;
                var message = e.Message;
                Debug.LogError(code + message);
            }
        }

        private static long ProcessBasicResponseData(object data)
        {
            var processedData = (Dictionary<object, object>) data;

            if (!processedData.ContainsKey("id")) return -1;

            return (long) processedData["id"];
        }

        #endregion
    }
}