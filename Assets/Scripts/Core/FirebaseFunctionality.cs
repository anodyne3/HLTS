using System.Threading.Tasks;
using Core.GameData;
using Core.MainSlotMachine;
using DG.Tweening;
using Firebase;
using Firebase.Auth;
using Firebase.Functions;
using Firebase.Unity.Editor;
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
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus.Result}");

            _firebaseApp.SetEditorDatabaseUrl(Constants.FirebaseDatabaseUrl);
            _firebaseFunc = FirebaseFunctions.DefaultInstance;
            
            _gameEventListeners = new GameObject("FirebaseGameEventListeners");
            _gameEventListeners.transform.SetParent(transform);
            EventManager.NewEventSubscription(_gameEventListeners, Constants.GameEvents.wheelRollEvent, RollReels);
            EventManager.NewEventSubscription(_gameEventListeners, Constants.GameEvents.userEarnedRewardEvent, ClaimAdReward);
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
                    Debug.Log("Signed out - firebaseReady:" + firebaseReady + "; consentGiven:" + PlayerData.ConsentGiven);
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
            _firebaseAuth.CurrentUser?.DeleteAsync().ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("DeleteAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User deleted successfully.");
            });
        }

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
        }
        #endregion
        
        #region Chests
        public async void ClaimChest()
        {
            await ChestClaim();
        }

        private async Task ChestClaim()
        {
            var chestClaim = _firebaseFunc.GetHttpsCallable(Constants.ChestClaimCloudFunction).CallAsync();

            await chestClaim;
            if (chestClaim.IsFaulted)
            {
                //maybe show message to player regarding some issue
                HandleFunctionError(chestClaim);
            }
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

        #endregion
    }
}