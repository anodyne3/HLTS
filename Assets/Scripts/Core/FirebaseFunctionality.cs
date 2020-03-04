using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;
using UnityEngine;

namespace Core
{
    public class FirebaseFunctionality : Singleton<FirebaseFunctionality>
    {
        private static FirebaseAuth _firebaseAuth;
        private FirebaseApp _firebaseApp;

        public FirebaseUser firebaseUser;
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

            firebaseUser = newUser;
            PlayerData.OnLogin();
        }
    }
}