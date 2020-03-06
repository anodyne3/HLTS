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

        public async void Login()
        {
            _firebaseAuth = FirebaseAuth.DefaultInstance;
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
            else
            {
                //Debug.Log("RollReels success");
            }
        }
    }
}