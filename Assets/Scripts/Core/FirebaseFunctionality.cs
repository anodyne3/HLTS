using System.Threading.Tasks;
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

        public bool firebaseReady;

        private async void Start()
        {
            var dependencyStatus = FirebaseApp.CheckAndFixDependenciesAsync();
            await dependencyStatus;

            if (dependencyStatus.Result == DependencyStatus.Available)
            {
                _firebaseApp = FirebaseApp.DefaultInstance;
                firebaseReady = true;
            }
            else
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus.Result}");

            _firebaseApp.SetEditorDatabaseUrl("https://he-loves-the-slots.firebaseio.com/");
            _firebaseFunc = FirebaseFunctions.DefaultInstance;

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.wheelRollEvent, ReelRoll);
        }

        private void OnDestroy()
        {
            if (_firebaseAuth == null) return;
            
            _firebaseAuth.StateChanged -= AuthStateChanged;
            _firebaseAuth = null;
        }
        
        #region user

        private void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {
            if (_firebaseAuth.CurrentUser == PlayerData.firebaseUser) return;

            var signedIn = PlayerData.firebaseUser != _firebaseAuth.CurrentUser && _firebaseAuth.CurrentUser != null;
            if (!signedIn)
            {
                if (PlayerData.firebaseUser != null)
                {
                    Debug.Log("Signed out " + PlayerData.firebaseUser.UserId);
                    SignIn();
                }
                else
                {
                    Debug.Log("Signed out & firebase dependency issue");
                    SceneManager.LoadSceneAsynchronously(0);
                }

                return;
            }

            PlayerData.firebaseUser = _firebaseAuth.CurrentUser;
            Debug.Log("Signed in " + PlayerData.firebaseUser.UserId);
            StartCoroutine(PlayerData.OnLogin());
        }

        public void CheckLogin()
        {
            _firebaseAuth = FirebaseAuth.DefaultInstance;
            _firebaseAuth.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);
        }

        private async void SignIn()
        {
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
            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
            PlayerData.firebaseUser = newUser;
            StartCoroutine(PlayerData.OnLogin());
        }

        public void SignOut()
        {
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
        
        private async void ReelRoll()
        {
            await RollReels();
        }

        private async Task RollReels()
        {
            var rollReel = _firebaseFunc.GetHttpsCallable(Constants.ReelRollCloudFunction).CallAsync();

            await rollReel;
            if (rollReel.IsFaulted)
            {
                //maybe player message regarding failed internet
                if (rollReel.Exception == null)
                    Debug.LogError("reelRoll.Exception is null");

                foreach (var innerException in rollReel.Exception.InnerExceptions)
                {
                    if (!(innerException is FunctionsException e)) continue;

                    var code = e.ErrorCode;
                    var message = e.Message;
                    Debug.LogError(code + message);
                }
            }
        }
        
        #endregion
    }
}